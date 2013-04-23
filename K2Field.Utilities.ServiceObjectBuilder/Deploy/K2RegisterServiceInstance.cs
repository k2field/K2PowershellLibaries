using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using SourceCode.EnvironmentSettings.Client;
using SourceCode.Framework.Deployment;
using SourceCode.ProjectSystem;
using SourceCode.Workflow.Design.EnvironmentSettings;
using SourceCode.Hosting.Client.BaseAPI;

using SourceCode.SmartObjects.Services.Management;

using K2Field.Utilities.ServiceObjectBuilder.HelperClasses;

namespace K2Field.Utilities.ServiceObjectBuilder
{
    public class K2RegisterServiceInstance : Task
    {
        #region Member Variables

        private string k2Server = string.Empty;
        private int k2HostServerPort;

        private string serviceTypeGuid = string.Empty;
        private string serviceInstanceGuid = string.Empty;
        private string serviceInstanceSystemName = string.Empty;
        private string serviceInstanceDisplayName = string.Empty;
        private string serviceInstanceDescription = string.Empty;
        private string serviceInstanceFilePath = string.Empty;
        private string serviceInstanceClassName = string.Empty;

        private string configImpersonate = "false";
        private string configKeyNames = string.Empty;
        private string configKeyValues = string.Empty;
        private string configKeysRequired = string.Empty;
        private string configKeyDelimiter;
            
        #endregion

        #region Properties

        [Required]
        public string K2Server
        {
            get { return k2Server; }
            set { k2Server = value; }
        }

        [Required]
        public int K2HostServerPort
        {
            get { return k2HostServerPort; }
            set { k2HostServerPort = value; }
        }

        [Required]
        public string ServiceTypeGuid
        {
            get
            {
                return serviceTypeGuid;
            }
            set
            {
                serviceTypeGuid = value;
            }
        }

        [Required]
        public string ServiceInstanceGuid
        {
            get
            {
                return serviceInstanceGuid;
            }
            set
            {
                serviceInstanceGuid = value;
            }
        }

        [Required]
        public string ServiceInstanceSystemName
        {
            get
            {
                return serviceInstanceSystemName;
            }
            set
            {
                serviceInstanceSystemName = value;
            }
        }

        public string ServiceInstanceDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(serviceInstanceDisplayName))
                    serviceInstanceDisplayName = ServiceInstanceSystemName;

