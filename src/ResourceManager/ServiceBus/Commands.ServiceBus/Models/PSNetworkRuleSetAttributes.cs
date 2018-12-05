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
    /// <summary>
    /// Description of topic resource.
    /// </summary>
    public class PSNetworkRuleSetAttributes
    {

        public PSNetworkRuleSetAttributes()
        {
           
        }

        public PSNetworkRuleSetAttributes(NetworkRuleSet nwRuleSet)
        {            
            Id = nwRuleSet.Id;
            Name = nwRuleSet.Name;
            Type = nwRuleSet.Type;

            if (nwRuleSet.VirtualNetworkRules.Count > 0)
            {
                VirtualNetworkRulesList = new PSNWRuleSetVirtualNetworkRuleAttributes[nwRuleSet.VirtualNetworkRules.Count];
                int itemcount = 0;
                foreach (NWRuleSetVirtualNetworkRules VnetRule in nwRuleSet.VirtualNetworkRules)
                {                    
                    VirtualNetworkRulesList[itemcount] = new PSNWRuleSetVirtualNetworkRuleAttributes() { IgnoreMissingVnetServiceEndpoint = VnetRule.IgnoreMissingVnetServiceEndpoint, Subnet = VnetRule.Subnet.Id };
                    itemcount += 1;
                }
            }

            if (nwRuleSet.IpRules.Count > 0)
            {
                IpRulesList = new PSNWRuleSetIpRuleAttributes[nwRuleSet.IpRules.Count];
                int itemcount = 0;

                foreach (NWRuleSetIpRules IpRule in nwRuleSet.IpRules)
                {
                    IpRulesList[itemcount] = new PSNWRuleSetIpRuleAttributes() { Action = IpRule.Action, IpMask = IpRule.IpMask};
                    itemcount += 1;
                }
            }
        }    


        public string Id { get; set; }

        /// <summary>
        /// Gets or sets IP Filter name
        /// </summary>
        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Gets or sets default Action for Network Rule Set. Possible values
        /// include: 'Allow', 'Deny'
        /// </summary>
        public string DefaultAction { get; set; }

        /// <summary>
        /// Gets or sets list VirtualNetwork Rules
        /// </summary>
        public PSNWRuleSetVirtualNetworkRuleAttributes[] VirtualNetworkRulesList { get; set; }

        /// <summary>
        /// Gets or sets list of IpRules
        /// </summary>
        public PSNWRuleSetIpRuleAttributes[] IpRulesList { get; set; }

    }
}
