﻿// ----------------------------------------------------------------------------------
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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Models;
using Microsoft.Azure.Commands.EventHub.Models;
using Microsoft.Azure.Management.EventHub;
using Microsoft.Azure.Management.EventHub.Models;

namespace Microsoft.Azure.Commands.EventHub.Utilities
{
    public class EventHubsClient
    {
        public Action<string> VerboseLogger { get; set; }

        public Action<string> ErrorLogger { get; set; }

        public Action<string> WarningLogger { get; set; }

        public EventHubsClient(AzureContext context)
        {
            this.Client = AzureSession.ClientFactory.CreateArmClient<EventHubManagementClient>(context, AzureEnvironment.Endpoint.ResourceManager);

        }
        public EventHubManagementClient Client
        {
            get;
            private set;
        }

        #region Namespace
        public NamespaceAttributes GetNamespace(string resourceGroupName, string namespaceName)
        {
            var response = Client.Namespaces.Get(resourceGroupName, namespaceName);
            return new NamespaceAttributes(response);
        }

        public IEnumerable<NamespaceAttributes> ListNamespacesByResourceGroup(string resourceGroupName)
        {
            var response = Client.Namespaces.ListByResourceGroup(resourceGroupName);
            var resourceList = response.Select(resource => new NamespaceAttributes(resource));
            return resourceList;
        }

        public IEnumerable<NamespaceAttributes> ListNamespacesBySubscription()
        {
            var response = Client.Namespaces.ListBySubscription();
            var resourceList = response.Select(resource => new NamespaceAttributes(resource));
            return resourceList;
        }

        public NamespaceAttributes BeginCreateNamespace(string resourceGroupName, string namespaceName, string location, string skuName, int? skuCapacity, bool? createACSNamespace, Dictionary<string, string> tags)
        {
            var parameter = new NamespaceCreateOrUpdateParameters()
            {
                Location = location
            };

            if (tags != null)
            {
                parameter.Tags = new Dictionary<string, string>(tags);
            }

            if (skuName != null)
            {
                parameter.Sku.Name = skuName;
                parameter.Sku.Tier = skuName;
            }

            if (skuCapacity.HasValue)
            {
                parameter.Sku.Capacity = skuCapacity;
            }

            if (createACSNamespace.HasValue)
            {
                parameter.CreateACSNamespace = createACSNamespace;
            }

            var response = Client.Namespaces.CreateOrUpdate(resourceGroupName, namespaceName, parameter);
            return new NamespaceAttributes(response);
        }

        public NamespaceAttributes UpdateNamespace(string resourceGroupName, string namespaceName, string location, string skuName, int? skuCapacity, Management.EventHub.Models.NamespaceState state, Dictionary<string, string> tags)
        {

            var parameter = new NamespaceCreateOrUpdateParameters()
            {
                Location = location,
                Status = ((state == Management.EventHub.Models.NamespaceState.Disabled) ? state : Management.EventHub.Models.NamespaceState.Active)
            };

            if (skuName != null)
            {
                parameter.Sku.Name = skuName;
                parameter.Sku.Tier = skuName;
            }

            if (skuCapacity.HasValue)
            {
                parameter.Sku.Capacity = skuCapacity;
            }

            if (tags != null && tags.Count() > 0)
            {
                parameter.Tags = new Dictionary<string, string>(tags);
            }

            var response = Client.Namespaces.CreateOrUpdate(resourceGroupName, namespaceName, parameter);
            return new NamespaceAttributes(response);
        }

        public void BeginDeleteNamespace(string resourceGroupName, string namespaceName)
        {
            Client.Namespaces.Delete(resourceGroupName, namespaceName);
        }

        public SharedAccessAuthorizationRuleAttributes GetNamespaceAuthorizationRule(string resourceGroupName, string namespaceName, string authRuleName)
        {
            var response = Client.Namespaces.GetAuthorizationRule(resourceGroupName, namespaceName, authRuleName);
            return new SharedAccessAuthorizationRuleAttributes(response);
        }

        public IEnumerable<SharedAccessAuthorizationRuleAttributes> ListNamespaceAuthorizationRules(string resourceGroupName, string namespaceName)
        {
            var response = Client.Namespaces.ListAuthorizationRules(resourceGroupName, namespaceName);
            var resourceList = response.Select(resource => new SharedAccessAuthorizationRuleAttributes(resource));
            return resourceList;
        }

        public SharedAccessAuthorizationRuleAttributes CreateOrUpdateNamespaceAuthorizationRules(string resourceGroupName, string namespaceName, string authorizationRuleName, SharedAccessAuthorizationRuleAttributes parameter)
        {
            var parameter1 = new SharedAccessAuthorizationRuleCreateOrUpdateParameters()
            {
                Name = parameter.Name,
                Rights = parameter.Rights.ToList()
            };
            var response = Client.Namespaces.CreateOrUpdateAuthorizationRule(resourceGroupName, namespaceName, authorizationRuleName, parameter1);
            return new SharedAccessAuthorizationRuleAttributes(response);
        }

