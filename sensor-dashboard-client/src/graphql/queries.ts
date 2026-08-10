import { gql } from '@apollo/client'

/** Latest sensor events with HotChocolate paging, filtering and sorting. */
export const GET_SENSOR_EVENTS = gql`
  query GetSensorEvents(
    $first: Int!
    $where: SensorEventFilterInput
    $order: [SensorEventSortInput!]
  ) {
    sensorEvents(first: $first, where: $where, order: $order) {
      nodes {
        id
        type
        name
        payload
        receivedAt
        createdAt
      }
      totalCount
    }
  }
`

/** Aggregations used by bar/pie charts. */
export const GET_STATS = gql`
  query GetStats {
    statsByType {
      type
      count
    }
    statsByLocation {
      type
      count
    }
  }
`
