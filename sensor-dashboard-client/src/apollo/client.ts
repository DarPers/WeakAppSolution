import { ApolloClient, HttpLink, InMemoryCache } from '@apollo/client'

// Relative URL works with Vite proxy (dev) and nginx proxy (Docker).
const graphqlUri = import.meta.env.VITE_GRAPHQL_URL ?? '/graphql'

export const apolloClient = new ApolloClient({
  link: new HttpLink({ uri: graphqlUri }),
  cache: new InMemoryCache(),
  defaultOptions: {
    watchQuery: {
      fetchPolicy: 'cache-and-network',
    },
    query: {
      fetchPolicy: 'network-only',
    },
  },
})
