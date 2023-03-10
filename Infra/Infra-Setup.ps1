. '.\Parameters.ps1';
. '.\DevOps\Shared\APIM.ps1';
. '.\DevOps\Shared\AppGateway.ps1';
. '.\DevOps\Shared\AppInsights.ps1';
. '.\DevOps\Shared\AppServicePlan.ps1';
. '.\DevOps\Shared\Functions.ps1';
. '.\DevOps\Shared\KeyVault.ps1';
. '.\DevOps\Shared\ResourceGroup.ps1';
#. '.\DevOps\Shared\ServiceBus.ps1';
. '.\DevOps\Shared\SQLServer.ps1';
. '.\DevOps\Shared\StorageAccount.ps1';
. '.\DevOps\Shared\Utilities.ps1';
. '.\DevOps\Shared\vnet.ps1';
. '.\DevOps\Shared\WebApp.ps1';

$ErrorActionPreference = "Stop";

try
{
    Create-ResourceGroup -name $resourceGroupName
    
    Create-vnet
    # Special fix for Azure issue
    Create-SubnetforStorage
    #--------------------------
    Create-SubnetforProject

    Create-APIM
    
    Create-AppGateway

    #Setting App insights
    Add-AppInsight -enableAppInsight $enableAppInsight -resourceGroupName $resourceGroupName -appName $appInsightName -location $location

    # Create-KeyVault

    Create-SqlServer
    Create-SqlDabase -name $sqlDatabaseName -databaseEdition $sqlDatabaseEdition
    
    Create-FunctionsAppServicePlan -functionsAppPlanName $azureFunctionsAppPlanName -functionsAppPlanSKU $azureFunctionsAppPlanSKU
    # Plan for Web Apps
    Create-WebAppServicePlan -webAppPlanName $azureWebAppPlanName -webAppPlanSKU $azureWebAppPlanSKU
    

    Create-WebApp -name '#{quizWebAppName}' -webAppPlanName $azureWebAppPlanName -webAppDeploymentSlotName '#{quizWebAppDeploymentSlotName}'
    Create-WebApp -name '#{quizAdminWebAppName}' -webAppPlanName $azureWebAppPlanName -webAppDeploymentSlotName '#{quizAdminWebAppDeploymentSlotName}'
    
    Create-StorageAccount -name '#{QuizStorageAccountName}'
    Create-FunctionApp -name '#{QuizFunctionName}' -storageAccountName '#{QuizStorageAccountName}'

    Create-StorageAccount -name '#{UsersStorageAccountName}'
    Create-FunctionApp -name '#{UsersFunctionName}' -storageAccountName '#{UsersStorageAccountName}'
    
    Create-StorageAccount -name '#{ReportingStorageAccountName}'
    Create-FunctionApp -name '#{ReportingFunctionName}' -storageAccountName '#{ReportingStorageAccountName}'
    
    Create-StorageAccount -name '#{QuizAdminStorageAccountName}'
    Create-FunctionApp -name '#{QuizAdminFunctionName}' -storageAccountName '#{QuizAdminStorageAccountName}'
}
catch
{
    Write-Host $Error[0] -ForegroundColor Red
    exit 1
}






