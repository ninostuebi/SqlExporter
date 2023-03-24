using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlExporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection.PortableExecutable;

namespace SqlExporterTest
{
    [TestClass]
    public class ExportJobConfigurationTests
    {
        
        [TestMethod]
        public void verify_queryname_no_whitespace()
        {
          Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ExportJobConfiguration("query 1", @"C:\temp\fileexporter\{0}\{1}\{2}\{3}\{4:yyyy-MM-dd_HH-mm-ss}_export.txt", ExportType.XMLperRow, "select 1", false, DateTime.MinValue), "query name must not have special characters except _");

        }
        [TestMethod]
        public void verify_queryname_no_special()
        {

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ExportJobConfiguration("querysdflkj&31", @"C:\temp\fileexporter\{0}\{1}\{2}\{3}\{4:yyyy-MM-dd_HH-mm-ss}_export.txt", ExportType.XMLperRow, "select 1", false, DateTime.MinValue), "query name must not have special characters except _");

        }
        [TestMethod]
        public void verify_queryname()
        {


            Assert.IsNotNull(new ExportJobConfiguration("query_df1", @"C:\temp\fileexporter\{0}\{1}\{2}\{3}\{4:yyyy-MM-dd_HH-mm-ss}_export.txt", ExportType.XMLperRow, "select 1", false, DateTime.MinValue));
        }

    }
}