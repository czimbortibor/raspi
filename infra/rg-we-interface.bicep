param location string = resourceGroup().location

resource LogAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: 'DefaultWorkspace-7d9dae89-eedb-4c22-9778-5659626c3853-WEU'
  scope: resourceGroup('DefaultResourceGroup-WEU')
}

module appService 'modules/appService.bicep' = {
  name: '${deployment().name}_appService'
  params:{
    location: location
    appServiceName: 'app-we-interface'
    appServicePlanName: 'asp-we-interface'
    appInsightsName: 'ai-we-interface'
    logAnalyticsWorkspaceId: LogAnalyticsWorkspace.id
  }
}
