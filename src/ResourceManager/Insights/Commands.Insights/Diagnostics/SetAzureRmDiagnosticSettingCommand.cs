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

using Microsoft.Azure.Commands.Insights.OutputClasses;
using Microsoft.Azure.Management.Insights;
using Microsoft.Azure.Management.Insights.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Xml;

namespace Microsoft.Azure.Commands.Insights.Diagnostics
{
    /// <summary>
    /// Get the list of events for at a subscription level.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureRmDiagnosticSetting"), OutputType(typeof(PSServiceDiagnosticSettings))]
    public class SetAzureRmDiagnosticSettingCommand : ManagementCmdletBase
    {
        #region Parameters declarations

        /// <summary>
        /// Gets or sets the resourceId parameter of the cmdlet
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the storage account parameter of the cmdlet
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The storage account id")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountId { get; set; }

        /// <summary>
        /// Gets or sets the service bus rule id parameter of the cmdlet
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The service bus rule id")]
        [ValidateNotNullOrEmpty]
        public string ServiceBusRuleId { get; set; }

        /// <summary>
        /// Gets or sets the enable parameter of the cmdlet
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The value indicating whether the diagnostics should be enabled or disabled")]
        [ValidateNotNullOrEmpty]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the categories parameter of the cmdlet
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The log categories")]
        [ValidateNotNullOrEmpty]
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the timegrain parameter of the cmdlet
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The timegrains")]
        [ValidateNotNullOrEmpty]
        public List<string> Timegrains { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether retention should be enabled
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The value indicating whether the retention should be enabled")]
        [ValidateNotNullOrEmpty]
        public bool? RetentionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the OMS workspace Id
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The OMS workspace Id")]
        [ValidateNotNullOrEmpty]
        public string WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the retention in days
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The retention in days.")]
        public int? RetentionInDays { get; set; }

        #endregion

        protected override void ProcessRecordInternal()
        {
            var putParameters = new ServiceDiagnosticSettingsCreateOrUpdateParameters();

            ServiceDiagnosticSettingsResource getResponse = this.InsightsManagementClient.ServiceDiagnosticSettings.GetAsync(resourceUri: this.ResourceId, cancellationToken: CancellationToken.None).Result;

            ServiceDiagnosticSettingsResource properties = getResponse;

            if (!string.IsNullOrWhiteSpace(this.StorageAccountId))
            {
                properties.StorageAccountId = this.StorageAccountId;
            }

            if (!string.IsNullOrWhiteSpace(this.ServiceBusRuleId))
            {
                properties.ServiceBusRuleId = this.ServiceBusRuleId;
            }

            if (!string.IsNullOrWhiteSpace(this.WorkspaceId))
            {
                properties.WorkspaceId = this.WorkspaceId;
            }

            if (this.Categories == null && this.Timegrains == null)
            {
                foreach (var log in properties.Logs)
                {
                    log.Enabled = this.Enabled;
                }

                foreach (var metric in properties.Metrics)
                {
                    metric.Enabled = this.Enabled;
                }
            }
            else
            {
                if (this.Categories != null)
                {
                    foreach (string category in this.Categories)
                    {
                        LogSettings logSettings = properties.Logs.FirstOrDefault(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));

                        if (logSettings == null)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Log category '{0}' is not available", category));
                        }

                        logSettings.Enabled = this.Enabled;
                    }
                }

                if (this.Timegrains != null)
                {
                    foreach (string timegrainString in this.Timegrains)
                    {
                        TimeSpan timegrain = XmlConvert.ToTimeSpan(timegrainString);
                        MetricSettings metricSettings = properties.Metrics.FirstOrDefault(x => TimeSpan.Equals(x.TimeGrain, timegrain));

                        if (metricSettings == null)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Metric timegrain '{0}' is not available", timegrainString));
                        }
                        metricSettings.Enabled = this.Enabled;
                    }
                }
            }

            if (this.RetentionEnabled.HasValue)
            {
                var retentionPolicy = new RetentionPolicy
                {
                    Enabled = this.RetentionEnabled.Value,
                    Days = this.RetentionInDays.Value
                };

                if (properties.Logs != null)
                {
                    foreach (LogSettings logSettings in properties.Logs)
                    {
                        logSettings.RetentionPolicy = retentionPolicy;
                    }
                }

                if (properties.Metrics != null)
                {
                    foreach (MetricSettings metricSettings in properties.Metrics)
                    {
                        metricSettings.RetentionPolicy = retentionPolicy;
                    }
                }
            }

            putParameters.Logs = properties.Logs;
            putParameters.Metrics = properties.Metrics;
            putParameters.ServiceBusRuleId = properties.ServiceBusRuleId;
            putParameters.StorageAccountId = properties.StorageAccountId;
            putParameters.WorkspaceId = properties.WorkspaceId;

            ServiceDiagnosticSettingsResource result = this.InsightsManagementClient.ServiceDiagnosticSettings.CreateOrUpdateAsync(resourceUri: this.ResourceId, parameters: putParameters, cancellationToken: CancellationToken.None).Result;
            WriteObject(result);
        }
    }
}
