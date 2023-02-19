param location string

param name string

@secure()
param allowedIpV4CIDR string

resource IoTHub 'Microsoft.Devices/IotHubs@2022-04-30-preview' = {
  name: name
  location: location
  sku: {
    name: 'S1'
    capacity: 1
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    ipFilterRules: []
    networkRuleSets: {
      defaultAction: 'Deny'
      applyToBuiltInEventHubEndpoint: false
      ipRules: [
        {
          filterName: 'developer-machine'
          action: 'Allow'
          ipMask: allowedIpV4CIDR
        }
      ]
    }
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