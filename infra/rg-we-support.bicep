param location string = 'westeurope'

var signalRName = 'signalr-we'
module signalRModule 'modules/signalR.bicep' = {
  name: '${deployment().name}_signalR'
  params: {
    location: location
    name: signalRName
    serviceMode: 'Default'
  }
}

resource SignalRService 'Microsoft.SignalRService/signalR@2022-02-01' existing = {
  name: signalRName
}

resource ContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  name: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
}

resource FunctionApp 'Microsoft.Web/sites@2022-03-01' existing = {
  name: 'fa-we-raspiconsumer'
  scope: resourceGroup('rg-we-business')
}

resource RoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: SignalRService
  dependsOn: [signalRModule]
  name: guid(FunctionApp.name, signalRName, ContributorRoleDefinition.id)
  properties: {
    roleDefinitionId: ContributorRoleDefinition.id
    principalId: FunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
