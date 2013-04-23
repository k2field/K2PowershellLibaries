using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.Hosting.Client;
using SourceCode.Workflow.Client;
using SourceCode.Hosting.Client.BaseAPI;

namespace SourceCode.SmartObjects.Services.WorklistService
{
    public class WorklistItemAction
    {
        #region private data members

        private string _connectionString;
        private string _connectionstringImpersonate;
        private Connection _cnn;

        private ServiceObject _so;

        #endregion

        #region ctor(s)

        internal WorklistItemAction() { }

        internal WorklistItemAction(string connectionString, string connectionStringImpersonate)
        {
            _connectionString = connectionString;
            _connectionstringImpersonate = connectionStringImpersonate;
        }

        #endregion

        /// <summary>
        /// Describes the functionality of the service object
        /// </summary>
        /// <returns>Returns the service object definition.</returns>
        internal ServiceObject DescribeServiceObject()
        {
            _so = new ServiceObject("WorklistItemAction");
            _so.Type = "WorklistItemAction";
            _so.Active = true;
            _so.MetaData.DisplayName = "Worklist Item Action";
            _so.MetaData.Description = "Used for to perform actions on worklist items";
            _so.Properties = DescribeProperties();
            _so.Methods = DescribeMethods();
            return _so;
        }

        private SourceCode.SmartObjects.Services.ServiceSDK.Objects.Properties DescribeProperties()
        {
            SourceCode.SmartObjects.Services.ServiceSDK.Objects.Properties properties = new SourceCode.SmartObjects.Services.ServiceSDK.Objects.Properties();
            // worklistitem properties
            properties.Add(new Property("ActionName", "System.String", SoType.Text, new MetaData("ActionName", "ActionName")));
            properties.Add(new Property("SerialNumber", "System.String", SoType.Text, new MetaData("Serial Number", "SerialNumber")));
            properties.Add(new Property("ErrorMessageBlankIfNoError", "System.String", SoType.Text, new MetaData("ErrorMessage", "Error Message which is Blank if there is No Error")));
            
            return properties;
        }

        private SourceCode.SmartObjects.Services.ServiceSDK.Objects.Methods DescribeMethods()
        {
            SourceCode.SmartObjects.Services.ServiceSDK.Objects.Methods methods = new Methods();
            methods.Add(new Method("GetWorklistItemActions", MethodType.List, new MetaData("Get Worklist Item Actions", "Returns a collection of worklist item actions."), GetRequiredProperties("GetWorklistItemActions"), GetMethodParameters("GetWorklistItemActions"), GetInputProperties("GetWorklistItemActions"), GetReturnProperties("GetWorklistItemActions")));
            methods.Add(new Method("RedirectWorklistItem", MethodType.Execute, new MetaData("Redirect Worklist Item", ""), GetRequiredProperties("RedirectWorklistItem"), GetMethodParameters("RedirectWorklistItem"), GetInputProperties("RedirectWorklistItem"), GetReturnProperties("RedirectWorklistItem")));
            methods.Add(new Method("RedirectManagedUserWorklistItem", MethodType.Execute, new MetaData("Redirect Managed User Worklist Item", ""), GetRequiredProperties("RedirectManagedUserWorklistItem"), GetMethodParameters("RedirectManagedUserWorklistItem"), GetInputProperties("RedirectManagedUserWorklistItem"), GetReturnProperties("RedirectManagedUserWorklistItem")));
            methods.Add(new Method("ActionWorklistItem", MethodType.Execute, new MetaData("Action Worklist Item", ""), GetRequiredProperties("ActionWorklistItem"), GetMethodParameters("ActionWorklistItem"), GetInputProperties("ActionWorklistItem"), GetReturnProperties("ActionWorklistItem")));
            methods.Add(new Method("OpenWorklistItem", MethodType.Execute, new MetaData("Open Worklist Item", ""), GetRequiredProperties("OpenWorklistItem"), GetMethodParameters("OpenWorklistItem"), GetInputProperties("OpenWorklistItem"), GetReturnProperties("OpenWorklistItem")));
            methods.Add(new Method("ReleaseWorklistItem", MethodType.Execute, new MetaData("Release Worklist Item", ""), GetRequiredProperties("ReleaseWorklistItem"), GetMethodParameters("ReleaseWorklistItem"), GetInputProperties("ReleaseWorklistItem"), GetReturnProperties("ReleaseWorklistItem")));
            return methods;
        }

        private InputProperties GetInputProperties(string method)
        {
            InputProperties properties = new InputProperties();

            switch (method)
            {
                case"ActionWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        properties.Add(_so.Properties["ActionName"]);
                        break;
                    }
                case "OpenWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }

                case "ReleaseWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "GetWorklistItemActions":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "RedirectWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "RedirectManagedUserWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }

            }
            return properties;
        }

