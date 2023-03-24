using Microsoft.Data.SqlClient.Server;
using System.IO.Abstractions;


namespace SqlExporter
{
    public class FileExporterBase
    {

        internal bool is_initialized = false;
        public DBTargetConfiguration dbConfig { get; }
        public ExportJobConfiguration exportConfig { get; }

        internal IFileSystem fileSystem;

        public FileExporterBase(DBTargetConfiguration dbconfig, ExportJobConfiguration exportconfig, IFileSystem filesystem)
        {
            dbConfig = dbconfig;
            exportConfig = exportconfig;
            fileSystem = filesystem;
        }

        public virtual void FinalizeFile()
        {
            //Todo --> ensure filewriter is properly closed (even in exception cases..)
        }


        public virtual void Initialize()
        {
            
            //Todo --> check for Path existence and create necessary folders

            //Todo --> try create filewriter instance --> throw error in case "append" is not allowed.


            is_initialized = true;
        }

        public virtual void Process(List<string> header, List<string> dataRow)
        {
            if (!is_initialized)
            {
                throw new InvalidOperationException("Call Initialize() first");
            }

            //Todo --> Write input data to File
        }


        internal string GetFileName(string dynamicparam = null)
        {

            return string.Format(exportConfig.filenamepattern, dbConfig.serverName, dbConfig.instanceName, dbConfig.stageName, exportConfig.queryname, exportConfig.createdat, dynamicparam);
        }
    }
}