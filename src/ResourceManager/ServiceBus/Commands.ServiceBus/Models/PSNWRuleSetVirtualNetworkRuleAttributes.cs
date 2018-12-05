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
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus.Models;

namespace Microsoft.Azure.Commands.ServiceBus.Models
{
    public class PSNWRuleSetVirtualNetworkRuleAttributes
    {
        public PSNWRuleSetVirtualNetworkRuleAttributes()
        {
           
        }

        public PSNWRuleSetVirtualNetworkRuleAttributes(NWRuleSetVirtualNetworkRules virtualNetworkRules)
        {
            Subnet = virtualNetworkRules.Subnet.Id;
            IgnoreMissingVnetServiceEndpoint = virtualNetworkRules.IgnoreMissingVnetServiceEndpoint;
        }

        /// <summary>
        /// Gets or sets subnet properties
        /// </summary>
        public string Subnet { get; set; }

        /// <summary>
        /// Gets or sets value that indicates whether to ignore missing Vnet
        /// Service Endpoint
        /// </summary>
        public bool? IgnoreMissingVnetServiceEndpoint { get; set; }
    }
}
