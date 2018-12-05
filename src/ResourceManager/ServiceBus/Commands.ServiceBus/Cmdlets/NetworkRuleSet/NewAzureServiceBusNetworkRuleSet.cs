// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.ServiceBus.Models;
using System.Management.Automation;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using System;
using System.Collections;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.serviceBus
{
    /// <summary>
    /// 'New-AzureRmServiceBusVNetRule' Cmdlet creates a new IPFilterRule
    /// </summary>
    [Cmdlet("New", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ServiceBusNetworkRuleSet", DefaultParameterSetName = NetworkRuleSetPropertiesParameterSet, SupportsShouldProcess = true), OutputType(typeof(PSNetworkRuleSetAttributes))]
    public class NewAzureServiceBusNetworkRuleSet : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Resource Group Name")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
         public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Namespace Name")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasNamespaceName)]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, Position = 3, HelpMessage = "Ntwework Rule Set object")]
        [ValidateNotNullOrEmpty]
        public PSNetworkRuleSetAttributes NetworkRuleSet { get; set; }

        //[Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, Position = 3, HelpMessage = "DefaultAction")]
        //[ValidateNotNullOrEmpty]
        //[ValidateSet("Allow", "Deny", IgnoreCase = true)]
        //public string DefaultAction { get; set; }

        //[Parameter(Mandatory = false, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, HelpMessage = "Hastable as Key=Subnet ID and value=true/false for Ignore Missing Vnet Service Endpoint")]
        //[ValidateNotNullOrEmpty]
        //public Hashtable VirtualNetworkRuleList { get; set; }

        //[Parameter(Mandatory = false, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, HelpMessage = "Hastable as Key=IPMask and value=Action(Allow/Deny) ")]
        //[ValidateNotNullOrEmpty]
        //public Hashtable IpRuleList { get; set; }

        public override void ExecuteCmdlet()
        {
            PSNetworkRuleSetAttributes psNWruleset = new PSNetworkRuleSetAttributes();

           //if (IpRuleList != null && IpRuleList.Count > 0)
           //     foreach (DictionaryEntry hastable in IpRuleList)
           //     {
           //         psNWruleset.IpRulesList.Add(new PSNWRuleSetIpRulesAttributes() { IpMask = hastable.Key.ToString(), Action = hastable.Value.ToString() });
           //     }

           // if (VirtualNetworkRuleList != null && VirtualNetworkRuleList.Count > 0)
           //     foreach (DictionaryEntry hastable in VirtualNetworkRuleList)
           //     {
           //         psNWruleset.VirtualNetworkRulesList.Add(new PSNWRuleSetVirtualNetworkRulesAttributes() { Subnet = hastable.Key.ToString(), IgnoreMissingVnetServiceEndpoint = Convert.ToBoolean(hastable.Value) });
           //     }

            if (ShouldProcess(target:Namespace, action:string.Format(Resources.CreateNetworkRuleSet,Namespace)))
            {
                try
                {
                    WriteObject(Client.CreateOrUpdateNetworkRuleSet(ResourceGroupName, Namespace, NetworkRuleSet));
                }
                catch (Management.ServiceBus.Models.ErrorResponseException ex)
                {
                    WriteError(ServiceBus.ServiceBusClient.WriteErrorforBadrequest(ex));
                }
            }
                        
        }
    }
}
