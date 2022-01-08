# A step-by-step intro to GraphQL in .NET

## Prerequisites

You must have installed in your machine:
- MongoDb: versione >= 4.0. The project works with DB on port 27017, not authenticated
- .NET: version 6+
- VS code

## How to follow the introduction

This introduction is structured in progressive steps. Each of the steps is managed on a separate branch which includes the previous steps; in this way it is possible to follow the steps of the guide by advancing in the numbered branches and following the notes present in the linked wiki.

## Scope

The guide is intended to allow a guided introduction to the use of GraphQL with .NET framework. In my learning path I have decided to limit the scope of my research/study focusing exclusively on research capabilities and functionalities; for this reason I do NOT deal with issues related to [Mutations](https://graphql.org/learn/queries/#mutations) or [Subscriptions](https://graphql.org/blog/subscriptions-in-graphql-and-relay/) or other topics at all. In this intro I will cover:

- an overview of what GraphQL is and its specifications
- how to use GraphQL to read data in databases (MongoDb in particular)
- how to implement useful aspects for the execution in production, such as eg. Security, Monitoring and Caching
- how to allow a client [Angular](https://angular.io/) to make GraphQL calls

## Intro steps

1. [Project Setup](./wiki/01.project-setup.md)
2. [Hotchocolate GraphQL](./wiki/02.hotchocolate-intro.md)
3. [Relay](./wiki/03.relay.md)
4. [Resolvers](./wiki/04.resolvers.md)
5. [DataLoaders](./wiki/05.dataloaders.md)
6. [Middlewares](./wiki/06.middlewares.md)
7. [Monitoring with Application Insights](./wiki/07.monitoring.md)
8. [Automatic Persisted Queries](./wiki/08.automatic-persisted-queries.md)
9. [GraphQL tools](./wiki/09.tools.md)
10. [Security](./wiki/10.security.md)
11. [Authentication/Authorization](./wiki/11.authorization.md)
12. [Apollo Angular](./wiki/12.apollo-angular-client.md)
13. [Schema Stitching](./wiki/13.schema-stitching.md)
14. [Schema Federation](./wiki/14.schema-federation.md)
15. [Polymorphic Ids](./wiki/15.polymorphic-ids.md)
16. [GraphQL Testing](./wiki/16.graphql-testing.md)

If you want to hands on this intro, clone the repo and switch to the step branches. 

```bash
git clone https://github.com/williamverdolini/graphQL-intro.git
cd graphQL-intro/
git switch 01.project-setup
```

Have fun.