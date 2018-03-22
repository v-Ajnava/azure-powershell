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

using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Commands.ServiceBus.Models;
using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;
using System.Management.Automation;
namespace Microsoft.Azure.Commands.ServiceBus.Commands.Topic
{
    /// <summary>
    /// 'Remove-AzureRmServiceBusTopic' Cmdlet removes the specified ServiceBus Topic
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, ServicebusTopicVerb, DefaultParameterSetName = TopicParameterSet, SupportsShouldProcess = true)]
    public class RemoveAzureRmServiceBusTopic : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = TopicParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "The name of the resource group")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        [Alias(AliasResourceGroup)]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = TopicParameterSet, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Namespace Name")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasNamespaceName)]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = TopicParameterSet, ValueFromPipelineByPropertyName = true, Position = 2, HelpMessage = "Topic Name")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasTopicName)]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = TopicInputObjectParameterSet, ValueFromPipeline = true, Position = 0, HelpMessage = "Topic Object")]
        [ValidateNotNullOrEmpty]
        public PSTopicAttributes InputObject { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = TopicResourceIdParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Topic Resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        public override void ExecuteCmdlet()
        {
            // delete a Topic
            if (ParameterSetName.Equals(TopicResourceIdParameterSet))
            {
                ResourceIdentifier getParamGeoDR = GetResourceDetailsFromId(ResourceId);

                if (ShouldProcess(target: Name, action: string.Format(Resources.RemoveTopic, getParamGeoDR.ResourceName, getParamGeoDR.ParentResource)))
                {
                    WriteObject(Client.DeleteTopic(getParamGeoDR.ResourceGroupName, getParamGeoDR.ParentResource, getParamGeoDR.ResourceName));
                }
            }
            else if (ParameterSetName.Equals(TopicInputObjectParameterSet))
            {
                ResourceIdentifier getParamGeoDR = GetResourceDetailsFromId(InputObject.Id);

                if (ShouldProcess(target: Name, action: string.Format(Resources.RemoveTopic, getParamGeoDR.ResourceName, getParamGeoDR.ParentResource)))
                {
                    WriteObject(Client.DeleteTopic(getParamGeoDR.ResourceGroupName, getParamGeoDR.ParentResource, getParamGeoDR.ResourceName));
                }
            }
            else
            {
                if (ShouldProcess(target: Name, action: string.Format(Resources.RemoveTopic, Name, Namespace)))
                {
                    WriteObject(Client.DeleteTopic(ResourceGroupName, Namespace, Name));
                }
            }
        }
    }
}
