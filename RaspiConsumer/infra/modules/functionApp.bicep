param location string

param logAnalyticsWorkspaceId string

param eventHubConnectionStringKVReference string

resource AppInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'ai-we-raspiconsumer'
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

resource StorageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'saraspiconsumer'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
  }
}

resource AppPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'asp-we-raspiconsumer'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

var faName = 'fa-we-raspiconsumer'
resource FunctionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: faName
  location: location
  tags: {
    'hidden-link: /app-insights-resource-id': AppInsights.id
  }
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned, UserAssigned'
    userAssignedIdentities: {
      '/subscriptions/7d9dae89-eedb-4c22-9778-5659626c3853/resourcegroups/rg-we-support/providers/Microsoft.ManagedIdentity/userAssignedIdentities/id-we-githubactionworkload': {
      }
    }
  }
  properties: {
    serverFarmId: AppPlan.id
    siteConfig: {
      acrUseManagedIdentityCreds: false
      functionAppScaleLimit: 10
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${StorageAccount.name}};EndpointSuffix=${environment().suffixes.storage};AccountKey=${StorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${StorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${StorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: faName
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: AppInsights.properties.ConnectionString
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'EventHubConnectionString'
          value: eventHubConnectionStringKVReference
        }
      ]
    }
    clientCertMode: 'Required'
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}

output id string = FunctionApp.id
output principalId string = FunctionApp.identity.principalId
