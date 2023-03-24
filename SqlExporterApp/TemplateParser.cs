using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SqlExporter
{
    public class TemplateParser
    {
       public List<DBTargetConfiguration> dbConfigs { get; set; }
       public List<ExportJobConfiguration> exportConfigs  {get;set;}
        public TemplateParser() {
            dbConfigs = new List<DBTargetConfiguration>();
            exportConfigs = new List<ExportJobConfiguration>();
        }
        public void Parse(string templatePath, IFileSystem fileSystem,DateTime createdAt) {
            if (!fileSystem.File.Exists(templatePath))
            {
                throw new FileNotFoundException("template file not found");
            }
            
            using (var fs = fileSystem.FileStream.New(templatePath, FileMode.Open, FileAccess.Read))
            {
                XmlDocument xmldoc = new XmlDataDocument();
                xmldoc.Load(fs);
                XmlNodeList xmlnode;
                xmlnode = xmldoc.GetElementsByTagName("SqlExporterTemplate");
                foreach (XmlNode node in xmlnode) { 
                
                    if(node.NodeType== XmlNodeType.Element && node.Name == "SqlExporterTemplate") 
                    {

                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (item.NodeType == XmlNodeType.Element && item.Name == "DBTargets")
                            {
                                foreach (XmlNode item2 in item.ChildNodes)
                                {
                                    if (item2.NodeType == XmlNodeType.Element && item2.Name == "DBTarget" && item2.Attributes != null && item2.Attributes.Count > 0)
                                    {
                                        string serverName = item2.Attributes["serverName"].Value;
                                        string instanceName = item2.Attributes["instanceName"].Value;
                                        string stageName = item2.Attributes["stageName"].Value;
                                        string connectionString = item2.Attributes["connectionString"].Value;
                                        dbConfigs.Add(new DBTargetConfiguration(serverName, instanceName, stageName, connectionString));
                                    }
                                }
                            }

                            if (item.NodeType == XmlNodeType.Element && item.Name == "ExportJobConfigurations")
                            {
                                foreach (XmlNode item2 in item.ChildNodes)
                                {
                                    if (item2.NodeType == XmlNodeType.Element && item2.Name == "ExportJobConfiguration" && item2.Attributes != null && item2.Attributes.Count> 0)
                                    {
                                        var attrib = item2.Attributes;
                                                                           
                                        string queryName = attrib["queryname"] != null ? attrib["queryname"]!.Value : string.Empty;
                                        string filenamepattern = attrib["filenamepattern"] != null ? attrib["filenamepattern"]!.Value : string.Empty;
                                        string exporttypestring = attrib["exporttype"] != null ? attrib["exporttype"]!.Value : string.Empty;
                                        ExportType exporttype = (ExportType)Enum.Parse(typeof(ExportType), exporttypestring);
                                        string appendstring = attrib["append"] != null ? attrib["append"]!.Value : string.Empty;
                                        bool append = bool.Parse(appendstring);
                                        string query = item2.FirstChild!.Value.Trim() ?? string.Empty;
                                        
                                        exportConfigs.Add(new ExportJobConfiguration(queryName, filenamepattern, exporttype,query, append,createdAt));

                                    }
                                }
                            }


                        }


               
                    }
                }
               


            }




            

        }
    }
}
