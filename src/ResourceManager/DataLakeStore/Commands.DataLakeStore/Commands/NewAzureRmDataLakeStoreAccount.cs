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

using Microsoft.Azure.Commands.DataLakeStore.Models;
using Microsoft.Azure.Commands.DataLakeStore.Properties;
using Microsoft.Azure.Management.DataLake.Store.Models;
using Microsoft.Rest.Azure;
using System.Collections;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.DataLakeStore
{
    [Cmdlet(VerbsCommon.New, "AzureRmDataLakeStoreAccount"), OutputType(typeof(DataLakeStoreAccount))]
    [Alias("New-AdlStore")]
    public class NewAzureDataLakeStoreAccount : DataLakeStoreCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 0, Mandatory = true,
            HelpMessage = "Name of resource group under which you want to create the account.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 1, Mandatory = true,
            HelpMessage = "Name of the account to create.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 2, Mandatory = true,
            HelpMessage = "Azure region where the account should be created.")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("North Central US", "South Central US", "Central US", "West Europe", "North Europe", "West US",
            "East US",
            "East US 2", "Japan East", "Japan West", "Brazil South", "Southeast Asia", "East Asia", "Australia East",
            "Australia Southeast", IgnoreCase = true)]
        public string Location { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 3, Mandatory = false,
            HelpMessage =
                "Name of the default group to give permissions to for freshly created files and folders in the DataLakeStore."
            )]
        [ValidateNotNullOrEmpty]
        public string DefaultGroup { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 4, Mandatory = false,
            HelpMessage = "A string,string dictionary of tags associated with this account")]
        [ValidateNotNull]
        public Hashtable Tags { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 5, Mandatory = false,
            HelpMessage = "Indicates what type of encryption to provision the account with, if any.")]
        [ValidateNotNull]
        public EncryptionConfigType? Encryption { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 6, Mandatory = false,
            HelpMessage = "If the encryption type is User assigned, this is the key vault the user wishes to use")]
        [ValidateNotNull]
        public string KeyVaultId { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 7, Mandatory = false,
            HelpMessage = "If the encryption type is User assigned, this is the key name in the key vault the user wishes to use")]
        [ValidateNotNull]
        public string KeyName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 8, Mandatory = false,
            HelpMessage = "If the encryption type is User assigned, this is the key version of the key the user wishes to use")]
        [ValidateNotNull]
        public string KeyVersion { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                if (DataLakeStoreClient.GetAccount(ResourceGroupName, Name) != null)
                {
                    throw new CloudException(string.Format(Resources.DataLakeStoreAccountExists, Name));
                }
            }
            catch (CloudException ex)
            {
                if (ex.Body != null && !string.IsNullOrEmpty(ex.Body.Code) && ex.Body.Code == "ResourceNotFound" ||
                    ex.Message.Contains("ResourceNotFound"))
                {
                    // account does not exists so go ahead and create one
                }
                else if (ex.Body != null && !string.IsNullOrEmpty(ex.Body.Code) &&
                         ex.Body.Code == "ResourceGroupNotFound" || ex.Message.Contains("ResourceGroupNotFound"))
                {
                    // resource group not found, let create throw error don't throw from here
                }
                else
                {
                    // all other exceptions should be thrown
                    throw;
                }
            }

            // validation of encryption parameters
            if (Encryption != null)
            {
                var identity = new EncryptionIdentity
                {
                    Type = EncryptionIdentityType.SystemAssigned,
                };
                var config = new EncryptionConfig
                {
                    Type = Encryption.Value,
                };

                if (Encryption.Value == EncryptionConfigType.UserManaged)
                {
                    if (string.IsNullOrEmpty(KeyVaultId) ||
                    string.IsNullOrEmpty(KeyName) ||
                    string.IsNullOrEmpty(KeyVersion))
                    {
                        throw new PSArgumentException(Resources.MissingKeyVaultParams);
                    }

                    config.KeyVaultMetaInfo = new KeyVaultMetaInfo
                    {
                        KeyVaultResourceId = KeyVaultId,
                        EncryptionKeyName = KeyName,
                        EncryptionKeyVersion = KeyVersion
                    };
                }
                else
                {
                    if (!string.IsNullOrEmpty(KeyVaultId) ||
                    !string.IsNullOrEmpty(KeyName) ||
                    !string.IsNullOrEmpty(KeyVersion))
                    {
                        WriteWarning(Resources.IgnoredKeyVaultParams);
                    }
                }

                WriteObject(DataLakeStoreClient.CreateOrUpdateAccount(ResourceGroupName, Name, DefaultGroup, Location, Tags, identity, config));
            }
            else
            {
                WriteObject(DataLakeStoreClient.CreateOrUpdateAccount(ResourceGroupName, Name, DefaultGroup, Location, Tags));
            }
        }
    }
}