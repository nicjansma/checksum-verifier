// <copyright file="Database.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>

using System.Linq;

namespace ChecksumVerifier;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// File/checksum database
/// </summary>
public class Database
{
    /// <summary>
    /// Whether or not the database has changes
    /// </summary>
    private bool _hasChanges = false;

    /// <summary>
    /// A Dictionary of file names to FileChecksums
    /// </summary>
    private readonly Dictionary<string, FileChecksum> _fileLookup = new Dictionary<string, FileChecksum>();
        
    /// <summary>
    /// Initializes a new instance of the Database class
    /// </summary>
    public Database()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Database class
    /// </summary>
    /// <param name="xmlFilePath">XML file name of the database</param>
    public Database(string xmlFilePath)
    {
        XmlFileName = xmlFilePath;
    }
        
    /// <summary>
    /// Gets or sets the database's XML file name
    /// </summary>
    public string XmlFileName { get; set; }

    /// <summary>
    /// Gets a list of Files in the Database
    /// </summary>
    public ICollection<FileChecksum> Files =>
        // return a clone so it can be iterated on
        new Dictionary<string, FileChecksum>(_fileLookup).Values;

    /// <summary>
    /// Loads a Database from a XML file
    /// </summary>
    /// <param name="fileName">XML file name</param>
    /// <param name="basePath">Base path</param>
    /// <param name="pathType">Path type</param>
    /// <returns>New Database</returns>
    public static Database FromFile(string fileName, string basePath, PathType pathType)
    {
        // ensure file exists first
        if (!FileUtils.ExistsLong(fileName))
        {
            // otherwise, return a new empty Database
            return new Database(fileName);
        }

        // ready serializer
        var serializerNameSpace = new XmlSerializerNamespaces();
        serializerNameSpace.Add(string.Empty, string.Empty);
        var serializer = new XmlSerializer(typeof(DatabaseFile));

        // load all elements from the XML
        DatabaseFile databaseFile = null;
        using (var reader = new StreamReader(fileName))
        {
            try
            {
                databaseFile = (DatabaseFile)serializer.Deserialize(reader);
            }
            catch (XmlException)
            {
                // let callers handle error
            }
            catch (InvalidOperationException)
            {
                // let callers handle error
            }
        }

        if (databaseFile == null)
        {
            return null;
        }

        var db = new Database()
        {
            XmlFileName = fileName
        };

        // initialize all elements into the hash table
        foreach (var fileChecksum in databaseFile.Files.Where(
                     fileChecksum => !string.IsNullOrEmpty(fileChecksum.ResolvedFileName) &&
                                     !string.IsNullOrEmpty(fileChecksum.Checksum)))
        {
            // FilePath 
            fileChecksum.InitFromXml(basePath, pathType);

            // add to the database's lookup table
            db._fileLookup.Add(fileChecksum.ResolvedFileName, fileChecksum);
        }

        return db;
    }

    /// <summary>
    /// Calculates the file's checksum and adds it to the Database
    /// </summary>
    /// <param name="fileName">File to add</param>
    /// <param name="basePath">Base path</param>
    /// <param name="pathType">Path type</param>
    /// <param name="checksumType">Checksum type</param>
    /// <returns>Checksum of the file</returns>
    public string AddFile(string fileName, string basePath, PathType pathType, ChecksumType checksumType)
    {
        _hasChanges = true;

        // load file checksum
        var fc = new FileChecksum(fileName, basePath, pathType, checksumType);

        // add to database
        _fileLookup.Add(fc.ResolvedFileName, fc);

        // return the checksum
        return fc.Checksum;
    }

    /// <summary>
    /// Updates a file's checksum
    /// </summary>
    /// <param name="fileName">File to update</param>
    /// <param name="basePath">Base path</param>
    /// <param name="pathType">Path type</param>
    /// <param name="checksumType">Checksum type</param>
    /// <returns>True if the file was updated</returns>
    public bool UpdateFile(string fileName, string basePath, PathType pathType, ChecksumType checksumType)
    {
        // load file checksum
        var fc = new FileChecksum(fileName, basePath, pathType, checksumType);

        if (!_fileLookup.ContainsKey(fc.ResolvedFileName))
            return false;

        // determine if the checksums are different
        if (_fileLookup[fc.ResolvedFileName].Checksum == fc.Checksum)
            return false;

        _fileLookup[fc.ResolvedFileName] = fc;

        _hasChanges = true;

        return true;
    }

    /// <summary>
    /// Determines if the file is in the database
    /// </summary>
    /// <param name="fileName">File name to check</param>
    /// <returns>True if the file is in the database or not</returns>
    public bool HasFile(string fileName) => _fileLookup.ContainsKey(fileName);

    /// <summary>
    /// Gets the number of files
    /// </summary>
    /// <returns>Number of files</returns>
    public int FileCount() => _fileLookup.Count;

    /// <summary>
    /// Gets the specified file's FileChecksum
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <returns>The specified file's FileChecksum, or null, if it doesn't exist</returns>
    public FileChecksum GetFile(string fileName) => _fileLookup[fileName];

    /// <summary>
    /// Removes a file from the Database
    /// </summary>
    /// <param name="fileName">File name to remove</param>
    public void RemoveFile(string fileName)
    {
        if (!HasFile(fileName))
            return;

        _hasChanges = true;

        _fileLookup.Remove(fileName);
    }

    /// <summary>
    /// Checks if the database has changes that need to be written
    /// </summary>
    /// <returns>Whether the database has changes or not</returns>
    public bool HasChanges() => _hasChanges;

    /// <summary>
    /// Writes the Database to an XML file
    /// </summary>
    public void Write()
    {
        // initialize writer settings
        var writerSettings = new XmlWriterSettings
        {
            Indent = true
        };

        // ready serializer
        var serializerNameSpace = new XmlSerializerNamespaces();
        serializerNameSpace.Add(string.Empty, string.Empty);
        var serializer = new XmlSerializer(typeof(DatabaseFile));

        // create a new XML writer
        using (var writer = XmlWriter.Create(XmlFileName, writerSettings))
        {
            writer.WriteStartDocument(true);

            var databaseFile = new DatabaseFile()
            {
                Files = new List<FileChecksum>(_fileLookup.Values)
            };

            serializer.Serialize(writer, databaseFile, serializerNameSpace);

            writer.WriteEndDocument();

            writer.Flush();
        }

        _hasChanges = false;
    }
}