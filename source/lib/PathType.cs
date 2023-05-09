// <copyright file="PathType.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

/// <summary>
/// Type of path to store/start from
/// </summary>
public enum PathType
{
    /// <summary>
    /// Relative path
    /// </summary>
    RelativePath,

    /// <summary>
    /// Full file path
    /// </summary>
    FullPath,

    /// <summary>
    /// Full path with no drive letter
    /// </summary>
    FullPathNoDrive
}