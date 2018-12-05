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
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.Servicebus
{
    /// <summary>
    /// 'Get-AzureRmServiceBusVNetRule' Cmdlet gives the details of a / List of VNetRules(s)
    /// <para> If VNetRule name provided, a single VNetRule detials will be returned</para>
    /// <para> If VNetRule name not provided, list of VNetRules will be returned</para>
    /// </summary>
    [Cmdlet("Get", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ServiceBusNetworkRuleSet", DefaultParameterSetName = NetworkRuleSetPropertiesParameterSet), OutputType(typeof(PSNetworkRuleSetAttributes))]
    public class GetAzureServiceBusNetworkRuleSet : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Resource Group Name")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetPropertiesParameterSet, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Namespace Name")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasNamespaceName)]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NetworkRuleSetRuleResourceIdParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "NetworkRuleSet Resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                if (ParameterSetName.Equals(NetworkRuleSetRuleResourceIdParameterSet))
                {
                    LocalResourceIdentifier identifier = new LocalResourceIdentifier(ResourceId);
                    ResourceGroupName = identifier.ResourceGroupName;
                    Namespace = identifier.ParentResource;
                }
                
                // Get a VNet Rule
                PSNetworkRuleSetAttributes sbNetworkRuleSet = Client.GetNetworkRuleSet(ResourceGroupName, Namespace);
                WriteObject(sbNetworkRuleSet);
                
            }
            catch (Management.ServiceBus.Models.ErrorResponseException ex)
            {
                WriteError(ServiceBus.ServiceBusClient.WriteErrorforBadrequest(ex));
            }
        }
    }
}
