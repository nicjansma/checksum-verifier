// <copyright file="EngineTests.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Engine class
    /// </summary>
    [TestClass]
    public class EngineTests : TestBase
    {
        /// <summary>
        /// Tests a simple database of two files
        /// </summary>
        [TestMethod]
        public void Database()
        {
            Engine engine = CreateEngine("simple");

            engine.ScanFiles(false);

            Database db = engine.Database;

            Assert.AreEqual(2, db.FileCount());

            List<FileChecksum> files = new List<FileChecksum>(db.Files);
            
            Assert.AreEqual(2, files.Count);

            Assert.AreEqual("1", files[0].ResolvedFileName);
            Assert.AreEqual(TestConstants.Corpus1MD5Checksum, files[0].Checksum);

            Assert.AreEqual("2", files[1].ResolvedFileName);
            Assert.AreEqual(TestConstants.Corpus2MD5Checksum, files[1].Checksum);
        }

        /// <summary>
        /// Tests a simple database of two files
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsSimple()
        {
            Engine engine = CreateEngine("simple");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }
        
        /// <summary>
        /// Tests a simple database of two files, should fail because of the wrong Checksum type
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsSimpleFailWrongChecksumType()
        {
            Engine engine = CreateEngine("simple", checksumType: ChecksumType.SHA1);
            
            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(2, result.BadFiles.Count);
        }

        /// <summary>
        /// Tests a simple database SHA1
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsSHA1()
        {
            Engine engine = CreateEngine("sha1", checksumType: ChecksumType.SHA1);

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }
        
        /// <summary>
        /// Tests a simple database SHA256
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsSHA256()
        {
            Engine engine = CreateEngine("sha256", checksumType: ChecksumType.SHA256);

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }

        /// <summary>
        /// Tests a simple database SHA512
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsSHA512()
        {
            Engine engine = CreateEngine("sha512", checksumType: ChecksumType.SHA512);

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }
        
        /// <summary>
        /// Tests a recursive database
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsRecursive()
        {
            Engine engine = CreateEngine("subdirs", matchType: MatchType.Directory);

            engine.ScanFiles(true);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }
                
        /// <summary>
        /// Tests with an exclusion pattern
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsExclusionPattern()
        {
            Engine engine = CreateEngine("exclusion", excludePattern: "*.foo");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }
                        
        /// <summary>
        /// Tests with a match pattern
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsMatchPattern()
        {
            Engine engine = CreateEngine("match", matchPattern: "*.foo");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.BadFiles.Count);
        }

        /// <summary>
        /// Tests a database with bad checksums (X)
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsBadChecksum()
        {
            Engine engine = CreateEngine("badchecksum");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(2, result.BadFiles.Count);

            Assert.AreEqual("1", result.BadFiles[0].FileName);
            Assert.AreEqual(BadFileType.DifferentChecksum, result.BadFiles[0].BadFileType);
            Assert.AreEqual("X", result.BadFiles[0].ChecksumDatabase);
            Assert.AreEqual(TestConstants.Corpus1MD5Checksum, result.BadFiles[0].ChecksumDisk);
            
            Assert.AreEqual("2", result.BadFiles[1].FileName);
            Assert.AreEqual(BadFileType.DifferentChecksum, result.BadFiles[1].BadFileType);
            Assert.AreEqual("X", result.BadFiles[1].ChecksumDatabase);
            Assert.AreEqual(TestConstants.Corpus2MD5Checksum, result.BadFiles[1].ChecksumDisk);
        }
        
        /// <summary>
        /// Tests a database with missing files
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsMissing()
        {
            Engine engine = CreateEngine("missing");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.BadFiles.Count);

            Assert.AreEqual("2", result.BadFiles[0].FileName);
            Assert.AreEqual(BadFileType.Missing, result.BadFiles[0].BadFileType);
        }
        
        /// <summary>
        /// Tests for a base directory that does not exist
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsDirectoryDoesNotExist()
        {
            Engine engine = CreateEngine("foo");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, false);

            Assert.IsFalse(result.Success);
        }
                        
        /// <summary>
        /// Tests VerifyChecksums when being notified of new files
        /// </summary>
        [TestMethod]
        public void VerifyChecksumsNewFiles()
        {
            Engine engine = CreateEngine("newfiles");

            engine.ScanFiles(false);

            VerifyChecksumsResult result = engine.VerifyChecksums(false, false, true);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(3, result.BadFiles.Count);
            Assert.AreEqual("1", result.BadFiles[0].FileName);
            Assert.AreEqual("2", result.BadFiles[1].FileName);
            Assert.AreEqual("3", result.BadFiles[2].FileName);
        }

        /// <summary>
        /// Tests UpdateChecksums
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateChecksumsDidNotScanFilesFirst()
        {
            Engine engine = CreateEngine("update");

            engine.UpdateChecksums(false, true, true);
        }

        /// <summary>
        /// Tests UpdateChecksums
        /// </summary>
        [TestMethod]
        public void UpdateChecksums()
        {
            Engine engine = CreateEngine("update");

            engine.ScanFiles(false);

            UpdateChecksumsResult result = engine.UpdateChecksums(false, true, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.FilesUpdated);
        }

        /// <summary>
        /// Tests UpdateChecksums when ignoring new files
        /// </summary>
        [TestMethod]
        public void UpdateChecksumsIgnoreNew()
        {
            Engine engine = CreateEngine("newfiles");

            engine.ScanFiles(false);

            UpdateChecksumsResult result = engine.UpdateChecksums(true, true, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.FilesUpdated);
        }
        
        /// <summary>
        /// Tests UpdateChecksums when not ignoring new files
        /// </summary>
        [TestMethod]
        public void UpdateChecksumsDoNotIgnoreNew()
        {
            Engine engine = CreateEngine("newfiles");

            engine.ScanFiles(false);

            UpdateChecksumsResult result = engine.UpdateChecksums(false, false, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.FilesUpdated);
            Assert.IsTrue(engine.Database.HasFile("1"));
            Assert.IsTrue(engine.Database.HasFile("2"));
            Assert.IsTrue(engine.Database.HasFile("3"));
        }

        /// <summary>
        /// Tests UpdateChecksums using a single file match
        /// </summary>
        [TestMethod]
        public void UpdateChecksumsMatchTypeFile()
        {
            Engine engine = CreateEngine("update", matchPattern: "1", matchType: MatchType.File);

            engine.ScanFiles(false);

            UpdateChecksumsResult result = engine.UpdateChecksums(false, false, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.FilesUpdated);
        }
            
        /// <summary>
        /// Tests UpdateChecksums using a single directory
        /// </summary>
        [TestMethod]
        public void UpdateChecksumsMatchTypeDirectory()
        {
            Engine engine = CreateEngine("update", matchPattern: ".", matchType: MatchType.Directory);

            engine.ScanFiles(false);

            UpdateChecksumsResult result = engine.UpdateChecksums(false, false, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.FilesUpdated);
        }
    
        /// <summary>
        /// Tests UpdateChecksums with missing files
        /// </summary>
        [TestMethod]
        public void UpdateChecksumsMissing()
        {
            Engine engine = CreateEngine("updatemissing");

            engine.ScanFiles(false);

            Assert.IsTrue(engine.Database.HasFile("2"));

            UpdateChecksumsResult result = engine.UpdateChecksums(false, true, true);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.FilesUpdated);
            Assert.IsFalse(engine.Database.HasFile("2"));
        }
        
        /// <summary>
        /// Tests VerifyChecksums without ScanFiles first
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyChecksumsDidNotScanFilesFirst()
        {
            Engine engine = CreateEngine("simple");

            engine.VerifyChecksums(false, true, false);
        }
    }
}
