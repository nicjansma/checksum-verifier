// <copyright file="VerifyChecksumsResult.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

using System.Collections.ObjectModel;

/// <summary>
/// The results of an Engine.VerifyChecksums() call
/// </summary>
public class VerifyChecksumsResult
{
    /// <summary>
    /// Whether or not the result was a success
    /// </summary>
    private readonly bool _success;

    /// <summary>
    /// Bad files
    /// </summary>
    private readonly ReadOnlyCollection<BadFile> _badFiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyChecksumsResult" /> class.
    /// </summary>
    /// <param name="success">Whether or not the update was a success</param>
    /// <param name="badFiles">List of bad files</param>
    public VerifyChecksumsResult(bool success, ReadOnlyCollection<BadFile> badFiles)
    {
        _success = success;
        _badFiles = badFiles;
    }

    /// <summary>
    /// Gets a value indicating whether or not the Verify was a success
    /// </summary>
    public bool Success => _success;

    /// <summary>
    /// Gets the collection of BadFiles found during the Verify
    /// </summary>
    public ReadOnlyCollection<BadFile> BadFiles => _badFiles;
}