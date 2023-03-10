[CmdletBinding(DefaultParameterSetName = 'None')]
param
(
  [String] [Parameter(Mandatory = $true)] $ServerName,
  [String] [Parameter(Mandatory = $true)] $ResourceGroup,
  [String] $AzureFirewallName = "SwiftCapsAzDOAgentFirewall"
)
Remove-AzSqlServerFirewallRule -ServerName $ServerName -FirewallRuleName $AzureFirewallName -ResourceGroupName $ResourceGroup