using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlExporter
{
    public class ExportJobConfiguration
    {
        public string queryname { get; }
        public string filenamepattern { get; }
        public ExportType exporttype { get; }
        public string query { get; }
        public bool append { get; }
        public DateTime createdat{ get; }

        public ExportJobConfiguration(string queryName, string filenamePattern, ExportType exportType, string query, bool append, DateTime createdAt)
        {

            if (!Regex.IsMatch(queryName, "^[A-Za-z0-9_]+$"))
            {
                throw new ArgumentOutOfRangeException("query name must not have special characters except _");
            }
            this.queryname = queryName;
            this.filenamepattern = filenamePattern;
            this.exporttype = exportType;
            this.query = query;
            this.append = append;
            this.createdat = createdAt;
        }


       


    }
}