        public bool DeleteNamespaceAuthorizationRules(string resourceGroupName, string namespaceName, string authRuleName)
        {
            if (string.Equals(SharedAccessAuthorizationRuleAttributes.DefaultNamespaceAuthorizationRule, authRuleName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            Client.Namespaces.DeleteAuthorizationRule(resourceGroupName, namespaceName, authRuleName);
            return true;
        }

        public ResourceListKeys GetNamespaceListKeys(string resourceGroupName, string namespaceName, string authRuleName)
        {
            var listKeys = Client.Namespaces.ListKeys(resourceGroupName, namespaceName, authRuleName);
            return listKeys;
        }

        #endregion

        #region EventHub
        public EventHubResource GetEventHub(string resourceGroupName, string namespaceName, string eventHubName)
        {
            var response = Client.EventHubs.Get(resourceGroupName, namespaceName, eventHubName);
            return response;
        }

        public IEnumerable<EventHubAttributes> ListAllEventHubs(string resourceGroupName, string namespaceName)
        {
            var response = Client.EventHubs.ListAll(resourceGroupName, namespaceName);
            var resourceList = response.Select(resource => new EventHubAttributes(resource));
            return resourceList;
        }

        public EventHubAttributes CreateOrUpdateEventHub(string resourceGroupName, string namespaceName, string eventHubName, EventHubAttributes parameter)
        {
            var Parameter1 = new EventHubCreateOrUpdateParameters()
            {
                Name = parameter.Name,
                Location = parameter.Location,
                Properties = new EventHubProperties()
                {
                    CreatedAt = parameter.CreatedAt,
                    MessageRetentionInDays = parameter.MessageRetentionInDays,
                    PartitionCount = parameter.PartitionCount,
                    PartitionIds = parameter.PartitionIds.ToList(),
                    Status = parameter.Status,
                    UpdatedAt = parameter.UpdatedAt
                }

            };
            var response = Client.EventHubs.CreateOrUpdate(resourceGroupName, namespaceName, eventHubName, Parameter1);
            return new EventHubAttributes(response);
        }

        public bool DeleteEventHub(string resourceGroupName, string namespaceName, string eventHubName)
        {
            Client.EventHubs.Delete(resourceGroupName, namespaceName, eventHubName);
            return true;
        }

        public SharedAccessAuthorizationRuleAttributes GetEventHubAuthorizationRules(string resourceGroupName, string namespaceName, string eventHubName, string authRuleName)
        {
            var response = Client.EventHubs.GetAuthorizationRule(resourceGroupName, namespaceName, eventHubName, authRuleName);
            return new SharedAccessAuthorizationRuleAttributes(response);
        }

        public IEnumerable<SharedAccessAuthorizationRuleAttributes> ListEventHubAuthorizationRules(string resourceGroupName, string namespaceName, string eventHubName)
        {
            var response = Client.EventHubs.ListAuthorizationRules(resourceGroupName, namespaceName, eventHubName);
            var resourceList = response.Select(resource => new SharedAccessAuthorizationRuleAttributes(resource));
            return resourceList;
        }
        
        public SharedAccessAuthorizationRuleAttributes CreateOrUpdateEventHubAuthorizationRules(string resourceGroupName, string namespaceName, string eventHubName, string authorizationRuleName, SharedAccessAuthorizationRuleAttributes parameters)
        {
            var parameter1 = new SharedAccessAuthorizationRuleCreateOrUpdateParameters()
            {
                Name = parameters.Name,
                Rights = parameters.Rights.ToList()
            };         
            
            var response = Client.EventHubs.CreateOrUpdateAuthorizationRule(resourceGroupName, namespaceName, eventHubName, authorizationRuleName, parameter1);
            return new SharedAccessAuthorizationRuleAttributes(response);
        }

        public bool DeleteEventHubAuthorizationRules(string resourceGroupName, string namespaceName, string eventHubName, string authRuleName)
        {
            if (string.Equals(SharedAccessAuthorizationRuleAttributes.DefaultNamespaceAuthorizationRule, authRuleName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            Client.EventHubs.DeleteAuthorizationRule(resourceGroupName, namespaceName, eventHubName, authRuleName);
            return true;
        }

        public ResourceListKeys GetEventHubListKeys(string resourceGroupName, string namespaceName, string eventHubName, string authRuleName)
        {
            var listKeys = Client.EventHubs.ListKeys(resourceGroupName, namespaceName, eventHubName, authRuleName);
            return listKeys;
        }

        #endregion

        #region ConsumerGroup
        public ConsumerGroupAttributes CreateOrUpdateConsumerGroup(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName,  ConsumerGroupAttributes parameter)
        {
            var Parameter1 = new ConsumerGroupCreateOrUpdateParameters()
            {
                Name = parameter.Name,
                Location = parameter.Location,
                CreatedAt = parameter.CreatedAt,
                UpdatedAt = parameter.UpdatedAt,
                EventHubPath = parameter.EventHubPath,
                UserMetadata = parameter.UserMetadata
            };
            var response = Client.ConsumerGroups.CreateOrUpdate(resourceGroupName, namespaceName, eventHubName, Parameter1.Name, Parameter1);
            return new ConsumerGroupAttributes(response);
        }

        public ConsumerGroupAttributes GetConsumerGroup(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName)
        {
            var response = Client.ConsumerGroups.Get(resourceGroupName, namespaceName, eventHubName, consumerGroupName);
            return new ConsumerGroupAttributes(response);
        }

        public IEnumerable<ConsumerGroupAttributes> ListAllConsumerGroup(string resourceGroupName, string namespaceName, string eventHubName)
        {
            var response = Client.ConsumerGroups.ListAll(resourceGroupName, namespaceName, eventHubName);
            var resourceList = response.Select(resource => new ConsumerGroupAttributes(resource));
            return resourceList;
        }

        public void DeletConsumerGroup(string resourceGroupName, string namespaceName, string eventHubName, string consumerGroupName)
        {
            Client.ConsumerGroups.Delete(resourceGroupName, namespaceName, eventHubName, consumerGroupName);
        }


        #endregion ConsumerGroup
    }
}
