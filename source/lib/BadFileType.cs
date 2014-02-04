// <copyright file="BadFileType.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier
{
    /// <summary>
    /// Represents the type of a "bad" file
    /// </summary>
    public enum BadFileType
    {
        /// <summary>
        /// The file is missing
        /// </summary>
        Missing,

        /// <summary>
        /// The file has a different checksum than what's in the database
        /// </summary>
        DifferentChecksum,

        /// <summary>
        /// The file is new
        /// </summary>
        New
    }
}
