param appServiceName string

param appServicePlanName string

param sku string = 'F1'

param location string

resource AppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  properties: {
    isSpot: false
    zoneRedundant: false
  }
  sku: {
    name: sku
  }
}

resource AppService 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: AppServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      http20Enabled: true
      webSocketsEnabled: true
      appSettings: [
        
      ]
    }
  }
}
