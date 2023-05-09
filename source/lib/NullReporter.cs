// <copyright file="NullReporter.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

using System.Collections.ObjectModel;

/// <summary>
/// A null output Engine reporter
/// </summary>
public class NullReporter : IEngineReporter
{
    /// <inheritdoc />
    public void LoadingDatabaseFromFile()
    {
    }

    /// <inheritdoc />
    public void LoadingDatabaseFromFileCompleted(int fileCount)
    {
    }

    /// <inheritdoc />
    public void UpdatingChecksums(int fileCount)
    {
    }

    /// <inheritdoc />
    public void UpdatingChecksumsCompleted(bool hadChanges)
    {
    }

    /// <inheritdoc />
    public void AddingFile(string fileName)
    {
    }

    /// <inheritdoc />
    public void AddingFileCompleted(string fileName, string checksum)
    {
    }

    /// <inheritdoc />
    public void RemovedFile(string fileName)
    {
    }

    /// <inheritdoc />
    public void WritingDatabase(string xmlFileName)
    {
    }

    /// <inheritdoc />
    public void WritingDatabaseCompleted()
    {
    }

    /// <inheritdoc />
    public void VerifyingChecksums(int fileCount)
    {
    }

    /// <inheritdoc />
    public void VerifyingFile(int current, int max, string fileName)
    {
    }

    /// <inheritdoc />
    public void VerifyingChecksumsCompleted(ReadOnlyCollection<BadFile> badFiles)
    {
    }

    /// <inheritdoc />
    public void UpdatedFile(string filePath)
    {
    }
}