                return serviceInstanceDisplayName;
            }
            set
            {
                serviceInstanceDisplayName = value;
            }
        }

        public string ServiceInstanceDescription
        {
            get
            {
                return serviceInstanceDescription;
            }
            set
            {
                serviceInstanceDescription = value;
            }
        }

        public String ConfigImpersonate
        {
            get { return configImpersonate; }
            set { configImpersonate = value; }
        }

        public string ConfigKeyNames
        {
            get { return configKeyNames; }
            set { configKeyNames = value; }
        }

        public string ConfigKeyValues
        {
            get { return configKeyValues; }
            set { configKeyValues = value; }
        }

        public string ConfigKeysRequired
        {
            get { return configKeysRequired; }
            set { configKeysRequired = value; }
        }

        public string ConfigKeyDelimiter
        {
            get { return configKeyDelimiter; }
            set { configKeyDelimiter = value; }
        }

        private string ConnectionString
        {
            get
            {
                return K2Helper.GetK2ConnectionString(K2Server, K2HostServerPort.ToString());
            }
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            bool result;
            int currentStep = 0;

            // clean up some values
            char token = '"';

            K2Server = K2Helper.StripLeadingAndTailing(K2Server, token);
            ServiceInstanceGuid = K2Helper.StripLeadingAndTailing(ServiceInstanceGuid, token);
            ServiceInstanceSystemName = K2Helper.StripLeadingAndTailing(ServiceInstanceSystemName, token);
            ServiceInstanceDisplayName = K2Helper.StripLeadingAndTailing(ServiceInstanceDisplayName, token);
            ServiceInstanceDescription = K2Helper.StripLeadingAndTailing(ServiceInstanceDescription, token);
            ConfigKeyNames = K2Helper.StripLeadingAndTailing(configKeyNames, token);
            ConfigKeyValues = K2Helper.StripLeadingAndTailing(ConfigKeyValues, token);
            ConfigKeysRequired = K2Helper.StripLeadingAndTailing(ConfigKeysRequired, token);
            ConfigKeyDelimiter = K2Helper.StripLeadingAndTailing(ConfigKeyDelimiter, token);

          
            LogHelper.LogMessage("\n\n*****     BEGIN     *******\n");

            LogHelper.LogSubStepInfo("K2 Server: " + K2Server);
            LogHelper.LogSubStepInfo("K2 HostServer Port: " + K2HostServerPort.ToString());
            LogHelper.LogSubStepInfo("Service Instance Guid: " + ServiceInstanceGuid);
            LogHelper.LogSubStepInfo("Service Instance : " + ServiceInstanceSystemName);
            LogHelper.LogSubStepInfo("Service Instance : " + ServiceInstanceDisplayName);
            LogHelper.LogSubStepInfo("Service Instance : " + ServiceInstanceDescription);
            LogHelper.LogSubStepInfo("Config Key Names : " + ConfigKeyNames);
            LogHelper.LogSubStepInfo("Config Key Values : " + ConfigKeyValues);
            LogHelper.LogSubStepInfo("Config Key Reqds : " + ConfigKeysRequired);
            LogHelper.LogSubStepInfo("Config Delimiter : " + ConfigKeyDelimiter);

            try
            {
                LogHelper.LogStep(++currentStep, "Validating");

                if (!GuidHelper.IsGuid(ServiceTypeGuid))
                    throw new ArgumentException("Invalid Service Type Guid");

                if (!GuidHelper.IsGuid(ServiceInstanceGuid))
                    throw new ArgumentException("Invalid Service Instance Guid");

                // Create the Service Type Guid
                Guid svcTypeGuid = new Guid(ServiceTypeGuid);

                // Create the Service Instance Guid
                Guid svcInstanceGuid = new Guid(ServiceInstanceGuid);

                LogHelper.LogStep(++currentStep, "Getting K2 Service Manager");
                LogHelper.LogSubStep("Create K2 Service Manager");
                ServiceManagementServer svcMng = new ServiceManagementServer();

                using (svcMng.CreateConnection())
                {
                    LogHelper.LogSubStep("Opening Connection to K2 Server");
                    svcMng.Connection.Open(ConnectionString);
                    LogHelper.LogSubStepInfo("Connection State: " +
                        (svcMng.Connection.IsConnected ? "Connected" : "Disconnected"));

                    char delim = ',';
                    if (ConfigKeyDelimiter.Length == 1)
                    {
                        delim = ConfigKeyDelimiter.ToCharArray()[0];
                    }

                    LogHelper.LogSubStep("Getting Service Instance Configuration");
                    string svcInstanceConfig = K2Helper.GetK2ServiceInstanceConfig(
                        ConfigImpersonate, ConfigKeyNames, ConfigKeyValues,
                        ConfigKeysRequired, delim);

                    LogHelper.LogSubStepInfo("Service Instance Config = ");
                    LogHelper.LogSubStepInfo(svcInstanceConfig);

                    LogHelper.LogStep(++currentStep, "Registering Service Instance");
                    LogHelper.LogSubStep("Determining if Service Instance already exists");

                    // Check if this Service Instance is already registered
                    if (!K2Helper.DetermineIfServiceInstanceExists(svcMng, svcInstanceGuid))
                    {
                        LogHelper.LogSubStepInfo("Service Instance does not exist");

                        LogHelper.LogSubStep("Registering new Service Instance");
                        svcMng.RegisterServiceInstance(
                            svcTypeGuid, svcInstanceGuid, ServiceInstanceSystemName, 
                            ServiceInstanceDisplayName, ServiceInstanceDescription, svcInstanceConfig);

                        if (!K2Helper.DetermineIfServiceInstanceExists(svcMng, svcInstanceGuid))
                            throw new Exception("Registration of Service Instance Unsuccessful");
                        else
                            LogHelper.LogSubStepInfo("Registration of Service Instance was Successful");
                    }
                    else
                    {
                        LogHelper.LogSubStepInfo("Service Instance already exists");

                        LogHelper.LogSubStep("Updating existing Service Instance");

                        svcMng.UpdateServiceInstance(
                            svcTypeGuid, svcInstanceGuid, ServiceInstanceSystemName,
                            ServiceInstanceDisplayName, ServiceInstanceDescription, svcInstanceConfig);

                        if (!K2Helper.DetermineIfServiceInstanceExists(svcMng, svcInstanceGuid))
                            throw new Exception("Update of Service Instance Unsuccessful");
                        else
                            LogHelper.LogSubStepInfo("Update of Service Instance was Successful");
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
                //LogHelper.LogSubStepInfo(string.Format("Got an error:{0}{1}", ex.Message, ex.StackTrace));
                //result = false;
            }
            finally
            {
                LogHelper.LogMessage("\n*****       END       *******\n\n");
            }


            return result;
        }

    }
}
