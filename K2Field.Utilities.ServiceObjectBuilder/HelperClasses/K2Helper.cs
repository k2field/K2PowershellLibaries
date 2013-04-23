using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.IO;
using SourceCode.EnvironmentSettings.Client;
using SourceCode.Framework.Deployment;
using SourceCode.ProjectSystem;
using SourceCode.Workflow.Design.EnvironmentSettings;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.SmartObjects.Services.Management;

namespace K2Field.Utilities.ServiceObjectBuilder.HelperClasses
{
    public static class K2Helper
    {
        /// <summary>
        /// Compiles the k2 project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// true if the project compiles successfully executed; otherwise, false.
        /// </returns>
        public static bool CompileK2Project(Project project)
        {
            bool result = false;
            DeploymentResults results;

            LogHelper.LogMessage("   -- Compiling the project");
            results = project.Compile();

            result = results.Successful;
            LogHelper.LogMessage("      Success=" + result.ToString());

            if (results.Errors.Count > 0)
            {
                StringBuilder sbErrors = new StringBuilder();

                sbErrors.Append("An exception occurred during compilation of the project.\n\n");

                // loop through the Compile errors and log them
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                    sbErrors.Append("     -- " + error.ErrorText + "\n");

                throw new Exception(sbErrors.ToString());
            }

            return result;
        }

        /// <summary>
        /// Gets the deployment package.
        /// </summary>
        /// <param name="project">The K2 project.</param>
        /// <param name="environmentManager">The K2 environment manager.</param>
        /// <param name="IsTest">Will Deployment Package be in Test Mode?</param>
        /// <returns>The Deployment Package</returns>
        public static DeploymentPackage CreateDeploymentPackage(
            Project project, EnvironmentSettingsManager environmentManager,
            string DeploymentLabel, string DeploymentDescription, bool IsTest)
        {
            DeploymentPackage package;

            LogHelper.LogMessage("      -- Creating Deployment Package");
            package = project.CreateDeploymentPackage();

            // Populate Environment Fields 
            foreach (EnvironmentInstance env in environmentManager.CurrentTemplate.Environments)
            {
                DeploymentEnvironment depEnv = package.AddEnvironment(env.EnvironmentName);

                foreach (EnvironmentField field in env.EnvironmentFields)
                {
                    depEnv.Properties[field.FieldName] = field.Value;
                }
            }

            LogHelper.LogMessage("   -- Setting Package Info");
            package.SelectedEnvironment = environmentManager.CurrentEnvironment.EnvironmentName;
            package.DeploymentLabelName = string.IsNullOrEmpty(DeploymentLabel) ? DateTime.Now.ToString() : DeploymentLabel;
            package.DeploymentLabelDescription = DeploymentDescription;
            package.TestOnly = IsTest;

            LogHelper.LogMessage("      SelectedEnvironment = " + package.SelectedEnvironment);
            LogHelper.LogMessage("      DeploymentLabelName = " + package.DeploymentLabelName);
            LogHelper.LogMessage("      DeploymentLabelDescription = " + package.DeploymentLabelDescription);
            LogHelper.LogMessage("      TestOnly = " + package.TestOnly);

            // Get the Default SmartObject Server in the Environment
            // The prefix "$Field=" is when the value of the SmartObject server is registered in the environment fields collection.
            // this will do a lookup in the environment with the display name of the field, and use the value.
            // If you set the value directly, no lookups will be performed.
            EnvironmentField smartObjectServerField =
                environmentManager.CurrentEnvironment.GetDefaultField(typeof(SmartObjectField));

            LogHelper.LogMessage("   -- Setting SmartObject Server ConnectionString");
            package.SmartObjectConnectionString = "$Field=" + smartObjectServerField.DisplayName;
            LogHelper.LogMessage("      SmartObject Server ConnectionString = " + package.SmartObjectConnectionString);

            // Get the Default Workflow Management Server in the Environment
            EnvironmentField workflowServerField =
                environmentManager.CurrentEnvironment.GetDefaultField(typeof(WorkflowManagementServerField));

            LogHelper.LogMessage("   -- Setting Workflow Management ConnectionString");
            package.WorkflowManagementConnectionString = "$Field=" + workflowServerField.DisplayName;
            LogHelper.LogMessage("      Workflow Management ConnectionString = " + package.WorkflowManagementConnectionString);

            return package;
        }

