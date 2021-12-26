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
