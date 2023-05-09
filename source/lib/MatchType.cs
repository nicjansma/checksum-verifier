// <copyright file="MatchType.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

/// <summary>
/// What files to match
/// </summary>
public enum MatchType
{
    /// <summary>
    /// Match files
    /// </summary>
    File,

    /// <summary>
    /// Match directory
    /// </summary>
    Directory,

    /// <summary>
    /// Match wildcard
    /// </summary>
    Wildcard
}