        /// <summary>
        /// Gets an EnvironmentSettingsManager for the specified environment.
        /// </summary>
        /// <param name="ConnStr">The K2 connection string.</param>
        /// <param name="EnvName">The targetted K2 environment.</param>
        /// <param name="UseCache">Use local K2 Environment Cache?</param>
        /// <returns>
        /// An EnvironmentSettingsManager object containing settings 
        /// for the specified environment
        /// </returns>
        public static EnvironmentSettingsManager GetEnvironmentManager(
            string ConnStr, string EnvName, bool LocalOnly)
        {
            LogHelper.LogMessage("   -- Creating K2 Environment Manager");
            LogHelper.LogMessage("   -- Use Local Environment Cache = " + LocalOnly.ToString());
            EnvironmentSettingsManager environmentManager = new EnvironmentSettingsManager(false, true);

            environmentManager.ConnectionString = ConnStr;

            // Use the Local Environment Cache only.  Do not connect to the K2 Server
            if (LocalOnly)
            {
                environmentManager.WorkOffline = true;

                LogHelper.LogMessage("   -- Loading Environment Cache");
                environmentManager.LoadCache();
            }
            // Connect to the K2 Server to retrieve the Environment settings
            else
            {
                LogHelper.LogMessage("   -- Connecting Environment Manager to K2 Server");
                environmentManager.ConnectToServer();

                LogHelper.LogMessage("   -- Refreshing Environment");
                environmentManager.Refresh();

                LogHelper.LogMessage("   -- Saving Environment to local cache");
                environmentManager.SaveCache();
            }

            LogHelper.LogMessage("   -- Setting the Environment to " + EnvName);
            if (!string.IsNullOrEmpty(EnvName))
                environmentManager.ChangeEnvironment(EnvName);

            LogHelper.LogMessage("   -- Initializing Environment Settings Manager");
            environmentManager.InitializeSettingsManager();

            LogHelper.LogMessage("   -- Getting the Environment fields");
            if (environmentManager.CurrentEnvironment == null)
            {
                throw new ArgumentException("Environment does not exist!");
            }
            else
            {
                environmentManager.GetEnvironmentFields(environmentManager.CurrentEnvironment);
            }

            return environmentManager;
        }

        /// <summary>
        /// Gets a k2 connection string.
        /// </summary>
        /// <param name="HostServerName">Name of the host server.</param>
        /// <param name="HostServerPortNum">The host server port num.</param>
        /// <param name="Domain">The domain.</param>
        /// <param name="UserID">The user ID.</param>
        /// <param name="Password">The password.</param>
        /// <returns>A K2 connection string</returns>
        public static string GetK2ConnectionString(
            string HostServerName, string HostServerPortNum, 
            string Domain, string UserID, string Password)
        {
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connBldrSmO =
                new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();

            connBldrSmO.Authenticate = true;
            connBldrSmO.Host = HostServerName;
            connBldrSmO.IsPrimaryLogin = true;
            connBldrSmO.SecurityLabelName = "K2";
            connBldrSmO.Port = Convert.ToUInt32(HostServerPortNum);
            connBldrSmO.Integrated = true;

            if (!string.IsNullOrEmpty(Domain) &&
                !string.IsNullOrEmpty(UserID) &&
                !string.IsNullOrEmpty(Password))
            {
                connBldrSmO.UserID = UserID;
                connBldrSmO.Password = Password;
                connBldrSmO.WindowsDomain = Domain;
            }

            return connBldrSmO.ToString();
        }

        /// <summary>
        /// Gets the k2 connection string.
        /// </summary>
        /// <param name="HostServerName">Name of the host server.</param>
        /// <param name="HostServerPortNum">The host server port num.</param>
        /// <returns>A K2 connection string</returns>
        public static string GetK2ConnectionString(
            string HostServerName, string HostServerPortNum)
        {
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connBldrSmO =
                new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();

            connBldrSmO.Authenticate = true;
            connBldrSmO.Host = HostServerName;
            connBldrSmO.IsPrimaryLogin = true;
            connBldrSmO.SecurityLabelName = "K2";
            connBldrSmO.Port = Convert.ToUInt32(HostServerPortNum);
            connBldrSmO.Integrated = true;

            return connBldrSmO.ToString();
        }

        public static  string StripLeadingAndTailing(string value, char token)
        {
            string results = value;

            results = results.TrimStart(token);
            results = results.TrimEnd(token);

            results = results.Replace("{[equals]}", "=").Replace("{[semicolon]}", ";");

            return results;
        }
        /// <summary>
        /// Gets the k2 service type definition.
        /// </summary>
        /// <param name="ServiceTypeGuid">The service type GUID.</param>
        /// <param name="ServiceTypeSystemName">Name of the service type system.</param>
        /// <param name="ServiceTypeDisplayName">Display name of the service type.</param>
        /// <param name="ServiceTypeDescription">The service type description.</param>
        /// <param name="ServiceTypeAssemblyPath">The service type assembly path.</param>
        /// <param name="ServiceTypeClassName">Name of the service type class.</param>
        /// <returns>The service type definition xml string</returns>
        public static string GetK2ServiceTypeDefinition(
            Guid ServiceTypeGuid, string ServiceTypeSystemName, string ServiceTypeDisplayName,
            string ServiceTypeDescription, string ServiceTypeAssemblyPath, string ServiceTypeClassName)
        {
            // Build the Service Type Definition
            StringBuilder sbServiceTypeDefinition = new StringBuilder();
            
            sbServiceTypeDefinition.Append("<servicetype ");
            sbServiceTypeDefinition.Append("name=\"");
            sbServiceTypeDefinition.Append(ServiceTypeSystemName);
            sbServiceTypeDefinition.Append("\" ");
            sbServiceTypeDefinition.Append("guid=\"");
            sbServiceTypeDefinition.Append(ServiceTypeGuid.ToString());
            sbServiceTypeDefinition.Append("\">");

            sbServiceTypeDefinition.Append("<metadata>");

            sbServiceTypeDefinition.Append("<display>");

            sbServiceTypeDefinition.Append("<displayname>");
            sbServiceTypeDefinition.Append(ServiceTypeDisplayName);
            sbServiceTypeDefinition.Append("</displayname>");

            sbServiceTypeDefinition.Append("<description>");
            sbServiceTypeDefinition.Append(ServiceTypeDescription);
            sbServiceTypeDefinition.Append("</description>");

            sbServiceTypeDefinition.Append(" </display>");

            sbServiceTypeDefinition.Append("</metadata>");

            sbServiceTypeDefinition.Append("<config>");

            sbServiceTypeDefinition.Append("<assembly path=\"");                
            sbServiceTypeDefinition.Append(ServiceTypeAssemblyPath);
            sbServiceTypeDefinition.Append("\" ");
            sbServiceTypeDefinition.Append("class=\"");
            sbServiceTypeDefinition.Append(ServiceTypeClassName);
            sbServiceTypeDefinition.Append("\" />");

            sbServiceTypeDefinition.Append("</config>");

            sbServiceTypeDefinition.Append("</servicetype>");

            return sbServiceTypeDefinition.ToString();
        }

