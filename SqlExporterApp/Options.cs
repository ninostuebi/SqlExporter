using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;


namespace SqlExporter
{
    public enum configMode
    {
        
    }
    public class Options
    {
        [Option('t', "template", Required = true, HelpText = "path to template file, cannot be combined with any other parameter",SetName ="template")]
        public string templatePath { get; set; }

        [Option('s', "serverName", Required = false, HelpText = "serverName can be used in filepattern with index 0", SetName = "single_call",Default ="")]
        public string serverName { get; set; }
        [Option('i', "instanceName", Required = false, HelpText = "instanceName can be used in filepattern with index 1", SetName = "single_call", Default = "")]
        public string instanceName { get; set; }
        [Option('v', "stageName", Required = false, HelpText = "stageName can be used in filepattern with index 2", SetName = "single_call", Default = "")]
        public string stageName { get; set; }
        [Option('c', "connectionString", Required = true, HelpText = "Enter mssql connectionString as needed by DbDriver", SetName = "single_call")]
        public string connectionString { get; set; }
        
        [Option('n', "queryname", Required = false, HelpText = "queryname can be used in filepattern with index 3", SetName = "single_call", Default = "")]
        public string queryname { get; set; }
        [Option('p', "filenamepattern", Required = true, HelpText = "Enter filename pattern using string.Format syntax of c# ie: C:\\export\\{0}\\{1}_{2}\\{4:yyyy-MM-dd}_{5}.txt", SetName = "single_call")]
        public string filenamepattern { get; set; }
        [Option('e', "exporttype", Required = true, HelpText = "Enter filename pattern using string.Format syntax", SetName = "single_call")]
        public ExportType exporttype { get; set; }
        [Option('q', "query", Required = true, HelpText = "SQL Query which should be executed against target database and output will be exportet - supported Values:  CSV,XMLperRow or FilePerRow", SetName = "single_call")]
        public string query { get; set; }
        [Option('a', "append", Required = false, HelpText = "define if content should be added in case file already exists", SetName = "single_call",Default = false)]
        public bool append { get; set; }




        //[Option('l', "log", Required = false, HelpText = "")]
        //public string logfiletarget { get; set; }
       
       
    }
}
