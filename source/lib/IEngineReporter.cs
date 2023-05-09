// <copyright file="IEngineReporter.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

using System.Collections.ObjectModel;

/// <summary>
/// Reports on Engine progress
/// </summary>
public interface IEngineReporter
{
    /// <summary>
    /// The Engine is loading the database from the XML file
    /// </summary>
    void LoadingDatabaseFromFile();

    /// <summary>
    /// The Engine has loaded the database from the XML file
    /// </summary>
    /// <param name="fileCount">File count</param>
    void LoadingDatabaseFromFileCompleted(int fileCount);

    /// <summary>
    /// The Engine is updating the checksums database
    /// </summary>
    /// <param name="fileCount">Number of files it already has in the database</param>
    void UpdatingChecksums(int fileCount);

    /// <summary>
    /// The Engine has completed updating the database
    /// </summary>
    /// <param name="hadChanges">Whether or not there were changes to the database</param>
    void UpdatingChecksumsCompleted(bool hadChanges);

    /// <summary>
    /// The Engine has found a new file and is about to calculate its checksum
    /// </summary>
    /// <param name="fileName">File name</param>
    void AddingFile(string fileName);

    /// <summary>
    /// The Engine has calculated the checksum and has added the file to the database
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="checksum">The file's checksum</param>
    void AddingFileCompleted(string fileName, string checksum);

    /// <summary>
    /// The Engine has removed a file that is missing from the disk
    /// </summary>
    /// <param name="fileName">File name</param>
    void RemovedFile(string fileName);

    /// <summary>
    /// The Engine is writing the database to disk
    /// </summary>
    /// <param name="xmlFileName">XML file name</param>
    void WritingDatabase(string xmlFileName);

    /// <summary>
    /// The Engine has completed writing the database to disk
    /// </summary>
    void WritingDatabaseCompleted();

    /// <summary>
    /// The Engine is verifying the checksums of files in the database
    /// </summary>
    /// <param name="fileCount">Number of files in the database</param>
    void VerifyingChecksums(int fileCount);

    /// <summary>
    /// The Engine is verifying the specified file
    /// </summary>
    /// <param name="current">Current file index</param>
    /// <param name="max">Total number of files to verify</param>
    /// <param name="fileName">File name</param>
    void VerifyingFile(int current, int max, string fileName);
        
    /// <summary>
    /// The Engine has completed verifying all files in the database
    /// </summary>
    /// <param name="badFiles">A list of bad files</param>
    void VerifyingChecksumsCompleted(ReadOnlyCollection<BadFile> badFiles);

    /// <summary>
    /// The Engine updated a file's Checksum
    /// </summary>
    /// <param name="filePath">File updated</param>
    void UpdatedFile(string filePath);
}