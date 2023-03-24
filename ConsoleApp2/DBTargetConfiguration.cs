using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExporter
{
    public class DBTargetConfiguration
    {
        public string serverName { get; }
        public string instanceName { get; }
        public string stageName { get; }
        public string connectionString { get { return sqlBuilder.ConnectionString; } }

        public SqlConnectionStringBuilder sqlBuilder;



        public DBTargetConfiguration(string serverName, string instanceName, string stageName, string connectionString)
        {
            this.serverName = serverName;
            this.instanceName = instanceName;
            this.stageName = stageName;
            sqlBuilder = new SqlConnectionStringBuilder(connectionString);
        }



    }
}
