// <copyright file="DatabaseFile.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the serialized form of a Database
    /// </summary>
    [XmlRoot("db")]
    public class DatabaseFile
    {        
        /// <summary>
        /// Gets or sets the list of FileChecksums
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Only used for serialization, must be public")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Only used for serialization, must be public")]
        [XmlArray("files")]
        [XmlArrayItem("file", typeof(FileChecksum))]
        public List<FileChecksum> Files { get; set; }
    }
}
