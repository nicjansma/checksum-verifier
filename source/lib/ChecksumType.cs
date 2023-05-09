// <copyright file="ChecksumType.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

/// <summary>
/// Checksum type
/// </summary>
public enum ChecksumType
{
    /// <summary>
    /// MD5 hash
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MD")]
    MD5,
        
    /// <summary>
    /// SHA-1 hash
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA")]
    SHA1,

    /// <summary>
    /// SHA-2 256 bits hash
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA")]
    SHA256,

    /// <summary>
    /// SHA-2 512 bits hash
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA")]
    SHA512
}