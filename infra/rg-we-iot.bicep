param location string = 'westeurope'

@secure()
param allowedIpV4CIDR string

module iotHubModule 'modules/iotHub.bicep' = {
  name: '${deployment().name}_iotHub'
  params: {
    location: location
    name: 'raspi'
    allowedIpV4CIDR: allowedIpV4CIDR
  }
}
