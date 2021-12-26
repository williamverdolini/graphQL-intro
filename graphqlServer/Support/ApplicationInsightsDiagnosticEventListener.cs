using System.Diagnostics;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace graphqlServer.Support
{
    // see: https://github.com/ChilliCream/hotchocolate/issues/1596
    public class ApplicationInsightsDiagnosticEventListener : ExecutionDiagnosticEventListener
    {
        private readonly ILogger<ApplicationInsightsDiagnosticEventListener> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly bool _enabled;
        private readonly bool _traceResolversEnabled;

        public ApplicationInsightsDiagnosticEventListener(
            ILogger<ApplicationInsightsDiagnosticEventListener> logger,
            TelemetryClient telemetryClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _enabled = configuration.GetValue<bool>("MonitoringEnabled", false);
            _traceResolversEnabled = configuration.GetValue<bool>("TraceResolversEnabled", false);
        }

        public override IDisposable ExecuteRequest(IRequestContext context)
        {
            if(!_enabled) {
                return EmptyScope;
            }
            var httpContext = GetHttpContextFrom(context);
            if (httpContext == null)
                return EmptyScope;

            //During debugging every playground action will come here so we want this while debugging
#if DEBUG
            if (context.Request.OperationName == "IntrospectionQuery")
                return EmptyScope;
#endif

            //Create a new telemetry request
            var operationPath = $"{context.Request.OperationName ?? "UnknownOperation"} - {context.Request.QueryHash}";
            var requestTelemetry = new RequestTelemetry()
            {
                Name = $"/graphql/{operationPath}",
                Url = new Uri(httpContext.Request.GetUri().AbsoluteUri + operationPath),
            };

            requestTelemetry.Context.Operation.Name = $"POST /graphql/{operationPath}";
            requestTelemetry.Context.Operation.Id = GetOperationIdFrom(httpContext);
            requestTelemetry.Context.Operation.ParentId = GetOperationIdFrom(httpContext);
            requestTelemetry.Context.User.AuthenticatedUserId = httpContext.User.Identity?.Name ?? "Not authenticated";

            if (context.Request.Query != null)
                requestTelemetry.Properties.Add("GraphQL Query", context.Request.Query.ToString());

            var operation = _telemetryClient.StartOperation(requestTelemetry);
            return new ScopeWithEndAction(() => OnEndRequest(context, operation)); // Request end.
        }


        public override bool EnableResolveFieldValue => _traceResolversEnabled;
        public override IDisposable ResolveFieldValue(IMiddlewareContext context)
        {
            if(!EnableResolveFieldValue) {
                return base.ResolveFieldValue(context);
            }
            var sw = new Stopwatch();
            sw.Start();
            base.ResolveFieldValue(context);
            sw.Stop();
            return new ScopeWithEndAction(() => Console.WriteLine("TRACE RESOLVER:"+context.Selection.Field.Name + " - ms:" + sw.ElapsedMilliseconds));
        }

        private void OnEndRequest(IRequestContext context, IOperationHolder<RequestTelemetry> operation)
        {
            var httpContext = GetHttpContextFrom(context);
            operation.Telemetry.Success = httpContext!.Response.StatusCode is >= 200 and <= 299;
            operation.Telemetry.ResponseCode = httpContext.Response.StatusCode.ToString();

            if (context.Exception != null)
            {
                operation.Telemetry.Success = false;
                _telemetryClient.TrackException(context.Exception);
            }

            if (context.ValidationResult?.HasErrors ?? false)
            {
                operation.Telemetry.Success = false;
                operation.Telemetry.ResponseCode = "400 - Validation Error";
            }

            if (context.Result is { ContextData: null } and not { Errors: null })
            {
                if (context.Result?.Errors != null)
                {
                    foreach (var error in context.Result.Errors)
                    {
                        if (error.Exception != null)
                        {
                            operation.Telemetry.Success = false;
                            _telemetryClient.TrackException(error.Exception);
                        }
                    }
                }
            }

            _telemetryClient.StopOperation(operation);
        }

        public override void RequestError(IRequestContext context, Exception exception)
        {
            _telemetryClient.TrackException(exception);
            base.RequestError(context, exception);
        }

        public override void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
        {
            foreach (var error in errors)
            {
                _telemetryClient.TrackTrace("GraphQL validation error: " + error.Message, SeverityLevel.Warning);
            }
            base.ValidationErrors(context, errors);
        }

        private HttpContext? GetHttpContextFrom(IRequestContext context)
        {
            // This method is used to enable start/stop events for query.
            if (!context.ContextData.ContainsKey("HttpContext"))
                return null;

            return context.ContextData["HttpContext"] as HttpContext;
        }

        private string GetOperationIdFrom(HttpContext context) => context.TraceIdentifier;

    }

    internal class ScopeWithEndAction : IDisposable
    {
        private readonly Action _disposeAction;

        public ScopeWithEndAction(Action disposeAction) => _disposeAction = disposeAction;

        public void Dispose() => _disposeAction.Invoke();
    }
}