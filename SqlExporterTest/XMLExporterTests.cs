using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlExporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

namespace SqlExporterTest
{
    [TestClass]
    public class XMLFileExporterTests
    {
        
        [TestMethod]
        public void verify_XMLExporter_file_Creation_fails_if_existing()
        {
            var dbConfig = new DBTargetConfiguration("server1", "instance1", "prod", "Server=myServerAddress;Database=myDataBase;");
            var exportConfig = new ExportJobConfiguration("query1", @"C:\temp\fileexporter\{0}\{1}\{2}\{3}\{4:yyyy-MM-dd_HH-mm-ss}_export.txt", ExportType.XMLperRow, "select 1", false, DateTime.MinValue);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\temp\fileexporter\server1\instance1\prod\query1\0001-01-01_00-00-00_export.txt", new MockFileData("Testing is meh.") },
               
            });

            var a = new XMLFileExporter(dbConfig, exportConfig, fileSystem);
            List<string> header = new List<string>();

            header.Add("id");
            header.Add("name");
            header.Add("description");
            header.Add("xml");

            List<string> content1 = new List<string>();
            content1.Add("1");
            content1.Add("name1");
            content1.Add("beschreibung 1");
            content1.Add("<xml><hello id=\"blbl\">content</hello></xml>");

            a.Initialize();
          
            Assert.ThrowsException<IOException>(() => a.Process(header,content1), "file already exists");
            
            
        }



        [TestMethod]
        public void verify_XMLExporter_file_Creation_Content()
        {
            var dbConfig = new DBTargetConfiguration("server1", "instance1", "prod", "Server=myServerAddress;Database=myDataBase;");
            var exportConfig = new ExportJobConfiguration("query1", @"C:\temp\fileexporter\{0}\{1}\{2}\{3}\{4:yyyy-MM-dd_HH-mm-ss}_{5}_export.txt", ExportType.XMLperRow, "select 1", false, DateTime.MinValue);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            
            List<string> header = new List<string>();

            header.Add("id");
            header.Add("name");
            header.Add("description");
            header.Add("xml");

            List<string> content1 = new List<string>();
            content1.Add("1");
            content1.Add("name1");
            content1.Add("beschreibung 1");
            content1.Add("<xml><hello id=\"blbl\">content</hello></xml>");

            List<string> content2 = new List<string>();
            content2.Add("2");
            content2.Add("name2");
            content2.Add("beschreibung 2");
            content2.Add(" <xml> mit whitespace</xml> ");

            List<string> content3 = new List<string>();
            content3.Add("3");
            content3.Add("name3");
            content3.Add("beschreibung 3");
            content3.Add("<xml> mit separator ;</xml> ");




            var a = new XMLFileExporter(dbConfig, exportConfig, fileSystem);
            a.Initialize();
            a.Process(header, content1);
            a.Process(header, content2);
            a.Process(header, content3);
            a.FinalizeFile();

            var t = fileSystem.GetFile(@"C:\temp\fileexporter\server1\instance1\prod\query1\0001-01-01_00-00-00_2_export.txt");
            Assert.AreEqual(t.TextContents, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<query1>\r\n  <id>2</id>\r\n  <name>name2</name>\r\n  <description>beschreibung 2</description>\r\n  <xml> &lt;xml&gt; mit whitespace&lt;/xml&gt; </xml>\r\n</query1>");

        }
    }
}