        private Validation GetRequiredProperties(string method)
        {
            RequiredProperties properties = new RequiredProperties();
            Validation validation = null;
            validation = new Validation();
            switch (method)
            {
                case "ActionWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        properties.Add(_so.Properties["ActionName"]);
                        break;
                    }
                case "OpenWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "ReleaseWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "RedirectWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "RedirectManagedUserWorklistItem":
                    {
                        properties.Add(_so.Properties["SerialNumber"]);
                        break;
                    }
                case "GetWorklistItemActions":
                    {
                        properties.Add(new Property("SerialNumber", "System.String", SoType.Text, new MetaData("SerialNumber", "SerialNumber")));
                        break;
                    }
            }
            validation.RequiredProperties = properties;
            return validation;
        }

        private MethodParameters GetMethodParameters(string method)
        {
            MethodParameters parameters = new MethodParameters();
            switch (method)
            {
                case "RedirectWorklistItem":
                    {
                        parameters.Add(new MethodParameter("UserName", "System.String", SoType.Text, new MetaData("User Name", "")));
                        break;
                    }
                case "RedirectManagedUserWorklistItem":
                    {
                        parameters.Add(new MethodParameter("ManagedUserName", "System.String", SoType.Text, new MetaData("Managed User Name", "")));
                        parameters.Add(new MethodParameter("UserName", "System.String", SoType.Text, new MetaData("User Name", "")));
                        break;
                    }
            }
            return parameters;
        }

        private ReturnProperties GetReturnProperties(string method)
        {
            ReturnProperties properties = new ReturnProperties();
            switch (method)
            {
                case "GetWorklistItemActions":
                    {
                        properties.Add(_so.Properties["ActionName"]);
                        break;
                    }
                case "OpenWorklistItem":
                    {
                        properties.Add(_so.Properties["ErrorMessageBlankIfNoError"]);
                        break;
                    }
                case "ReleaseWorklistItem":
                    {
                        properties.Add(_so.Properties["ErrorMessageBlankIfNoError"]);
                        break;
                    }
            }
            return properties;
        }

        internal void RedirectWorklistItem(string serialNumber, string userName)
        {
            try
            {
                OpenConnection();
                WorklistItem item = _cnn.OpenWorklistItem(serialNumber, "ASP", false);

                item.Redirect(userName);
                CloseConnection();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                _cnn.Dispose();
            }
        }

        internal void RedirectManagedUserWorklistItem(string managedUser, string serialNumber, string userName)
        {
            try
            {
                OpenConnection();
                WorklistItem item = _cnn.OpenManagedWorklistItem(managedUser, serialNumber, "ASP", false);

                item.Redirect(userName);
                CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _cnn.Dispose();
            }
        }

        internal DataTable GetWorklistItemActions(string serialNumber)
        {
            DataTable result = new DataTable("WorklistAction");
            result.Columns.Add("ActionName", typeof(string));
            try
            {
                OpenConnection();
                WorklistItem item = _cnn.OpenWorklistItem(serialNumber, "ASP", false);

                foreach(SourceCode.Workflow.Client.Action action in item.Actions)
                {
                    DataRow row = result.NewRow();
                    row["ActionName"] = action.Name;

                    result.Rows.Add(row);
                }
                return result;

            }
            catch(Exception ex)
            {
                throw(ex);
            }
            finally
            {
                _cnn.Dispose();
            }
        }

        internal void ActionWorklistItem(string serialNumber, string actionName)
        { 
            try
            {
                OpenConnection();
                WorklistItem item = _cnn.OpenWorklistItem(serialNumber, "ASP", true);

                foreach(SourceCode.Workflow.Client.Action action in item.Actions)
                {
                    if(action.Name == actionName)
                        action.Execute();
                }
                CloseConnection();
            }
            catch(Exception ex)
            {
                throw(ex);
            }
            finally
            {
                _cnn.Dispose();
            }
        }

        internal DataTable OpenWorklistItem(string serialNumber)
        {
            DataTable result = new DataTable("WorklistAction");
            result.Columns.Add("ErrorMessageBlankIfNoError", typeof(string));
            DataRow row = result.NewRow();
            row["ErrorMessageBlankIfNoError"] = string.Empty;
       
            try
            {
                OpenConnection();
                WorklistItem item = _cnn.OpenWorklistItem(serialNumber, "ASP", true);
                
                
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("worklist"))
                {
                    row["ErrorMessageBlankIfNoError"] = "You cannot work on this item. Another user may have opened it before you. Refresh the list and try again.";
                    
                }
                else
                {
                    throw (ex);
                }
            }
            finally
            {

                _cnn.Dispose();
            }

            result.Rows.Add(row);
            return result;
        }

        internal DataTable ReleaseWorklistItem(string serialNumber)
        {
            DataTable result = new DataTable("WorklistAction");
            result.Columns.Add("ErrorMessageBlankIfNoError", typeof(string));
            DataRow row = result.NewRow();
            row["ErrorMessageBlankIfNoError"] = string.Empty;

            try
            {
                OpenConnection();
                WorklistItem item = _cnn.OpenWorklistItem(serialNumber, "ASP", true);
                item.Release();

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("worklist"))
                {
                    row["ErrorMessageBlankIfNoError"] = "You cannot work on this item. Another user may have opened it before you. Refresh the list and try again.";

                }
                else
                {
                    throw (ex);
                }
            }
            finally
            {

                _cnn.Dispose();
            }

            result.Rows.Add(row);
            return result;
        }

        private void OpenConnection()
        {
            ConnectionSetup connectSetup = new ConnectionSetup();
            connectSetup.ConnectionString = _connectionString;
            _cnn = new Connection();
            _cnn.Open(connectSetup);
        }

        private void CloseConnection()
        {
            _cnn.Close();
        }
    }
}
