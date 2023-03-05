param location string = 'westeurope'

module iotHubModule 'modules/iotHub.bicep' = {
  name: '${deployment().name}_iotHub'
  params: {
    location: location
    name: 'raspi'
  }
}
