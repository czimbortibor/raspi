param location string = 'westeurope'

resource LogAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: 'DefaultWorkspace-7d9dae89-eedb-4c22-9778-5659626c3853-WEU'
  scope: resourceGroup('DefaultResourceGroup-WEU')
}

var functionAppName = 'fa-we-raspiconsumer'
module functionAppModule 'modules/functionApp.bicep' = {
  name: 'functionAppAndDependencies'
  params: {
    location: location
    faName: functionAppName
    logAnalyticsWorkspaceId: LogAnalyticsWorkspace.id
    eventHubConnectionStringKVReference: '@Microsoft.KeyVault(VaultName=${KeyVault.name};SecretName=${IoTHubConnectionStringKVSecret.name})'
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
