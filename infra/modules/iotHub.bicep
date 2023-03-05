param location string

param name string

param sku string = 'F1'

param capacity int = 1

resource IoTHub 'Microsoft.Devices/IotHubs@2022-04-30-preview' = {
  name: name
  location: location
  sku: {
    name: sku
    capacity: capacity
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    ipFilterRules: []
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: 2
      }
    }
    routing: {
      routes: [
        {
          name: 'ToDefaultBuiltin'
          source: 'DeviceMessages'
          condition: 'true'
          endpointNames: [
            'events'
          ]
          isEnabled: true
        }
      ]
      fallbackRoute: {
        isEnabled: false
        name: '$fallback'
        source: 'DeviceMessages'
        endpointNames: [
          'events'
        ]
      }
    }
    enableDataResidency: false
    enableFileUploadNotifications: false
    cloudToDevice: {
      maxDeliveryCount: 10
      defaultTtlAsIso8601: 'PT1H'
      feedback: {
        lockDurationAsIso8601: 'PT1M'
        ttlAsIso8601: 'PT1H'
        maxDeliveryCount: 10
      }
    }
    features: 'DeviceManagement'
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
    allowedFqdnList: []
  }
}
