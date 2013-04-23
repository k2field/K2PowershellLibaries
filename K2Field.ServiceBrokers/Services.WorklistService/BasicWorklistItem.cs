using SDKObjects = SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SourceCode.SmartObjects.Services.WorklistService
{
    public class BasicWorklistItem
    {
        #region ..: private members :..

        internal ServiceObject _svcObject;
        internal ExecutionSettings _execSettings;

        #endregion

        #region ..: ctor(s) :..

        internal BasicWorklistItem()
        {

        }
        internal BasicWorklistItem(ExecutionSettings settings)
        {
            this._execSettings = settings;
        }

        #endregion

        #region ..: describe the service object :..

        internal virtual ServiceObject DescribeServiceObject()
        {
            this._svcObject = new ServiceObject("BasicWorklistItem");
            this._svcObject.Type = "BasicWorklistItem";
            this._svcObject.Active = true;
            this._svcObject.MetaData.DisplayName = "Basic Worklist Item";
            this._svcObject.MetaData.Description = "Represents a Basic WorklistItem";
            this._svcObject.Properties = DescribeProperties();
            this._svcObject.Methods = DescribeMethods();

            return this._svcObject;
        }

        internal virtual SDKObjects.Properties DescribeProperties()
        {
            var props = new SDKObjects.Properties();
            // worklistitem properties
            props.Add(new Property("AllocatedUser", "System.String", SoType.Text, new MetaData("Allocated User", "AllocatedUser")));
            props.Add(new Property("Data", "System.String", SoType.Text, new MetaData("Data", "Data")));
            props.Add(new Property("ID", "System.Int32", SoType.Autonumber, new MetaData("ID", "ID")));
            props.Add(new Property("SerialNumber", "System.String", SoType.Text, new MetaData("SerialNumber", "SerialNumber")));
            props.Add(new Property("Status", "System.String", SoType.Text, new MetaData("Status", "Status")));
            props.Add(new Property("Link", "System.String", SoType.HyperLink, new MetaData("Link", "Link")));

            // activity instance destination properties
            props.Add(new Property("ActivityID", "System.Int32", SoType.Number, new MetaData("Activity ID", "Activity ID")));
            props.Add(new Property("ActivityInstanceID", "System.Int32", SoType.Number, new MetaData("Activity Instance ID", "Activity Instance ID")));
            props.Add(new Property("ActivityInstanceDestinationID", "System.Int32", SoType.Number, new MetaData("Activity Instance Destination ID", "Activity Instance Destination ID")));
            props.Add(new Property("ActivityName", "System.String", SoType.Text, new MetaData("Activity Name", "Activity Name")));
            props.Add(new Property("Priority", "System.String", SoType.Text, new MetaData("Priority", "Priority")));
            props.Add(new Property("StartDate", "System.DateTime", SoType.DateTime, new MetaData("StartDate", "StartDate")));

            // process instance properties
            props.Add(new Property("ProcessInstanceID", "System.Int32", SoType.Number, new MetaData("Process Instance ID", "Process Instance ID")));
            props.Add(new Property("ProcessFullName", "System.String", SoType.Text, new MetaData("Process Full Name", "Process Full Name")));
            props.Add(new Property("ProcessName", "System.String", SoType.Text, new MetaData("Process Name", "Process Name")));
            props.Add(new Property("Folio", "System.String", SoType.Text, new MetaData("Folio", "Folio")));

            // event instance properties
            props.Add(new Property("EventInstanceName", "System.String", SoType.Text, new MetaData("Event Instance Name", "Event Instance Name")));

            // username property for impersonation
            props.Add(new Property("UserName", "System.String", SoType.Text, new MetaData("User Name", "User Name")));
            return props;
        }

        internal virtual Methods DescribeMethods()
        {
            Methods methods = new Methods();
            methods.Add(new Method("GetWorklistItems", MethodType.List, new MetaData("Get Worklist Items", "Returns a collection of worklist items."), GetRequiredProperties("GetWorklistItems"), GetMethodParameters("GetWorklistItems"), GetInputProperties("GetWorklistItems"), GetReturnProperties("GetWorklistItems")));
            methods.Add(new Method("LoadWorklistItem", MethodType.Read, new MetaData("Load Worklist Item", "Returns the specified worklist item."), GetRequiredProperties("LoadWorklistItem"), GetMethodParameters("LoadWorklistItem"), GetInputProperties("LoadWorklistItem"), GetReturnProperties("LoadWorklistItem")));
            return methods;
        }

        internal virtual InputProperties GetInputProperties(string method)
        {
            InputProperties props = new InputProperties();

            switch (method)
            {
                case "GetWorklistItems":
                    {
                        #region properties
                        props.Add(this._svcObject.Properties["Status"]);
                        props.Add(this._svcObject.Properties["ActivityName"]);
                        props.Add(this._svcObject.Properties["ProcessName"]);
                        props.Add(this._svcObject.Properties["ProcessFullName"]);
                        props.Add(this._svcObject.Properties["Folio"]);
                        props.Add(this._svcObject.Properties["EventInstanceName"]);
                        props.Add(this._svcObject.Properties["Priority"]);
                        props.Add(this._svcObject.Properties["UserName"]);
                        break;
                        #endregion
                    }
                case "LoadWorklistItem":
                    {
                        props.Add(this._svcObject.Properties["SerialNumber"]);
                        props.Add(this._svcObject.Properties["UserName"]);
                        break;
                    }
            }
            return props;
        }

        internal virtual Validation GetRequiredProperties(string method)
        {
            RequiredProperties props = new RequiredProperties();
            Validation validation = null;
            validation = new Validation();
            switch (method)
            {
                case "GetWorklistItems":
                    {
                        break;
                    }
                case "LoadWorklistItem":
                    {
                        props.Add(this._svcObject.Properties["SerialNumber"]);
                        break;
                    }
            }
            validation.RequiredProperties = props;
            return validation;
        }

        internal virtual MethodParameters GetMethodParameters(string method)
        {
            MethodParameters parameters = new MethodParameters();
            switch (method)
            {
                case "GetWorklistItems":
                    {
                        break;
                    }
            }
            return parameters;
        }

        internal virtual ReturnProperties GetReturnProperties(string method)
        {
            ReturnProperties props = new ReturnProperties();
            switch (method)
            {
                case "GetWorklistItems":
                case "LoadWorklistItem":
                    {
                        #region GetWorklistItems
                        // worklistitem properties
                        props.Add(this._svcObject.Properties["AllocatedUser"]);
                        props.Add(this._svcObject.Properties["Data"]);
                        props.Add(this._svcObject.Properties["ID"]);
                        props.Add(this._svcObject.Properties["Link"]);
                        props.Add(this._svcObject.Properties["SerialNumber"]);
                        props.Add(this._svcObject.Properties["Status"]);

                        // activity instance destination properties
                        props.Add(this._svcObject.Properties["ActivityID"]);
                        props.Add(this._svcObject.Properties["ActivityInstanceID"]);
                        props.Add(this._svcObject.Properties["ActivityInstanceDestinationID"]);
                        props.Add(this._svcObject.Properties["ActivityName"]);
                        props.Add(this._svcObject.Properties["Priority"]);
                        props.Add(this._svcObject.Properties["StartDate"]);

                        // process instance properties
                        props.Add(this._svcObject.Properties["ProcessInstanceID"]);
                        props.Add(this._svcObject.Properties["ProcessFullName"]);
                        props.Add(this._svcObject.Properties["Folio"]);
                        props.Add(this._svcObject.Properties["EventInstanceName"]);
                        break;
                        #endregion
                    }
            }
            return props;
        }

        #endregion 

        #region ..: broker execution methods :..

        internal virtual void PopulateDataRow(WorklistItem item, DataRow row)
        {
            row["AllocatedUser"] = item.AllocatedUser;
            row["Data"] = item.Data;
            row["ID"] = item.ID;
            row["Link"] = "<hyperlink><link>" + HttpUtility.HtmlEncode(item.Data) + "</link><display>Open</display></hyperlink>";
            row["SerialNumber"] = item.SerialNumber;
            row["Status"] = item.Status;
            row["ActivityID"] = item.ActivityInstanceDestination.ActID;
            row["ActivityInstanceID"] = item.ActivityInstanceDestination.ActInstID;
            row["ActivityName"] = item.ActivityInstanceDestination.Name;
            row["Priority"] = item.ActivityInstanceDestination.Priority;
            row["StartDate"] = item.EventInstance.StartDate;
            row["ActivityInstanceDestinationID"] = item.ActivityInstanceDestination.ID;
            row["ProcessInstanceID"] = item.ProcessInstance.ID;
            row["ProcessFullName"] = item.ProcessInstance.FullName;
            row["ProcessName"] = item.ProcessInstance.Name;
            row["Folio"] = item.ProcessInstance.Folio;
            row["EventInstanceName"] = item.EventInstance.Name;
        }

        internal DataTable LoadWorklistItem(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
                throw new ArgumentException("Value must not be null or empty.", "serialNumer");

            try
            {
                DataTable dt = GetResultTable();
                DataRow row = dt.NewRow();

                ConnectionSetup cnnSetup = new ConnectionSetup();
                cnnSetup.ConnectionString = this._execSettings.ConnectionString;

                using (Connection cnn = new Connection())
                {
                    cnn.Open(cnnSetup);

                    if (this._execSettings.UseImpersonation)
                        cnn.ImpersonateUser(this._execSettings.ImpersonateUser);

                    WorklistItem item = cnn.OpenWorklistItem(serialNumber);

                    if (item != null)
                    {
                        PopulateDataRow(item, row);
                        dt.Rows.Add(row);
                    }

                    cnn.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetWorklistItems(Dictionary<string, object> properties, Dictionary<string, object> parameters)
        {
            try
            {
                DataTable dt = GetResultTable();
                WorklistCriteria criteria = GetWorklistCriteria(properties);

                ConnectionSetup cnnSetup = new ConnectionSetup();
                cnnSetup.ConnectionString = this._execSettings.ConnectionString;

                using (Connection cnn = new Connection())
                {
                    cnn.Open(cnnSetup);

                    if (this._execSettings.UseImpersonation)
                        cnn.ImpersonateUser(this._execSettings.ImpersonateUser);

                    Worklist worklist;
                    if ((criteria != null) && (criteria.Filters.GetLength(0) > 0))
                        worklist = cnn.OpenWorklist(criteria);
                    else
                        worklist = cnn.OpenWorklist();

                    foreach (WorklistItem item in worklist)
                    {
                        DataRow row = dt.NewRow();
                        PopulateDataRow(item, row);
                        dt.Rows.Add(row);
                    }
                    cnn.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        private WorklistCriteria GetWorklistCriteria(Dictionary<string, object> properties)
        {
            WorklistCriteria criteria = new WorklistCriteria();
            criteria.AddFilterField(WCLogical.Or, WCField.WorklistItemOwner, "Me", WCCompare.Equal, WCWorklistItemOwner.Me);
            criteria.AddFilterField(WCLogical.Or, WCField.WorklistItemOwner, "Other", WCCompare.Equal, WCWorklistItemOwner.Other);

            foreach (KeyValuePair<string, object> property in properties)
            {
                switch (property.Key)
                {
                    case "Status":
                        criteria.AddFilterField(WCLogical.And, WCField.WorklistItemStatus, WCCompare.Equal, property.Value);
                        break;
                    case "ActivityName":
                        criteria.AddFilterField(WCLogical.And, WCField.ActivityName, WCCompare.Equal, property.Value);
                        break;
                    case "ProcessName":
                        criteria.AddFilterField(WCLogical.And, WCField.ProcessName, WCCompare.Equal, property.Value);
                        break;
                    case "ProcessFullName":
                        criteria.AddFilterField(WCLogical.And, WCField.ProcessFullName, WCCompare.Equal, property.Value);
                        break;
                    case "Folio":
                        criteria.AddFilterField(WCLogical.And, WCField.ProcessFolio, WCCompare.Equal, property.Value);
                        break;
                    case "EventInstanceName":
                        criteria.AddFilterField(WCLogical.And, WCField.EventName, WCCompare.Equal, property.Value);
                        break;
                    case "Priority":
                        criteria.AddFilterField(WCLogical.And, WCField.ActivityPriority, WCCompare.Equal, property.Value);
                        break;
                }
            }

            return criteria;
        }

        internal virtual DataTable GetResultTable()
        {
            DataTable result = new DataTable("BasicWorklistItem");
            result.Columns.Add("AllocatedUser", typeof(string));
            result.Columns.Add("Data", typeof(string));
            result.Columns.Add("ID", typeof(Int32));
            result.Columns.Add("SerialNumber", typeof(string));
            result.Columns.Add("Status", typeof(string));
            result.Columns.Add("Link", typeof(string));
            result.Columns.Add("ActivityID", typeof(Int32));
            result.Columns.Add("ActivityInstanceID", typeof(Int32));
            result.Columns.Add("ActivityName", typeof(string));
            result.Columns.Add("Priority", typeof(string));
            result.Columns.Add("StartDate", typeof(DateTime));
            result.Columns.Add("ActivityInstanceDestinationID", typeof(Int32));
            result.Columns.Add("ProcessInstanceID", typeof(Int32));
            result.Columns.Add("ProcessFullName", typeof(string));
            result.Columns.Add("ProcessName", typeof(string));
            result.Columns.Add("Folio", typeof(string));
            result.Columns.Add("EventInstanceName", typeof(string));

            return result;
        }
    }
}
