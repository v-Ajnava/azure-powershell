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
using System.Collections;
using System.Management.Automation;
using System.Collections.Generic;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.Queue
{
    /// <summary>
    /// 'Get-AzureRmServiceBusQueue' Cmdlet gives the details of a / List of ServiceBus Queue(s)
    /// <para> If Queue name provided, a single Queue detials will be returned</para>
    /// <para> If Queue name not provided, list of Queue will be returned</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, ServicebusQueueVerb, DefaultParameterSetName = QueueParameterSet), OutputType(typeof(PSQueueAttributes))]
    public class GetAzureRmServiceBusQueue : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = QueueParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "The name of the resource group")]
        [ResourceGroupCompleter]
        [Alias("ResourceGroup")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = QueueParameterSet, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Namespace Name")]
        [Alias(AliasNamespaceName)]
        [ValidateNotNullOrEmpty]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NamespaceInputObjectParameterSet, ValueFromPipeline = true, Position = 0, HelpMessage = "Namespace Object")]
        [ValidateNotNullOrEmpty]
        public PSNamespaceAttributes InputObject { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NamespaceResourceIdParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Namespace Resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Queue Name")]
        [Alias(AliasQueueName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            ResourceIdentifier getResourceIdParam = new ResourceIdentifier();

            if (ParameterSetName.Equals(NamespaceResourceIdParameterSet))
            {
                getResourceIdParam = GetResourceDetailsFromId(ResourceId);

                if (!string.IsNullOrEmpty(Name))
                {
                    var queueAttributes = Client.GetQueue(getResourceIdParam.ResourceGroupName, getResourceIdParam.ResourceName, Name);
                    WriteObject(queueAttributes);
                }
                else
                {
                    IEnumerable<PSQueueAttributes> queueAttributes = Client.ListQueues(getResourceIdParam.ResourceGroupName, getResourceIdParam.ResourceName);
                    WriteObject(queueAttributes, true);
                }
            }
            else if (ParameterSetName.Equals(NamespaceInputObjectParameterSet))
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    var queueAttributes = Client.GetQueue(InputObject.ResourceGroup, InputObject.Name, Name);
                    WriteObject(queueAttributes);
                }
                else
                {
                    IEnumerable<PSQueueAttributes> queueAttributes = Client.ListQueues(InputObject.ResourceGroup, InputObject.Name);
                    WriteObject(queueAttributes, true);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    var queueAttributes = Client.GetQueue(ResourceGroupName, Namespace, Name);
                    WriteObject(queueAttributes);
                }
                else
                {
                    IEnumerable<PSQueueAttributes> queueAttributes = Client.ListQueues(ResourceGroupName, Namespace);
                    WriteObject(queueAttributes, true);
                }
            }
        }
    }
}
