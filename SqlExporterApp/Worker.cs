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
using System.Xml;
using System.Xml.Linq;

namespace SqlExporter
{
    public static class Extensions
    {
        public static string SafeConvertString(this IDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) 
            { 
                return Convert.ToString(reader[colIndex]) ?? string.Empty;
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

                        result.Add(reader.SafeConvertString(i));
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

            if (!string.IsNullOrEmpty(options.templatePath))
            {
                var p = new TemplateParser();
                p.Parse(options.templatePath, new FileSystem(), runTimestamp);
                dbConfigs = p.dbConfigs;
                exportConfigs = p.exportConfigs;
            }
            else
            {
                //constructing a single call
                var dbConfig = new DBTargetConfiguration(options.serverName, options.instanceName, options.stageName,options.connectionString);
                var exportConfig = new ExportJobConfiguration(options.queryname, options.filenamepattern, options.exporttype, options.query, options.append, runTimestamp);
                dbConfigs.Add(dbConfig);
                exportConfigs.Add(exportConfig);
            }

            foreach (var dbConf in dbConfigs)
            {
                //do for all Databases the following
                ExportData(exportConfigs, dbConf);
            }
        }

        public void ExportData(List<ExportJobConfiguration> exportConfigs, DBTargetConfiguration dbConf)
        {
            logger.LogInformation("Connecting to {0}", dbConf.ToString());
            using (var dbConn = new SqlConnection(dbConf.connectionString))
            {
                dbConn.Open();

                //Run Specific Export
                foreach (var export in exportConfigs)
                {
                    var exporter = ExporterFactory.GetExporter(dbConf, export, new FileSystem());
                    try
                    {
                        logger.LogInformation("Start Export for {0}", export.ToString());
                        exporter.Initialize();

                        using (SqlCommand command = new SqlCommand(exporter.exportConfig.query, dbConn))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        exporter.Process(reader.GetRowHeaders(), reader.GetDataRow());
                                    }
                                    catch (Exception e)
                                    {
                                        logger.LogError(e, "issue on exporting {0} {1}, FirstColumn: {2}", export.queryname, dbConf.instanceName, reader.SafeConvertString(0));
                                    }
   
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

                logger.LogInformation("Closed connection to {0}", dbConf.ToString());


            }
        }
    }
}
