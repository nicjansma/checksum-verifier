// <copyright file="DatabaseTests.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Database class
    /// </summary>
    [TestClass]
    public class DatabaseTests : TestBase
    {
        /// <summary>
        /// Tests creating a new Database
        /// </summary>
        [TestMethod]
        public void NewDatabase()
        {
            Database db = new Database(GetCorpusDatabase("test"));

            Assert.AreEqual(0, db.FileCount());
        }

        /// <summary>
        /// Tests FileCount()
        /// </summary>
        [TestMethod]
        public void FileCount()
        {
            Database db = LoadDatabase("simple");

            Assert.AreEqual(2, db.FileCount());
        }

        /// <summary>
        /// Tests HasFile()
        /// </summary>
        [TestMethod]
        public void HasFile()
        {
            Database db = LoadDatabase("simple");

            Assert.IsTrue(db.HasFile("1"));
        }
        
        /// <summary>
        /// Tests RemoveFile()
        /// </summary>
        [TestMethod]
        public void RemoveFile()
        {
            Database db = LoadDatabase("simple");

            Assert.IsTrue(db.HasFile("1"));
            db.RemoveFile("1");
            Assert.IsFalse(db.HasFile("1"));
            Assert.IsTrue(db.HasChanges());
        }
                
        /// <summary>
        /// Tests AddFile()
        /// </summary>
        [TestMethod]
        public void AddFile()
        {
            Database db = LoadDatabase("simple");

            // remove the file first
            Assert.IsTrue(db.HasFile("1"));
            db.RemoveFile("1");
            Assert.IsFalse(db.HasFile("1"));
            Assert.IsTrue(db.HasChanges());

            // add file
            db.AddFile(GetCorpusFile("simple", "1"), GetCorpusDirectory("simple"), PathType.RelativePath, ChecksumType.MD5);
            Assert.IsTrue(db.HasFile("1"));
            Assert.IsTrue(db.HasChanges());

            // get file back and check
            Assert.AreEqual("1", db.GetFile("1").ResolvedFileName);
            Assert.AreEqual(Test.TestConstants.Corpus1MD5Checksum, db.GetFile("1").Checksum);
        }

        /// <summary>
        /// Tests GetFile()
        /// </summary>
        [TestMethod]
        public void GetFile()
        {
            Database db = LoadDatabase("simple");

            Assert.IsTrue(db.HasFile("1"));

            Assert.AreEqual("1", db.GetFile("1").ResolvedFileName);
            Assert.AreEqual(Test.TestConstants.Corpus1MD5Checksum, db.GetFile("1").Checksum);
        }
                
        /// <summary>
        /// Tests FromFile
        /// </summary>
        [TestMethod]
        public void FromFile()
        {
            Database db = LoadDatabase("simple");

            Assert.AreEqual(2, db.FileCount());
        }

        /// <summary>
        /// Tests FromFile on a file that doesn't exist
        /// </summary>
        [TestMethod]
        public void FromFileDoesNotExist()
        {
            Database db = LoadDatabase("foo");

            Assert.AreEqual(0, db.FileCount());
        }

        /// <summary>
        /// Runs a comprehensive test on the simple database
        /// </summary>
        [TestMethod]
        public void Comprehensive()
        {
            Database db = LoadDatabase("simple");

            Assert.AreEqual(2, db.FileCount());

            List<FileChecksum> files = new List<FileChecksum>(db.Files);
            
            Assert.AreEqual(2, files.Count);

            Assert.AreEqual("1", files[0].ResolvedFileName);
            Assert.AreEqual(TestConstants.Corpus1MD5Checksum, files[0].Checksum);

            Assert.AreEqual("2", files[1].ResolvedFileName);
            Assert.AreEqual(TestConstants.Corpus2MD5Checksum, files[1].Checksum);
        }
        
        /// <summary>
        /// Tests write
        /// </summary>
        [TestMethod]
        public void Write()
        {
            Database db = LoadDatabase("test-write");
            db.AddFile(GetCorpusFile("simple", "1"), GetCorpusDirectory("simple"), PathType.RelativePath, ChecksumType.MD5);
            Assert.IsTrue(db.HasChanges());
            db.Write();
            Assert.IsFalse(db.HasChanges());

            db = null;

            db = LoadDatabase("test-write");
            Assert.AreEqual(1, db.FileCount());
            Assert.AreEqual("1", db.Files.First().ResolvedFileName);
        }
                
        /// <summary>
        /// Ensures the PathType of FullPath works
        /// </summary>
        [TestMethod]
        public void PathTypeFullPath()
        {
            Database db = LoadDatabase("test-write", pathType: ChecksumVerifier.PathType.FullPath);
            db.AddFile(GetCorpusFile("simple", "1"), GetCorpusDirectory("simple"), PathType.FullPath, ChecksumType.MD5);

            Assert.IsTrue(db.HasFile(GetCorpusFile("simple", "1")));

            // read back the DB and make sure it's the same
            db.Write();
            db = null;

            db = LoadDatabase("test-write", pathType: ChecksumVerifier.PathType.FullPath);
            Assert.IsTrue(db.HasFile(GetCorpusFile("simple", "1")));
        }
                        
        /// <summary>
        /// Ensures the PathType of FullPathNoDrive works
        /// </summary>
        [TestMethod]
        public void PathTypeFullPathNoDrive()
        {
            Database db = LoadDatabase("test-write", pathType: ChecksumVerifier.PathType.FullPathNoDrive);
            db.AddFile(GetCorpusFile("simple", "1"), GetCorpusDirectory("simple"), PathType.FullPathNoDrive, ChecksumType.MD5);

            Assert.IsTrue(db.HasFile(GetCorpusFile("simple", "1").Substring(3)));

            // read back the DB and make sure it's the same
            db.Write();
            db = null;

            db = LoadDatabase("test-write", pathType: ChecksumVerifier.PathType.FullPathNoDrive);
            Assert.IsTrue(db.HasFile(GetCorpusFile("simple", "1").Substring(3)));
        }
    }
}
