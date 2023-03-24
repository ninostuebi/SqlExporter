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
                    return new XMLFileExporter(dbConfig, exportConfig, fileSystem);
                    break;
                case ExportType.FileperRow:
                    return new FilePerRowFileExporter(dbConfig,exportConfig,fileSystem);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown Enum Value");
                    break;
            }


        }
    }
}
