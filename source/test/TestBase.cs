// <copyright file="TestBase.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier.Test
{
    using System;
    using System.Globalization;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// ChecksumVerifier test base class
    /// </summary>
    [TestClass]
    public abstract class TestBase
    {
        /// <summary>
        /// Initialization before a test method is called
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // remove "test-write.xml" database
            if (File.Exists(GetCorpusDatabase("test-write")))
            {
                File.Delete(GetCorpusDatabase("test-write"));
            }

            // remove "update.xml" database
            if (File.Exists(GetCorpusDatabase("update")))
            {
                File.Delete(GetCorpusDatabase("update"));
            }
        }

        /// <summary>
        /// Gets the corpus directory file path
        /// </summary>
        /// <param name="subdirectory">Subdirectory under corpus</param>
        /// <returns>Corpus directory file path</returns>
        protected static string GetCorpusDirectory(string subdirectory)
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "corpus", subdirectory);
        }
        
        /// <summary>
        /// Gets a file in the corpus directory
        /// </summary>
        /// <param name="subdirectory">Subdirectory under corpus</param>
        /// <param name="fileName">File name in the subdirectory</param>
        /// <returns>File in the corpus directory</returns>
        protected static string GetCorpusFile(string subdirectory, string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "corpus", subdirectory, fileName);
        }

        /// <summary>
        /// Gets the corpus database path
        /// </summary>
        /// <param name="subdirectory">Subdirectory under corpus</param>
        /// <returns>Corpus database path</returns>
        protected static string GetCorpusDatabase(string subdirectory)
        {
            return Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), 
                String.Format(CultureInfo.InvariantCulture, @"corpus\{0}.xml", subdirectory));
        }

        /// <summary>
        /// Creates an Database from the specified file under corpus\.
        /// </summary>
        /// <param name="subdirectory">Subdirectory of corpus\ to use</param>
        /// <param name="pathType">Path type</param>
        /// <returns>Checksum Database</returns>
        protected static Database LoadDatabase(string subdirectory, PathType pathType = PathType.RelativePath)
        {
            return Database.FromFile(GetCorpusDatabase(subdirectory), GetCorpusDirectory(subdirectory), pathType);
        }

        /// <summary>
        /// Creates an engine of the specified directory under corpus\.
        /// <para />
        /// The database .xml file should be in corpus\, named the same as the subdirectory.
        /// </summary>
        /// <param name="subdirectory">Subdirectory of corpus\ to use</param>
        /// <param name="excludePattern">Exclude pattern</param>
        /// <param name="matchPattern">Match pattern</param>
        /// <param name="matchType">Match type</param>
        /// <param name="pathType">Path type</param>
        /// <param name="checksumType">Checksum type</param>
        /// <returns>Checksum Engine</returns>
        protected static Engine CreateEngine(
            string subdirectory, 
            string excludePattern = "",
            string matchPattern = "*",
            MatchType matchType = MatchType.Wildcard,
            PathType pathType = PathType.RelativePath,
            ChecksumType checksumType = ChecksumType.MD5)
        {
            Engine engine = new Engine(
                GetCorpusDatabase(subdirectory), 
                new NullReporter(), 
                GetCorpusDirectory(subdirectory), 
                excludePattern, 
                matchPattern, 
                matchType, 
                pathType, 
                checksumType);           

            return engine;
        }
    }
}
