using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlExporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

namespace SqlExporterTest
{
    [TestClass]
    public class TemplateParserTests
    {
        [TestMethod]
        public void verify_TemplateParser()
        {
            var dbConfig = new DBTargetConfiguration("1", "blb", "prod", "Server=myServerAddress;Database=myDataBase;");
            var exportConfig = new ExportJobConfiguration("query1", @"abcd", ExportType.CSV, "... query ....", false, DateTime.MinValue);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\template.txt", new MockFileData("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<SqlExporterTemplate>\r\n\t<DBTargets>\r\n\t\t<DBTarget serverName=\"1\" instanceName=\"blb\" stageName=\"prod\" connectionString=\"Server=myServerAddress;Database=myDataBase;\" />\r\n\t\t<DBTarget serverName=\"2\" instanceName=\"blb\" stageName=\"dev\" connectionString=\"\" />\r\n\r\n\r\n\t</DBTargets>\r\n\r\n\r\n\t<ExportJobConfigurations>\r\n\t\t<ExportJobConfiguration queryname=\"query1\" filenamepattern = \"abcd\" exporttype=\"CSV\" append=\"false\"> ... query ....</ExportJobConfiguration>\r\n\t\t<ExportJobConfiguration queryname=\"query2\" filenamepattern = \"ddf\" exporttype=\"XMLperRow\" append=\"true\"> ... query ....</ExportJobConfiguration>\r\n\t\t<ExportJobConfiguration queryname=\"query3\" filenamepattern = \"ddfwer\" exporttype=\"FileperRow\" append=\"false\"> ... query ....</ExportJobConfiguration>\r\n\t</ExportJobConfigurations>\r\n</SqlExporterTemplate>") },
            
            });

            var p = new TemplateParser();
            p.Parse(@"c:\template.txt", fileSystem,DateTime.MinValue);

            Assert.AreEqual(dbConfig.serverName, p.dbConfigs[0].serverName, "serverName wrong");
            Assert.AreEqual(dbConfig.instanceName, p.dbConfigs[0].instanceName, "instanceName wrong");
            Assert.AreEqual(dbConfig.stageName, p.dbConfigs[0].stageName, "stageName wrong");
            Assert.AreEqual(dbConfig.connectionString, p.dbConfigs[0].connectionString, "connectionString wrong");


            Assert.AreEqual(exportConfig.queryname, p.exportConfigs[0].queryname, "queryname wrong");
            Assert.AreEqual(exportConfig.query, p.exportConfigs[0].query, "query wrong");
            Assert.AreEqual(exportConfig.exporttype, p.exportConfigs[0].exporttype, "exporttype wrong");
            Assert.AreEqual(exportConfig.append, p.exportConfigs[0].append, "append wrong");
            Assert.AreEqual(exportConfig.filenamepattern, p.exportConfigs[0].filenamepattern, "filenamepattern wrong");

        }
        [TestMethod]
        public void verify_TemplateParser_withCDATAQuery()
        {
            var dbConfig = new DBTargetConfiguration("1", "blb", "prod", "Server=myServerAddress;Database=myDataBase;");
            var exportConfig = new ExportJobConfiguration("query1", @"abcd", ExportType.CSV, "... query ....", false, DateTime.MinValue);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\template.txt", new MockFileData("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<SqlExporterTemplate>\r\n\t<DBTargets>\r\n\t\t<DBTarget serverName=\"1\" instanceName=\"blb\" stageName=\"prod\" connectionString=\"Server=myServerAddress;Database=myDataBase;\" />\r\n\t</DBTargets>\r\n\t<ExportJobConfigurations>\r\n\t\t<ExportJobConfiguration queryname=\"query1\" filenamepattern = \"abcd\" exporttype=\"CSV\" append=\"false\"> \r\n\t\r\n\t\t\r\n\t\t<![CDATA[\r\n   ... query ....\r\n]]>\r\n\t\t</ExportJobConfiguration>\r\n\t</ExportJobConfigurations>\r\n</SqlExporterTemplate>") },

            });

            var p = new TemplateParser();
            p.Parse(@"c:\template.txt", fileSystem, DateTime.MinValue);

            Assert.AreEqual(dbConfig.serverName, p.dbConfigs[0].serverName, "serverName wrong");
            Assert.AreEqual(dbConfig.instanceName, p.dbConfigs[0].instanceName, "instanceName wrong");
            Assert.AreEqual(dbConfig.stageName, p.dbConfigs[0].stageName, "stageName wrong");
            Assert.AreEqual(dbConfig.connectionString, p.dbConfigs[0].connectionString, "connectionString wrong");


            Assert.AreEqual(exportConfig.queryname, p.exportConfigs[0].queryname, "queryname wrong");
            Assert.AreEqual(exportConfig.query, p.exportConfigs[0].query, "query wrong");
            Assert.AreEqual(exportConfig.exporttype, p.exportConfigs[0].exporttype, "exporttype wrong");
            Assert.AreEqual(exportConfig.append, p.exportConfigs[0].append, "append wrong");
            Assert.AreEqual(exportConfig.filenamepattern, p.exportConfigs[0].filenamepattern, "filenamepattern wrong");

        }
        
    }
}