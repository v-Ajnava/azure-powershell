# ----------------------------------------------------------------------------------
#
# Copyright Microsoft Corporation
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ----------------------------------------------------------------------------------

<#
.SYNOPSIS
Tests New Parameter names ServiceBus Create List Remove operations.
#>

function NetworkRuleSetTests
{
	# Setup    
	$location = Get-Location
	$resourceGroupName = getAssetName "RSG"
	$namespaceName = getAssetName "EH-NS-"	

	Try
	{
		# Create Resource Group
		Write-Debug "Create resource group"
		Write-Debug " Resource Group Name : $resourceGroupName"
		New-AzureRmResourceGroup -Name $resourceGroupName -Location $location -Force	
		
		# Create ServiceBus Namespace
		Write-Debug "  Create new ServiceBus namespace"
		Write-Debug " Namespace name : $namespaceName"
		$result = New-AzureRmServiceBusNamespace -ResourceGroup $resourceGroupName -Name $namespaceName -Location $location -SkuName "Premium"

		# Assert
		Assert-AreEqual $result.Name $namespaceName	"New Namespace: Namespace created earlier is not found."

		# get the created ServiceBus Namespace
		Write-Debug " Get the created namespace within the resource group"
		$createdNamespace = Get-AzureRmServiceBusNamespace -ResourceGroup $resourceGroupName -Name $namespaceName
	
		Assert-AreEqual $createdNamespace.Name $namespaceName "Get Namespace: Namespace created earlier is not found."
		
		
		# Create a VNetRule
		Write-Debug " Create new VNetRule "
		$psnwiprule = New-Object Microsoft.Azure.Commands.ServiceBus.Models.PSNWRuleSetIpRulesAttributes
		$psnwiprule.Action = "Allow"
		$psnwiprule.IpMask ="1.1.1.1"
		$psnwiprule1 = New-Object Microsoft.Azure.Commands.ServiceBus.Models.PSNWRuleSetIpRulesAttributes
		$psnwiprule1.Action = "Allow"
		$psnwiprule1.IpMask ="1.1.1.2"
		$psnwiprule2 = New-Object Microsoft.Azure.Commands.ServiceBus.Models.PSNWRuleSetIpRulesAttributes
		$psnwiprule2.Action = "Allow"
		$psnwiprule2.IpMask ="1.1.1.3"
		$listIPrule = @($psnwiprule, $psnwiprule1, $psnwiprule2)
		
		$psnwvnetrule = New-Object Microsoft.Azure.Commands.ServiceBus.Models.PSNWRuleSetVirtualNetworkRulesAttributes
		$psnwvnetrule.IgnoreMissingVnetServiceEndpoint = $True
		$psnwvnetrule.Subnet = "/subscriptions/854d368f-1828-428f-8f3c-f2affa9b2f7d/resourceGroups/alitest/providers/Microsoft.Network/virtualNetworks/myvn/subnets/subnet2"
		$psnwvnetrule1 = New-Object Microsoft.Azure.Commands.ServiceBus.Models.PSNWRuleSetVirtualNetworkRulesAttributes
		$psnwvnetrule1.Subnet = "/subscriptions/854d368f-1828-428f-8f3c-f2affa9b2f7d/resourceGroups/alitest/providers/Microsoft.Network/virtualNetworks/myvn/subnets/subnet3"
		$listvnetrules = @($psnwvnetrule, $psnwvnetrule1)

		$psnwruleset = New-Object Microsoft.Azure.Commands.ServiceBus.Models.PSNetworkRuleSetAttributes
		$psnwruleset.DefaultAction = "Allow"
		$psnwruleset.IpRulesList = $listIPrule
		$psnwruleset.VirtualNetworkRulesList = $listvnetrules
		
		$result = New-AzureRmServiceBusNetworkRuleSet -ResourceGroup $resourceGroupName -Namespace $namespaceName -NetworkRuleSet $psnwruleset
		
		Write-Debug " Get the created VNetRule "
		$createdVNetRule = Get-AzureRmServiceBusNetworkRuleSet -ResourceGroup $resourceGroupName -Namespace $namespaceName 

	}
	Finally
	{
		# Cleanup
		
		Write-Debug " Delete namespaces"
		Remove-AzureRmServiceBusNamespace -ResourceGroup $resourceGroupName -Namespace $namespaceName

		Write-Debug " Delete resourcegroup"
		Remove-AzureRmResourceGroup -Name $resourceGroupName
	}	
}