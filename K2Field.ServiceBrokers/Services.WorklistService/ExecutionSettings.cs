using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceCode.SmartObjects.Services.WorklistService
{
    internal sealed class ExecutionSettings
    {
    
        internal ExecutionSettings(string connectionString)
        {
            this.UseImpersonation = false;
            this.ConnectionString = connectionString;
        }

        internal ExecutionSettings(string impersonateConnectionString, string impersonateUser)
        {
            this.UseImpersonation = true;
            this.ConnectionString = impersonateConnectionString;
            this.ImpersonateUser = impersonateUser;
        }

        public bool UseImpersonation
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }

        public string ImpersonateUser
        {
            get;
            set;
        }
    }
}
