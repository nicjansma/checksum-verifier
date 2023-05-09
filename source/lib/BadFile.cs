// <copyright file="BadFile.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

/// <summary>
/// Represents a file that is "bad" - missing, bad checksum, etc
/// </summary>
public class BadFile
{
    /// <summary>
    /// Gets or sets the type of the file
    /// </summary>
    public BadFileType BadFileType { get; set; }

    /// <summary>
    /// Gets or sets the file name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the file's Checksum in the database
    /// </summary>
    public string ChecksumDatabase { get; set; }
        
    /// <summary>
    /// Gets or sets the file's Checksum on disk
    /// </summary>
    public string ChecksumDisk { get; set; }
}