﻿// <copyright file="EngineTests.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier.Test;

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
        var engine = CreateEngine("simple");

        engine.ScanFiles(false);

        var db = engine.Database;

        Assert.AreEqual(2, db.FileCount());

        var files = new List<FileChecksum>(db.Files);
            
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
        var engine = CreateEngine("simple");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }
        
    /// <summary>
    /// Tests a simple database of two files, should fail because of the wrong Checksum type
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsSimpleFailWrongChecksumType()
    {
        var engine = CreateEngine("simple", checksumType: ChecksumType.SHA1);
            
        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(2, result.BadFiles.Count);
    }

    /// <summary>
    /// Tests a simple database SHA1
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsSHA1()
    {
        var engine = CreateEngine("sha1", checksumType: ChecksumType.SHA1);

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }
        
    /// <summary>
    /// Tests a simple database SHA256
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsSHA256()
    {
        var engine = CreateEngine("sha256", checksumType: ChecksumType.SHA256);

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }

    /// <summary>
    /// Tests a simple database SHA512
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsSHA512()
    {
        var engine = CreateEngine("sha512", checksumType: ChecksumType.SHA512);

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }
        
    /// <summary>
    /// Tests a recursive database
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsRecursive()
    {
        var engine = CreateEngine("subdirs", matchType: MatchType.Directory);

        engine.ScanFiles(true);

        var result = engine.VerifyChecksums(false, false, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }
                
    /// <summary>
    /// Tests with an exclusion pattern
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsExclusionPattern()
    {
        var engine = CreateEngine("exclusion", excludePattern: "*.foo");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }
                        
    /// <summary>
    /// Tests with a match pattern
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsMatchPattern()
    {
        var engine = CreateEngine("match", matchPattern: "*.foo");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.BadFiles.Count);
    }

    /// <summary>
    /// Tests a database with bad checksums (X)
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsBadChecksum()
    {
        var engine = CreateEngine("badchecksum");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

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
        var engine = CreateEngine("missing");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

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
        var engine = CreateEngine("foo");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, false);

        Assert.IsFalse(result.Success);
    }
                        
    /// <summary>
    /// Tests VerifyChecksums when being notified of new files
    /// </summary>
    [TestMethod]
    public void VerifyChecksumsNewFiles()
    {
        var engine = CreateEngine("newfiles");

        engine.ScanFiles(false);

        var result = engine.VerifyChecksums(false, false, true);

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
        var engine = CreateEngine("update");

        engine.UpdateChecksums(false, true, true);
    }

    /// <summary>
    /// Tests UpdateChecksums
    /// </summary>
    [TestMethod]
    public void UpdateChecksums()
    {
        var engine = CreateEngine("update");

        engine.ScanFiles(false);

        var result = engine.UpdateChecksums(false, true, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.FilesUpdated);
    }

    /// <summary>
    /// Tests UpdateChecksums when ignoring new files
    /// </summary>
    [TestMethod]
    public void UpdateChecksumsIgnoreNew()
    {
        var engine = CreateEngine("newfiles");

        engine.ScanFiles(false);

        var result = engine.UpdateChecksums(true, true, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.FilesUpdated);
    }
        
    /// <summary>
    /// Tests UpdateChecksums when not ignoring new files
    /// </summary>
    [TestMethod]
    public void UpdateChecksumsDoNotIgnoreNew()
    {
        var engine = CreateEngine("newfiles");

        engine.ScanFiles(false);

        var result = engine.UpdateChecksums(false, false, true);

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
        var engine = CreateEngine("update", matchPattern: "1", matchType: MatchType.File);

        engine.ScanFiles(false);

        var result = engine.UpdateChecksums(false, false, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.FilesUpdated);
    }
            
    /// <summary>
    /// Tests UpdateChecksums using a single directory
    /// </summary>
    [TestMethod]
    public void UpdateChecksumsMatchTypeDirectory()
    {
        var engine = CreateEngine("update", matchPattern: ".", matchType: MatchType.Directory);

        engine.ScanFiles(false);

        var result = engine.UpdateChecksums(false, false, true);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.FilesUpdated);
    }
    
    /// <summary>
    /// Tests UpdateChecksums with missing files
    /// </summary>
    [TestMethod]
    public void UpdateChecksumsMissing()
    {
        var engine = CreateEngine("updatemissing");

        engine.ScanFiles(false);

        Assert.IsTrue(engine.Database.HasFile("2"));

        var result = engine.UpdateChecksums(false, true, true);

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
        var engine = CreateEngine("simple");

        engine.VerifyChecksums(false, true, false);
    }
}