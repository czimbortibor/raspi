param location string = 'westeurope'

resource LogAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: 'DefaultWorkspace-7d9dae89-eedb-4c22-9778-5659626c3853-WEU'
  scope: resourceGroup('DefaultResourceGroup-WEU')
}

var iotHubConnectionStringKvRef = '@Microsoft.KeyVault(VaultName=${KeyVault.name};SecretName=${IoTHubConnectionStringKVSecret.name})'
var signalRConnectionString = 'Endpoint=https://${SignalRService.properties.hostName};AuthType=azure.msi;Version=1.0;'
var cosmosDbConnectionString = '@Microsoft.KeyVault(VaultName=${KeyVault.name};SecretName=${CosmosDbAccountConnectiongStringKVSecret.name})'
var functionAppName = 'fa-we-raspiconsumer'
module functionAppModule 'modules/functionApp.bicep' = {
  name: '${deployment().name}_functionApp'
  params: {
    location: location
    faName: functionAppName
    logAnalyticsWorkspaceId: LogAnalyticsWorkspace.id
    iotHubConnectionString: iotHubConnectionStringKvRef
    iotHubName: IoTHub.properties.eventHubEndpoints.events.path
    // TODO: managed identity
    signalRConnectionString: signalRConnectionString
    // TODO: managed identity
    cosmosDbConnectionString: cosmosDbConnectionString
  }
}

resource KeyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: 'kv-we-raspiconsumer'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: '7491022c-1291-41b3-bf41-90c28b021b49'
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 10
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
  }
}

resource IoTHub 'Microsoft.Devices/IotHubs@2021-07-02' existing = {
  name: 'raspi'
  scope: resourceGroup('rg-we-iot')
}

var iotHubBuiltinEndpointConnString = 'Endpoint=${IoTHub.properties.eventHubEndpoints.events.endpoint};SharedAccessKeyName=${listKeys(IoTHub.id, '2020-04-01').value[1].keyName};SharedAccessKey=${listKeys(IoTHub.id, '2020-04-01').value[1].primaryKey}'
resource IoTHubConnectionStringKVSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: KeyVault
  name: 'conn-raspi-iothub'
  properties: {
    value: iotHubBuiltinEndpointConnString
  }
}

resource CosmosDbAccountConnectiongStringKVSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: KeyVault
  name: 'conn-raspi-cosmos'
  properties: {
    value: CosmosDbAcc.listConnectionStrings().connectionStrings[0].connectionString
  }
}

resource KVSecretsUserRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}

resource KVSecretsUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: KeyVault
  name: guid(functionAppName, KeyVault.id, KVSecretsUserRoleDefinition.id)
  properties: {
    roleDefinitionId: KVSecretsUserRoleDefinition.id
    principalId: functionAppModule.outputs.principalId
    principalType: 'ServicePrincipal'
  }
}

resource SignalRService 'Microsoft.SignalRService/signalR@2022-02-01' existing = {
  name: 'signalr-we'
  scope: resourceGroup('rg-we-support')
}

// TODO: define in iac
resource CosmosDbAcc 'Microsoft.DocumentDB/databaseAccounts@2022-11-15' existing = {
  name: 'cosmos-raspi'
  scope: resourceGroup('rg-we-storage')
}
