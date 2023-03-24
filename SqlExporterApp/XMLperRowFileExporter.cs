using Microsoft.Data.SqlClient.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SqlExporter
{
    public class XMLFileExporter : FileExporterBase
    {


        
        public XMLFileExporter(DBTargetConfiguration dbconfig, ExportJobConfiguration exportconfig, IFileSystem filesystem) : base(dbconfig, exportconfig,filesystem) { }

        public override void FinalizeFile()
        {
            base.FinalizeFile();
        }


        public override void Initialize()
        {
                   
            base.is_initialized = true;
        }

        public override void Process(List<string> header, List<string> dataRow)
        {
            base.Process(header, dataRow);

            var finalPath = base.GetFileName(dataRow[0]);
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
                if (!exportConfig.append)
                {
                    throw new IOException("file already exists");
                }
            }
            else
            {
                //create the File                
                using (var fileStream = fileSystem.FileStream.New(finalPath, FileMode.Append))
                {
                    using (var sw = new StreamWriter(fileStream, new UTF8Encoding(true)))
                    { 
                        GenerateXMLLine(dataRow, header, sw);
                    }
                }

            }

            
        }

        private void GenerateXMLLine(List<string> data, List<string> header,StreamWriter sw)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XmlNode));

            var doc = new XmlDocument();
            var root = doc.CreateElement(exportConfig.queryname);
            for (int i = 0; i < header.Count; i++)
            {
                var a = doc.CreateElement(header[i]);
                a.InnerText = data[i];
                root.AppendChild(a);
            }
            ser.Serialize(sw, root);

        }
        

    }
}
