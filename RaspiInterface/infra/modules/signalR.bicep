param name string

param location string

@allowed([
  'Default'
  'Serverless'
  'Classic'
])
param serviceMode string = 'Default'

param enableConnectivityLogs bool = true

param enableMessagingLogs bool = true

param enableLiveTrace bool = true

param allowedOrigins array = [
  '*'
]

resource SignalR 'Microsoft.SignalRService/signalR@2022-02-01' = {
  name: name
  location: location
  sku: {
    capacity: 1
    name: 'Free_F1'
  }
  kind: 'SignalR'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    tls: {
      clientCertEnabled: false
    }
    features: [
      {
        flag: 'ServiceMode'
        value: serviceMode
      }
      {
        flag: 'EnableConnectivityLogs'
        value: string(enableConnectivityLogs)
      }
      {
        flag: 'EnableMessagingLogs'
        value: string(enableMessagingLogs)
      }
      {
        flag: 'EnableLiveTrace'
        value: string(enableLiveTrace)
      }
    ]
    cors: {
      allowedOrigins: allowedOrigins
    }
    publicNetworkAccess: 'Enabled'
    networkACLs: {
      defaultAction: 'Allow'
      publicNetwork: {
      }
    }
  }
}