        /// <summary>
        /// Builds a k2 service instance configuration xml string
        /// </summary>
        /// <param name="AuthImpersonate">if set to <c>true</c> [auth impersonate].</param>
        /// <param name="ConfigKeyNames">The config key names.</param>
        /// <param name="ConfigKeyValues">The config key values.</param>
        /// <param name="ConfigKeysRequired">The config keys required.</param>
        /// <param name="DelimiterChar">The delimiter char.</param>
        /// <returns>a k2 service instance configuration xml string</returns>
        public static string GetK2ServiceInstanceConfig(
            string AuthImpersonate, string ConfigKeyNames, string ConfigKeyValues, 
            string ConfigKeysRequired, char DelimiterChar)
        {
            StringBuilder sbConfigData = new StringBuilder();

            sbConfigData.Append("<serviceconfig>");

            sbConfigData.Append("<serviceauthentication securityprovider=\"\" ");
            sbConfigData.Append("impersonate=\"");
            sbConfigData.Append(AuthImpersonate);
            //  sbConfigData.Append(AuthImpersonate.ToString());
            sbConfigData.Append("\" ");
            sbConfigData.Append("isrequired=\"false\">");
            sbConfigData.Append("<username />");
            sbConfigData.Append("<password />");
            sbConfigData.Append("<extra />");
            sbConfigData.Append("</serviceauthentication>");

            // Get the Delimiter(s)
            //char[] delimiterChar = DelimiterChar.Trim().ToCharArray();

            // Parse the delimited Config Keys
            string[] configKeyNames = ConfigKeyNames.Split(DelimiterChar);

            // Parse the delimited Config Key Values
            string[] configKeyValues = ConfigKeyValues.Split(DelimiterChar);

            // Parse the delimited Config Keys Required
            string[] configKeysRequired = ConfigKeysRequired.Split(DelimiterChar);

            int configKeyNamesLength = configKeyNames.Length;
            int configKeyValuesLength = configKeyValues.Length;
            int configKeysRequiredLength = configKeysRequired.Length;

            // Check to make sure that the lengths match
            if ((configKeyNamesLength != configKeyValuesLength) || (configKeyNamesLength != configKeysRequiredLength))
                throw new ArgumentException("Item lengths of ConfigKeyNames, ConfigKeyValues and ConfigKeysRequired do not match.");

            // Build the Config Settings XML string
            sbConfigData.Append("<settings>");

            for (int i = 0; i < configKeyNamesLength; i++)
            {
                sbConfigData.Append("<key name=\"");
                sbConfigData.Append(configKeyNames[i].Trim());
                sbConfigData.Append("\" ");
                sbConfigData.Append("isrequired=\"");
                sbConfigData.Append(configKeysRequired[i].Trim());
                sbConfigData.Append("\">");
                sbConfigData.Append(configKeyValues[i].Trim());
                sbConfigData.Append("</key>");
            }            

            sbConfigData.Append("</settings>");

            sbConfigData.Append("</serviceconfig>");

            return sbConfigData.ToString();
        }

        /// <summary>
        /// Determines if a service instance exists.
        /// </summary>
        /// <param name="svcMng">ServiceManagementServer object.</param>
        /// <param name="ServiceInstanceGuid">The service instance GUID.</param>
        /// <returns>true if the the service instance already exists; otherwise, false.</returns>
        public static bool DetermineIfServiceInstanceExists(ServiceManagementServer svcMng, Guid ServiceInstanceGuid)
        {
            bool result = false;

            // Check if this Service Instance is already registered
            try
            {
                string _svcInstanceName = svcMng.GetServiceInstance(ServiceInstanceGuid);
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
