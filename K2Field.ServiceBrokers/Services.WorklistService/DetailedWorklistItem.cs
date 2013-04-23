using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SDKObjects = SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using SourceCode.Hosting.Client;
using SourceCode.Workflow.Client;
using SourceCode.Hosting.Client.BaseAPI;

namespace SourceCode.SmartObjects.Services.WorklistService
{
    public class DetailedWorklistItem : BasicWorklistItem
    {
        #region ctor(s)

        internal DetailedWorklistItem() { }

        internal DetailedWorklistItem(ExecutionSettings settings) : base(settings)
        {
        
        }

        #endregion

        internal override ServiceObject DescribeServiceObject()
        {
            ServiceObject so = base.DescribeServiceObject();
            so.Name = "DetailedWorklistItem";
            so.Type = "DetailedWorklistItem";
            so.Active = true;
            so.MetaData.DisplayName = "Detailed Worklist Item";
            so.MetaData.Description = "Represents a Detailed Worklist Item.";

            return so;
        }

        internal override ServiceSDK.Objects.Properties DescribeProperties()
        {
            SDKObjects.Properties props = base.DescribeProperties();

            // additional activity instance properties
            props.Add(new Property("ActivityDescription", "System.String", SoType.Text, new MetaData("Activity Description", "Activity Description")));
            props.Add(new Property("ActivityMetaData", "System.String", SoType.Text, new MetaData("Activity MetaData", "Activity MetaData")));
            props.Add(new Property("ActivityExpectedDuration", "System.Int32", SoType.Number, new MetaData("Activity Expected Duration", "Activity Expected Duration")));

            // additional process instance properties
            props.Add(new Property("ProcessDescription", "System.String", SoType.Text, new MetaData("Process Description", "Process Description")));
            props.Add(new Property("ProcessMetaData", "System.String", SoType.Text, new MetaData("Process MetaData", "Process Meta Data")));
            props.Add(new Property("ProcessExpectedDuration", "System.Int32", SoType.Number, new MetaData("Process Expected Duration", "Process Expected Duration")));
            props.Add(new Property("ProcessPriority", "System.String", SoType.Text, new MetaData("Process Priority", "Process Priority")));
            props.Add(new Property("ProcessInstanceStartDate", "System.DateTime", SoType.DateTime, new MetaData("Process Instance Start Date", "Process Instance Start Date")));

            return props;
        }

        internal override ReturnProperties GetReturnProperties(string method)
        {
            ReturnProperties props = base.GetReturnProperties(method);

            switch (method)
            {
                case "GetWorklistItems":
                case "LoadWorklistItem":
                    {
                        #region GetWorklistItems

                        props.Add(base._svcObject.Properties["ActivityDescription"]);
                        props.Add(base._svcObject.Properties["ActivityMetaData"]);
                        props.Add(base._svcObject.Properties["ActivityExpectedDuration"]);

                        props.Add(base._svcObject.Properties["ProcessDescription"]);
                        props.Add(base._svcObject.Properties["ProcessMetaData"]);
                        props.Add(base._svcObject.Properties["ProcessExpectedDuration"]);
                        props.Add(base._svcObject.Properties["ProcessPriority"]);
                        props.Add(base._svcObject.Properties["ProcessInstanceStartDate"]);
                        break;

                        #endregion
                    }
            }
            return props;
        }

        //internal DataTable LoadWorklistItem(string serialNumber)
        //{
        //    ConnectionSetup connectSetup = new ConnectionSetup();
        //    connectSetup.ConnectionString = _connectionString;
        //    Connection cnn = new Connection();

        //    DataTable dt = GetResultTable();

        //    try
        //    {
        //        cnn.Open(connectSetup);
        //        WorklistItem item = cnn.OpenWorklistItem(serialNumber);

