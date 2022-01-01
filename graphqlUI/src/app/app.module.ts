import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import {HttpClientModule, HttpHeaders} from '@angular/common/http';
import {APOLLO_OPTIONS} from 'apollo-angular';
import {HttpLink} from 'apollo-angular/http';
import {ApolloLink, concat, InMemoryCache} from '@apollo/client/core';


import { AppComponent } from './app.component';
import { TokenService } from './services/token.service';
import { FormsModule } from '@angular/forms';
import { GraphQLQueryComponent } from './graphQL-query/graphQL-query.component';

@NgModule({
  declarations: [
    AppComponent,
    GraphQLQueryComponent,
  ],
  imports: [
    BrowserModule, HttpClientModule, FormsModule
  ],
  providers: [
    {
      provide: APOLLO_OPTIONS,
      useFactory: (httpLink: HttpLink, tokenService: TokenService) => {
        const http = httpLink.create({uri: 'https://localhost:7059/graphql'});
        const authMiddleware = new ApolloLink((operation, forward) => {
          // add the authorization to the headers
          if(tokenService.token != null) {
            operation.setContext({
              headers: new HttpHeaders().set('Authorization', tokenService.token)
            });
          }

          return forward(operation);
        });

        return {
          cache: new InMemoryCache(),
          link: concat(authMiddleware, http),
          // Disable cache for demo
          defaultOptions: {
            watchQuery: {
              fetchPolicy: 'no-cache',
              errorPolicy: 'ignore',
            },
            query: {
              fetchPolicy: 'no-cache',
              errorPolicy: 'all',
            },
          }
        };
      },
      deps: [HttpLink, TokenService],
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
