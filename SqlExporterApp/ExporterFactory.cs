using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExporter
{
    public class ExporterFactory
    {

        public static FileExporterBase GetExporter(DBTargetConfiguration dbConfig, ExportJobConfiguration exportConfig,IFileSystem fileSystem)
        {
            switch (exportConfig.exporttype)
            {
                case ExportType.CSV:
                    return new CSVFileExporter(dbConfig, exportConfig,fileSystem);
                    break;
                case ExportType.XMLperRow:
                    throw new NotImplementedException("no Exporter for XMLPerRow defined so far");
                    break;
                case ExportType.FilePerRow:
                    throw new NotImplementedException("no Exporter for FilePerRow defined so far");
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown Enum Value");
                    break;
            }


        }
    }
}
