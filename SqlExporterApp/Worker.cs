using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExporter
{
    public static class Extensions
    {
        public static string SafeGetString(this IDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) 
            { 
                return reader.GetString(colIndex);
            }
            return string.Empty;
        }
        public static string SafeGetDateTime(this IDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDateTime(colIndex).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Empty;
        }
        public static List<string> GetDataRow(this IDataReader reader)
        {
          
            List<string> result = new List<string>();

               for (int i = 0; i < reader.FieldCount; i++)
                {
                switch (reader.GetValue(i))
                {
                    case DateTime:
                        result.Add(reader.SafeGetDateTime(i));
                        break;
                    default:

                        result.Add(reader.SafeGetString(i));
                        break;
                }
            }

               return result;
        }
        
        public static List<string> GetRowHeaders(this IDataReader reader)
        {

            List<string> result = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                result.Add(reader.GetName(i));
            }

            return result;
        }

    }
    public class Worker
    {
        private ILogger logger;
        public Worker(ILogger _logger) {
        
           logger= _logger;
        }

        public void Run(SqlExporter.Options options) {

            logger.LogInformation("Hi i'm starting some work now!");
            var runTimestamp = DateTime.Now;

            var dbConfigs = new List<DBTargetConfiguration>();
            var exportConfigs = new List<ExportJobConfiguration>();
            var dbConfig = new DBTargetConfiguration("test_server", "test_db", "ACC", "");
            var exportConfig = new ExportJobConfiguration("query_1", "filepatterntoadd", ExportType.CSV, "select 1", false, runTimestamp);
            dbConfigs.Add(dbConfig);
            exportConfigs.Add(exportConfig);



            foreach (var dbConf in dbConfigs)
            {
                //do for all Databases the following
                //Initialize Connection
                ExportData(exportConfigs, dbConf);

            }



        }

        public void ExportData(List<ExportJobConfiguration> exportConfigs, DBTargetConfiguration dbConf)
        {
            using (var dbConn = new SqlConnection(dbConf.connectionString))
            {
                dbConn.Open();

                //Run Specific Export
                foreach (var export in exportConfigs)
                {
                    var exporter = ExporterFactory.GetExporter(dbConf, export, new FileSystem());
                    try
                    {
                        
                        exporter.Initialize();

                        using (SqlCommand command = new SqlCommand(exporter.exportConfig.query, dbConn))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    exporter.Process(reader.GetRowHeaders(), reader.GetDataRow());
   
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "issue on exporting {0} {1}", export.queryname, dbConf.instanceName);
                        
                    }
                    finally { exporter.FinalizeFile(); } 

                }
                dbConn.Close();




            }
        }
    }
}
