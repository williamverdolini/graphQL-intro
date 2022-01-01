import { Component, OnInit } from '@angular/core';
import { Apollo, gql } from 'apollo-angular';
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-graphql-query',
  templateUrl: './graphQL-query.component.html',
  styleUrls: ['./graphQL-query.component.css', './../app.component.css']
})
export class GraphQLQueryComponent implements OnInit {

  data: any[] = [];
  loading = true;
  errors: any;
  token = null;

  queries: { [key: string]: string[]; } = {
    init: [`
    {
      books(first: 1, where: { title: { startsWith: "Reconstituirea" } }) {
        nodes {
          id @decodeBase64
          title
          publisher {
            name
            address
          }
        }
      }
    }
  `,''],
    authors: [`
    query autori {
      authors (where: {
        and: {
          firstName: { startsWith: "R" }
          surnName: { startsWith: "B"}
          }
      }
      order: {
        firstName: ASC
      }){
        nodes {
        id
        firstName
        surnName

        }
      }
  }
  `,''],
    book: [`
    {
      books(first: 1, where: { title: { startsWith: "Reconstituirea" } }) {
        nodes {
          id @decodeBase64
          title
          authors {
            firstName
            surnName
          }
          publisher {
            name
            address
          }
        }
      }
    }
    `,''],
    publisher: [`
    {
      publishers(first:5) {
          nodes {
            id
          }
        }
      }
    `,''],
    authorsFragment: [`
    query autoriFragment($shouldInclude: Boolean!) {
        authors(first: 10) @include(if:$shouldInclude) {
          nodes {
            ...Autore
          }
        }
    }

    fragment Autore on Author {
      id
      firstName
      surnName
    }
    `,`
    {
      "shouldInclude": true
    }
    `]
  }

  query = this.queries['init'][0];
  variables = this.queries['init'][1];

  constructor(
    private apollo: Apollo,
    private tokenService: TokenService
  ) { }

  ngOnInit() {
    this.executeQuery();
  }

  executeQuery() {
    let variables = null;
    try {
      variables = JSON.parse(this.variables);
    }
    catch { }

    this.apollo
      .query({
        query: gql(this.query),
        variables: variables,
        errorPolicy: 'all'
      })
      .subscribe((result: any) => {
        console.log("GRAPHQL DATA", result);
        this.data = result?.data;
        this.loading = result.loading;
        this.errors = result.errors;
      });
  }

  setToken(newToken: string) {
    let token = null;
    if (newToken != null) {
      token = "Bearer " + newToken
    }
    this.tokenService.setToken(token as string);
  }

  setQuery(query: string) {
    this.query = this.queries[query][0];
    this.variables = this.queries[query][1];
  }

  setVariables(variables: string) {
    this.variables = variables;
  }
}
