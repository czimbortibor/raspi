param location string = 'westeurope'

module signalRModule 'modules/signalR.bicep' = {
  name: '${deployment().name}_signalR'
  params: {
    location: location
    name: 'signalr-we'
    serviceMode: 'Default'
  }
}
