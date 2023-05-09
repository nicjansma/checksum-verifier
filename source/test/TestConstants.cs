// <copyright file="TestConstants.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier.Test;

/// <summary>
/// Test constants
/// </summary>
public static class TestConstants
{
    /// <summary>
    /// Corpus file with the contents of "1"'s MD5 checksum
    /// </summary>
    public static readonly string Corpus1MD5Checksum = "a5ea0ad9260b1550a14cc58d2c39b03d";

    /// <summary>
    /// Corpus file with the contents of "2"'s MD5 checksum
    /// </summary>
    public static readonly string Corpus2MD5Checksum = "c81e728d9d4c2f636f067f89cc14862c";

    /// <summary>
    /// Corpus file with the contents of "1"'s SHA512 checksum
    /// </summary>
    public static readonly string Corpus1SHA512Checksum = "a5ea0ad9260b1550a14cc58d2c39b03d";

    /// <summary>
    /// Corpus file with the contents of "2"'s SHA512 checksum
    /// </summary>
    public static readonly string Corpus2SHA512Checksum = "c81e728d9d4c2f636f067f89cc14862c";
}