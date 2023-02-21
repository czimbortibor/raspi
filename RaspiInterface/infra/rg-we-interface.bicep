param location string = resourceGroup().location

module appService 'modules/appService.bicep' = {
  name: '${deployment().name}_appService'
  params:{
    location: location
    appServiceName: 'app-we-interface'
    appServicePlanName: 'asp-we-interface'
  }
}
