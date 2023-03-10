[CmdletBinding(DefaultParameterSetName = 'None')]
param
(
  [String] [Parameter(Mandatory = $true)] $ServerName,
  [String] [Parameter(Mandatory = $true)] $ResourceGroup,
  [String] $AzureFirewallName = "SwiftCapsAzDOAgentFirewall"
)
$agentIP = (New-Object net.webclient).downloadstring("https://api.ipify.org")
New-AzSqlServerFirewallRule -ResourceGroupName $ResourceGroup -ServerName $ServerName -FirewallRuleName $AzureFirewallName -StartIPAddress $agentIp -EndIPAddress $agentIP