        //        DataRow row = dt.NewRow();
        //        row["AllocatedUser"] = item.AllocatedUser;
        //        row["Data"] = item.Data;
        //        row["ID"] = item.ID;
        //        //row["Link"] = "<hyperlink><link>" + item.Data + "</link><display>Open</display></hyperlink>";
        //        row["SerialNumber"] = item.SerialNumber;
        //        row["Status"] = item.Status;
        //        row["ActivityID"] = item.ActivityInstanceDestination.ActID;
        //        row["ActivityInstanceID"] = item.ActivityInstanceDestination.ActInstID;
        //        row["ActivityInstanceDestinationID"] = item.ActivityInstanceDestination.ID;
        //        row["ActivityName"] = item.ActivityInstanceDestination.Name;
        //        row["Priority"] = item.ActivityInstanceDestination.Priority;
        //        row["StartDate"] = item.EventInstance.StartDate;
        //        row["ActivityDescription"] = item.ActivityInstanceDestination.Description;
        //        row["ActivityMetaData"] = item.ActivityInstanceDestination.MetaData;
        //        row["ActivityExpectedDuration"] = item.ActivityInstanceDestination.ExpectedDuration;
        //        row["ProcessInstanceID"] = item.ProcessInstance.ID;
        //        row["ProcessFullName"] = item.ProcessInstance.FullName;
        //        row["ProcessName"] = item.ProcessInstance.Name;
        //        row["Folio"] = item.ProcessInstance.Folio;
        //        row["ProcessDescription"] = item.ProcessInstance.Description;
        //        row["ProcessMetaData"] = item.ProcessInstance.MetaData;
        //        row["ProcessPriority"] = item.ProcessInstance.Priority;
        //        row["ProcessInstanceStartDate"] = item.ProcessInstance.StartDate;
        //        row["EventInstanceName"] = item.EventInstance.Name;
        //        dt.Rows.Add(row);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        cnn.Close();
        //        cnn.Dispose();
        //    }
        //}

        //public DataTable GetWorklistItems(Dictionary<string, object> properties, Dictionary<string, object> parameters)
        //{
        //    bool impersonate = false;
        //    string impersonateUser = "";
        //    ConnectionSetup connectSetup = new ConnectionSetup();
        //    connectSetup.ConnectionString = _connectionString;

        //    if (properties.ContainsKey("UserName"))
        //    {
        //        if (!(string.IsNullOrEmpty(properties["UserName"].ToString())))
        //        {
        //            connectSetup.ConnectionString = _connectionstringImpersonate;
        //            impersonateUser = properties["UserName"].ToString();
        //            impersonate = true;
        //        }
        //        else
        //            connectSetup.ConnectionString = _connectionString;
        //    }

        //    WorklistCriteria criteria = null;
        //    if (properties.Count > 0)
        //        criteria = GetWorklistCriteria(properties);
           
        //    Connection cnn = new Connection();
        //    try
        //    {
        //        cnn.Open(connectSetup);
        //        if (impersonate)
        //            cnn.ImpersonateUser(impersonateUser);

        //        DataTable dt = GetResultTable();

