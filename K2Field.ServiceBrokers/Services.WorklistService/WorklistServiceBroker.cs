using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SourceCode.SmartObjects.Services.ServiceSDK;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;

namespace SourceCode.SmartObjects.Services.WorklistService
{
    public class WorklistServiceBroker : ServiceAssemblyBase
    {
        #region ..: private members  :..

        private string _connectionString;
        private string _connectionStringImpersonate;

        #endregion

        /// <summary>
        /// Adds service parameters that are needed for service execution.
        /// </summary>
        /// <returns></returns>
        public override string GetConfigSection()
        {
            // The K2 server property is used to identigy the server running K2 for this service instance.
            base.Service.ServiceConfiguration.Add("Connection String", true, "Integrated=True;IsPrimaryLogin=False;Authenticate=True;EncryptedPassword=False;Host=localhost;Port=5252");
            base.Service.ServiceConfiguration.Add("Impersonate Connection String", true, "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=True;Host=localhost;Port=5252;WindowsDomain=MyDomain;UserID=SvcAccount;Password=password");

            return base.GetConfigSection();
        }

        /// <summary>
        /// Describes what our service is capable of doing and what parameters would be needed.
        /// </summary>
        /// <returns></returns>
        public override string DescribeSchema()
        {
            base.Service.Name = "WorklistService";
            base.Service.MetaData.DisplayName = "Worklist Service";
            base.Service.MetaData.Description = "Service that is used to retrieve user(s) Worklist items.";

            BasicWorklistItem bwi = new BasicWorklistItem();
            DetailedWorklistItem dwi = new DetailedWorklistItem();
            WorklistItemAction wla = new WorklistItemAction();
            base.Service.ServiceObjects.Add(bwi.DescribeServiceObject());
            base.Service.ServiceObjects.Add(dwi.DescribeServiceObject());
            base.Service.ServiceObjects.Add(wla.DescribeServiceObject());
            
            return base.DescribeSchema();
        }

        public override void Extend()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Execute()
        {
            ValidateConfigSection();
            base.ServicePackage.ResultTable = null;
            DataTable result = new DataTable("Result");
            try
            {
                foreach (ServiceObject serviceObj in base.Service.ServiceObjects)
                {
                    foreach (Method method in serviceObj.Methods)
                    {
                        #region build the input collections for method execution
                        // build the properties collection.
                        Dictionary<string, object> properties = new Dictionary<string, object>();
                        foreach (Property property in serviceObj.Properties)
                        {
                            if ((property.Value != null) && (!string.IsNullOrEmpty(property.Value.ToString())))
                            {
                                properties.Add(property.Name, property.Value);
                            }
                        }

                        // build the method parameters collection
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        foreach (MethodParameter parameter in method.MethodParameters)
                        {
                            if ((parameter.Value != null) && (!string.IsNullOrEmpty(parameter.Value.ToString())))
                            {
                                parameters.Add(parameter.Name, parameter.Value);
                            }
                        }
                        #endregion

                        ExecutionSettings settings = null;

                        if (properties.ContainsKey("UserName"))
                        {
                            if (!(string.IsNullOrEmpty(properties["UserName"].ToString())))
                            {
                                settings = new ExecutionSettings(this._connectionStringImpersonate, properties["UserName"].ToString());
                            }
                        }
                        else
                            settings = new ExecutionSettings(this._connectionString);


                        string methodName = method.Name.ToLower();

                        switch (serviceObj.Name.ToLower())
                        {
                            case "basicworklistitem":
                                {
                                    BasicWorklistItem worklist = new BasicWorklistItem(settings);
                                    if (methodName == "getworklistitems")
                                        result = worklist.GetWorklistItems(properties, parameters);

                                    if (methodName == "loadworklistitem")
                                    {
                                        if ((properties.ContainsKey("SerialNumber") && (properties["SerialNumber"].ToString() != string.Empty)))
                                        {
                                            result = worklist.LoadWorklistItem(properties["SerialNumber"].ToString());
                                        }
                                        else
                                            throw new ArgumentException("Input value cannot be null or empty", "serialNumber");
                                    }
                                    break;
                                }
                            case "detailedworklistitem":
                                {
                                    DetailedWorklistItem worklist = new DetailedWorklistItem(settings);
                                    if (methodName == "getworklistitems")
                                        result = worklist.GetWorklistItems(properties, parameters);

                                    if (methodName == "loadworklistitem")
                                    {
                                        if ((properties.ContainsKey("SerialNumber") && (properties["SerialNumber"].ToString() != string.Empty)))
                                        {
                                            result = worklist.LoadWorklistItem(properties["SerialNumber"].ToString());
                                        }
                                        else
                                            throw new ArgumentException("Input value cannot be null or empty", "serialNumber");
                                    }
                                    break;
                                }
                            case "worklistitemaction":
                                {
                                    WorklistItemAction worklistaction = new WorklistItemAction(_connectionString, _connectionStringImpersonate);

                                    if (methodName == "getworklistitemactions")
                                        result = worklistaction.GetWorklistItemActions(Convert.ToString(properties["SerialNumber"]));
                                        
                                    if (methodName == "redirectworklistitem")   
                                        worklistaction.RedirectWorklistItem(Convert.ToString(properties["SerialNumber"]), Convert.ToString(parameters["UserName"]));

                                    if (methodName == "redirectmanageduserworklistitem")
                                        worklistaction.RedirectManagedUserWorklistItem(Convert.ToString(parameters["ManagedUserName"]), Convert.ToString(properties["SerialNumber"]), Convert.ToString(parameters["UserName"]));


                                    if (methodName == "actionworklistitem")
                                        worklistaction.ActionWorklistItem(Convert.ToString(properties["SerialNumber"]), Convert.ToString(properties["ActionName"]));

                                    if (methodName == "openworklistitem")
                                        result = worklistaction.OpenWorklistItem(Convert.ToString(properties["SerialNumber"]));

                                    if (methodName == "releaseworklistitem")
                                        result = worklistaction.ReleaseWorklistItem(Convert.ToString(properties["SerialNumber"]));

                                    break;
                                }
                        }
                    }
                }
                base.ServicePackage.ResultTable = result;
                base.ServicePackage.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                base.ServicePackage.IsSuccessful = false;
                base.ServicePackage.ServiceMessages.Add(new ServiceMessage(ex.Message, MessageSeverity.Error));
            }
        }

        #region private members

        /// <summary>
        /// Ensures that service configuration properties have been properly assigned values.
        /// </summary>
        private void ValidateConfigSection()
        {
            ServiceConfiguration config = base.Service.ServiceConfiguration;
            _connectionString = config["Connection String"].ToString();
            _connectionStringImpersonate = config["Impersonate Connection String"].ToString();

            if(string.IsNullOrEmpty(_connectionString))
            {
                base.ServicePackage.IsSuccessful = false;
                base.ServicePackage.ServiceMessages.Add(new ServiceMessage("Connection String property must be specified.", MessageSeverity.Error));
            }
            if (string.IsNullOrEmpty(_connectionStringImpersonate))
            {
                base.ServicePackage.IsSuccessful = false;
                base.ServicePackage.ServiceMessages.Add(new ServiceMessage("Impersonate Connection String property must be specified.", MessageSeverity.Error));
            }
        }

        #endregion
    }
}
