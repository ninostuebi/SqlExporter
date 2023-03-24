using Microsoft.Data.SqlClient.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExporter
{
    public class CSVFileExporter : FileExporterBase
    {
        public static char SEPARATOR = ';';
        private bool writeHeader = false;
        private Stream fileStream;
        private StreamWriter sw;
        
        public CSVFileExporter(DBTargetConfiguration dbconfig, ExportJobConfiguration exportconfig, IFileSystem filesystem) : base(dbconfig, exportconfig,filesystem) { }

        public override void FinalizeFile()
        {
            base.FinalizeFile();
            sw.Flush();
            sw.Close();
            fileStream.Close();
        }


        public override void Initialize()
        {

            //Todo --> check for Path existence and create necessary folders
            var finalPath = base.GetFileName();
            
            //ensure directory structure
            string directory = fileSystem.Path.GetDirectoryName(finalPath) ?? string.Empty;
            if (!string.IsNullOrEmpty(directory) && !fileSystem.Directory.Exists(directory))
            {
                fileSystem.Directory.CreateDirectory(directory);
            }
            bool fileCreated = true;
            if (fileSystem.File.Exists(finalPath))
            {
                fileCreated = false;
                if(!exportConfig.append)
                {
                    throw new IOException("file already exists");
                }
            }
            else
            {
                //create the File                
                fileStream= fileSystem.FileStream.New(finalPath, FileMode.Append);
                sw= new StreamWriter(fileStream, new UTF8Encoding(true));
            }

            if(fileCreated)
            {
                this.writeHeader= true; 
            }


            base.is_initialized = true;
        }

        public override void Process(List<string> header, List<string> dataRow)
        {
            base.Process(header, dataRow);

            if(writeHeader)
            {
                //writing header to file
                sw.WriteLine(string.Join(SEPARATOR, header));
                writeHeader= false;
            }

            sw.WriteLine(GenerateCSVLine(dataRow));
        }

        private string GenerateCSVLine(List<string> data)
        {
            StringBuilder sb = new StringBuilder(); 

            foreach (string column in data) {
                //if string contains separator, needs to be surrounded with "
                //if string starts or EndsWith whitespace needs to be surrounded as well
                bool quotate = false;
                if(column.StartsWith(" ") || column.EndsWith(" ") || column.Contains(SEPARATOR))
                {
                    quotate = true;
                }

                if(quotate)
                {
                    sb.Append('"');
                    //if string containts quotation marks they need to be doubled
                    
                }
                sb.Append(column.Replace("\"", "\"\""));

                if (quotate)
                {
                    sb.Append("\"");
                }
                sb.Append(SEPARATOR);

            }

            return sb.ToString().TrimEnd(SEPARATOR);

        }
        

    }
}
