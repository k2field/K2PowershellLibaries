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
    public class K2RegisterServiceType : Task
    {
        #region Member Variables

        private string k2Server = string.Empty;
        private int k2HostServerPort;

        private string serviceTypeGuid = string.Empty;
        private string serviceTypeSystemName = string.Empty;
        private string serviceTypeDisplayName = string.Empty;
        private string serviceTypeDescription = string.Empty;
        private string serviceTypeFilePath = string.Empty;
        private string serviceTypeClassName = string.Empty;

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
        public string ServiceTypeSystemName
        {
            get
            {
                return serviceTypeSystemName;
            }
            set
            {
                serviceTypeSystemName = value;
            }
        }

        public string ServiceTypeDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(serviceTypeDisplayName))
                    serviceTypeDisplayName = ServiceTypeSystemName;

                return serviceTypeDisplayName;
            }
            set
            {
                serviceTypeDisplayName = value;
            }
        }
        
        public string ServiceTypeDescription
        {
            get
            {
                return serviceTypeDescription;
            }
            set
            {
                serviceTypeDescription = value;
            }
        }

        [Required]
        public string ServiceTypeAssemblyPath
        {
            get
            {
                return serviceTypeFilePath;
            }
            set
            {
                serviceTypeFilePath = value;
            }
        }
        
        [Required]
        public string ServiceTypeClassName
        {
            get
            {
                return serviceTypeClassName;
            }
            set
            {
                serviceTypeClassName = value;
            }
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
            LogHelper.LogMessage("\n\n*****     BEGIN       *******\n");

            LogHelper.LogSubStepInfo("K2 Server: " + K2Server);
            LogHelper.LogSubStepInfo("K2 HostServer Port: " + K2HostServerPort.ToString());
            LogHelper.LogSubStepInfo("Service Type Guid: " + ServiceTypeGuid);
            LogHelper.LogSubStepInfo("Service Type : " + ServiceTypeSystemName);
            LogHelper.LogSubStepInfo("Service Type : " + ServiceTypeDisplayName);
            LogHelper.LogSubStepInfo("Service Type : " + ServiceTypeDescription);
            LogHelper.LogSubStepInfo("Service Type : " + ServiceTypeAssemblyPath);
            LogHelper.LogSubStepInfo("Service Type : " + serviceTypeClassName);

            try
            {
                LogHelper.LogStep(++currentStep, "Validating");
                if (!GuidHelper.IsGuid(ServiceTypeGuid))
                    throw new ArgumentException("Invalid Service Type Guid");

                if(!File.Exists(ServiceTypeAssemblyPath))
                    throw new ArgumentException("Service Type Assembly Path is not valid.");

                // Create the Guid
                Guid svcGuid = new Guid(ServiceTypeGuid);   

                LogHelper.LogStep(++currentStep, "Getting K2 Service Manager");
                LogHelper.LogSubStep("Create K2 Service Manager");
                ServiceManagementServer svcMng = new ServiceManagementServer();

                using (svcMng.CreateConnection())
                {
                    LogHelper.LogSubStep("Opening Connection to K2 Server");
                    svcMng.Connection.Open(ConnectionString);
                    LogHelper.LogSubStepInfo("Connection State: " + 
                        (svcMng.Connection.IsConnected ? "Connected" : "Disconnected"));

                    LogHelper.LogStep(++currentStep, "Registering Service Type");
                    LogHelper.LogSubStep("Determining if Service Type already exists");


                    // Check if this Service Type is already registered
                    if (String.IsNullOrEmpty(svcMng.GetServiceType(svcGuid)))
                    {

                        LogHelper.LogSubStepInfo("Service Type does not exist");

                        LogHelper.LogSubStep("Getting Service Type Definition");

                        // NOTE: Registering the Service Type without this Service Type definition 
                        // results in incorrect registration
                        string svcTypeDefinition =
                            K2Helper.GetK2ServiceTypeDefinition(
                                svcGuid, ServiceTypeSystemName, ServiceTypeDisplayName,
                                ServiceTypeDescription, ServiceTypeAssemblyPath, ServiceTypeClassName);

                        LogHelper.LogSubStepInfo(svcTypeDefinition);

                        LogHelper.LogSubStep("Registering new Service Type");                        
                        svcMng.RegisterServiceType(svcGuid, svcTypeDefinition);

                        if(String.IsNullOrEmpty(svcMng.GetServiceType(svcGuid)))
                            throw new Exception("Registration of Service Type Unsuccessful");
                        else
                            LogHelper.LogSubStepInfo("Registration of Service Type was Successful");
                    }
                    else
                    {
                        LogHelper.LogSubStepInfo("Service Type already exists");

                        LogHelper.LogSubStep("Updating existing Service Type");
                        svcMng.UpdateServiceType(
                            svcGuid, ServiceTypeSystemName, ServiceTypeDisplayName, 
                            ServiceTypeDescription, ServiceTypeAssemblyPath, ServiceTypeClassName);

                        if (String.IsNullOrEmpty(svcMng.GetServiceType(svcGuid)))
                            throw new Exception("Update of Service Type Unsuccessful");
                        else
                            LogHelper.LogSubStepInfo("Update of Service Type was Successful");
                    }
                }

                result = true;
            }
            catch 
            {
                throw;
            }
            finally
            {
                LogHelper.LogMessage("\n*****       END       *******\n\n");
            }


            return result;
        }

    }
}