        //        Worklist worklist;
        //        if ((criteria != null) && (criteria.Filters.GetLength(0) > 0))
        //            worklist = cnn.OpenWorklist(criteria);
        //        else
        //            worklist = cnn.OpenWorklist();
        //        foreach (WorklistItem item in worklist)
        //        {
        //            DataRow row = dt.NewRow();
        //            row["AllocatedUser"] = item.AllocatedUser;
        //            row["Data"] = item.Data;
        //            row["ID"] = item.ID;
        //            //row["Link"] = "<hyperlink><link>" + item.Data + "</link><display>Open</display></hyperlink>";
        //            row["SerialNumber"] = item.SerialNumber;
        //            row["Status"] = item.Status;
        //            row["ActivityID"] = item.ActivityInstanceDestination.ActID;
        //            row["ActivityInstanceID"] = item.ActivityInstanceDestination.ActInstID;
        //            row["ActivityInstanceDestinationID"] = item.ActivityInstanceDestination.ID;
        //            row["ActivityName"] = item.ActivityInstanceDestination.Name;
        //            row["Priority"] = item.ActivityInstanceDestination.Priority;
        //            row["StartDate"] = item.EventInstance.StartDate;
        //            row["ActivityDescription"] = item.ActivityInstanceDestination.Description;
        //            row["ActivityMetaData"] = item.ActivityInstanceDestination.MetaData;
        //            row["ActivityExpectedDuration"] = item.ActivityInstanceDestination.ExpectedDuration;
        //            row["ProcessInstanceID"] = item.ProcessInstance.ID;
        //            row["ProcessFullName"] = item.ProcessInstance.FullName;
        //            row["ProcessName"] = item.ProcessInstance.Name;
        //            row["Folio"] = item.ProcessInstance.Folio;
        //            row["ProcessDescription"] = item.ProcessInstance.Description;
        //            row["ProcessMetaData"] = item.ProcessInstance.MetaData;
        //            row["ProcessPriority"] = item.ProcessInstance.Priority;
        //            row["ProcessInstanceStartDate"] = item.ProcessInstance.StartDate;
        //            row["EventInstanceName"] = item.EventInstance.Name;
        //            dt.Rows.Add(row);
        //        }
        //        if (impersonate)
        //            cnn.RevertUser();
        //        cnn.Close();
        //        cnn.Dispose();
        //        return dt;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private WorklistCriteria GetWorklistCriteria(Dictionary<string, object> properties)
        {
            WorklistCriteria criteria = new WorklistCriteria();

            foreach (KeyValuePair<string, object> property in properties)
            {
                switch (property.Key)
                {
                    case "Status":
                        criteria.AddFilterField(WCField.WorklistItemStatus, WCCompare.Equal, property.Value);
                        break;
                    case "ActivityName":
                        criteria.AddFilterField(WCField.ActivityName, WCCompare.Equal, property.Value);
                        break;
                    case "ProcessName":
                        criteria.AddFilterField(WCField.ProcessName, WCCompare.Equal, property.Value);
                        break;
                    case "ProcessFullName":
                        criteria.AddFilterField(WCField.ProcessFullName, WCCompare.Equal, property.Value);
                        break;
                    case "Folio":
                        criteria.AddFilterField(WCField.ProcessFolio, WCCompare.Equal, property.Value);
                        break;
                    case "EventInstanceName":
                        criteria.AddFilterField(WCField.EventName, WCCompare.Equal, property.Value);
                        break;
                    case "Priority":
                        criteria.AddFilterField(WCField.ActivityPriority, WCCompare.Equal, property.Value);
                        break;
                }
            }

            return criteria;
        }

        internal override DataTable GetResultTable()
        {
            DataTable result = base.GetResultTable();
           
            result.Columns.Add("ActivityDescription", typeof(string));
            result.Columns.Add("ActivityMetaData", typeof(string));
            result.Columns.Add("ActivityExpectedDuration", typeof(Int32));
            result.Columns.Add("ProcessDescription", typeof(string));
            result.Columns.Add("ProcessMetaData", typeof(string));
            result.Columns.Add("ProcessPriority", typeof(string));
            result.Columns.Add("ProcessInstanceStartDate", typeof(DateTime));

            return result;
        }

        internal override void PopulateDataRow(WorklistItem item, DataRow row)
        {
            base.PopulateDataRow(item, row);

            row["ActivityInstanceDestinationID"] = item.ActivityInstanceDestination.ID;
            row["ActivityDescription"] = item.ActivityInstanceDestination.Description;
            row["ActivityMetaData"] = item.ActivityInstanceDestination.MetaData;
            row["ActivityExpectedDuration"] = item.ActivityInstanceDestination.ExpectedDuration;
            row["ProcessDescription"] = item.ProcessInstance.Description;
            row["ProcessMetaData"] = item.ProcessInstance.MetaData;
            row["ProcessPriority"] = item.ProcessInstance.Priority;
            row["ProcessInstanceStartDate"] = item.ProcessInstance.StartDate;
        }
    }
}
