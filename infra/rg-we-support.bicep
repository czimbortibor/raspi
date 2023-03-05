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

resource SignalRRestApiOwnerRole 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  name: 'fd53cd77-2268-407a-8f46-7e7863d0f521'
}

resource FunctionApp 'Microsoft.Web/sites@2022-03-01' existing = {
  name: 'fa-we-raspiconsumer'
  scope: resourceGroup('rg-we-business')
}

resource SignalRRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: SignalRService
  dependsOn: [signalRModule]
  name: guid(FunctionApp.name, signalRName, SignalRRestApiOwnerRole.id)
  properties: {
    roleDefinitionId: SignalRRestApiOwnerRole.id
    principalId: FunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
