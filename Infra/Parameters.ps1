#---------- Octopus variables----Begin-------------

$resourceGroupName = '#{resourceGroupName}';
$location = '#{location}';

$vnetName = '#{vnetName}';
$vnetAddressPrefix = '#{vnetAddressPrefix}';
$vnetDefaultSubnetName = '#{vnetDefaultSubnetName}';
$vnetDefaultSubnetAddressPrefix = '#{vnetDefaultSubnetAddressPrefix}';

# $keyvaultName = '#{keyvaultName}';
# $keyvaultDefaultAction = '#{keyvaultDefaultAction}';
# $keyvaultByPass = '#{keyvaultByPass}';
# $keyvaultSKU = '#{keyvaultSKU}';

$storageAccountSKU = '#{storageAccountSKU}';
$storageAccountKind = '#{storageAccountKind}';
$storageAccountAccessTier = '#{storageAccountAccessTier}';
$storageAccountByPass = '#{storageAccountByPass}';
$storageAccountDefaultAction = '#{storageAccountDefaultAction}';
$storageAccountSubnetName = '#{storageAccountSubnetName}';
$storageAccountSubnetAddessPrefix = '#{storageAccountSubnetAddessPrefix}';
$storageAccountSubnetServiceEndpoints = ('#{storageAccountSubnetServiceEndpoints}').Split(';', [System.StringSplitOptions]::RemoveEmptyEntries);


$projectSubnetName = '#{projectSubnetName}';
$projectSubnetAddessPrefix = '#{projectSubnetAddessPrefix}';
$projectSubnetDelegations = '#{projectSubnetDelegations}';
$projectSubnetServiceEndpoints = ('#{projectSubnetServiceEndpoints}').Split(';', [System.StringSplitOptions]::RemoveEmptyEntries);

$azureFunctionsAppPlanName = '#{azureFunctionsAppPlanName}';
$azureFunctionsAppPlanSKU = '#{azureFunctionsAppPlanSKU}';
$azureFunctionsOSType = '#{azureFunctionsOSType}';
$azureFunctionsRuntime = '#{azureFunctionsRuntime}';
$azureFunctionsVersion = '#{azureFunctionsVersion}';
$azureFunctionsDisableAppInsights = '#{azureFunctionsDisableAppInsights}';
# $azureKeyVaultCertPermissions = ('#{azureKeyVaultCertPermissions}').Split(';', [System.StringSplitOptions]::RemoveEmptyEntries);
# $azureKeyVaultKeyPermissions = ('#{azureKeyVaultKeyPermissions}').Split(';', [System.StringSplitOptions]::RemoveEmptyEntries);
# $azureKeyVaultSecretPermissions = ('#{azureKeyVaultSecretPermissions}').Split(';', [System.StringSplitOptions]::RemoveEmptyEntries);

$sqlServerName = '#{sqlServerName}';
$sqlServerUserName = '#{sqlServerUserName}';
$sqlServerUserPwd = '#{sqlServerUserPwd}';

$sqlDatabaseName = '#{sqlDatabaseName}';
$sqlDatabaseEdition = '#{sqlDatabaseEdition}';
$allowAzureServicesInSQL = '#{allowAzureServicesInSQL}';

$azureWebAppPlanName = '#{azureWebAppPlanName}';
$azureWebAppPlanSKU = '#{azureWebAppPlanSKU}';

$core3Extension = '#{core3Extension}';
$azureRestApiVersion = '#{azureRestApiVersion}';

$apimName = '#{apimName}';
$apimOrganization = '#{apimOrganization}';
$apimAdminEmail = '#{apimAdminEmail}';
$apimSKU = '#{apimSKU}';

$enableAppInsight='#{enableAppInsights}';
$appInsightName = '#{appInsightName}';

# App Gateway
$appGatewayNsgName='#{appGatewayNsgName}';
$appGatewaySubnetName='#{appGatewaySubnetName}';
$appGatewaySubnetAddressPrefix='#{appGatewaySubnetAddressPrefix}';
$appGatewayPublicIpName='#{appGatewayPublicIpName}';
$appGatewayPublicIpAssignment='#{appGatewayPublicIpAssignment}';
$appGatewayName='#{appGatewayName}';
$appGatewayPublicIpSKU='#{appGatewayPublicIpSKU}';
$appGatewayNsgPortRuleName='#{appGatewayNsgPortRuleName}';
$appGatewayNsgPortRulePriority='#{appGatewayNsgPortRulePriority}';
$appGatewayNsgPortRuleDestinationPortRange='#{appGatewayNsgPortRuleDestinationPortRange}';
$appGatewaySKU='#{appGatewaySKU}';