param appServiceName string

param appServicePlanName string

param appInsightsName string

param sku string = 'F1'

param location string

param logAnalyticsWorkspaceId string

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
  tags: {
    'hidden-link: /app-insights-resource-id': AppInsights.id
  }
  properties: {
    serverFarmId: AppServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      http20Enabled: true
      webSocketsEnabled: true
      ftpsState: 'Disabled'
      netFrameworkVersion: 'v7.0'
      appSettings: [
        
      ]
    }
  }
}

resource AppInsights 'microsoft.insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    RetentionInDays: 30
    WorkspaceResourceId: logAnalyticsWorkspaceId
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}
