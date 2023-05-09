// <copyright file="FileChecksum.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

using System.IO;
using System.Xml.Serialization;

/// <summary>
/// A single file checksum
/// </summary>
[XmlRootAttribute(ElementName = "file")]
public class FileChecksum
{    
    /// <summary>
    /// Initializes a new instance of the <see cref="FileChecksum" /> class
    /// </summary>
    public FileChecksum()
    {
    }

    /// <summary>
    /// Initializes a new instance of the FileChecksum class, and loads the file's hash (computed immediately)
    /// </summary>
    /// <param name="fileName">File to compute hash of</param>
    /// <param name="basePath">Base path</param>
    /// <param name="pathType">Path type</param>
    /// <param name="checksumType">Checksum type</param>
    public FileChecksum(string fileName, string basePath, PathType pathType, ChecksumType checksumType)
    {
        FilePath = fileName;

        ResolvedFileName = GetFileName(FilePath, basePath, pathType);

        Checksum = FileUtils.GetFileChecksum(fileName, checksumType);
    }
        
    /// <summary>
    /// Gets or sets the file's name
    /// </summary>
    /// <value>File's name</value>
    [XmlIgnore]
    public string FilePath
    {
        get;
        set;
    }
                
    /// <summary>
    /// Gets or sets the resolved file name (against the base path, etc)
    /// </summary>
    /// <value>Resolved file name</value>
    [XmlAttribute("name")]
    public string ResolvedFileName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the file's checksum
    /// </summary>
    /// <value>File's checksum</value>
    [XmlAttribute("checksum")]
    public string Checksum
    {
        get;
        set;
    }

    /// <summary>
    /// Returns a file name based on a path type
    /// </summary>
    /// <param name="fileName">File name to look at</param>
    /// <param name="basePath">Base path name</param>
    /// <param name="pathType">Path type to use</param>
    /// <returns>Trimmed (or expanded) file name</returns>
    public static string GetFileName(string fileName, string basePath, PathType pathType)
    {
        var newFileName = fileName;

        var fileInfo = new FileInfo(fileName);

        switch (pathType)
        {
            case PathType.FullPath:
                newFileName = fileInfo.FullName;
                break;

            case PathType.FullPathNoDrive:
                newFileName = fileInfo.FullName[3..];
                break;

            case PathType.RelativePath:
                newFileName = fileInfo.FullName.Replace(basePath + @"\", string.Empty);
                break;
        }

        return newFileName;
    }
        
    /// <summary>
    /// Returns a file path based on a path type
    /// </summary>
    /// <param name="fileName">Resolved file name to look at</param>
    /// <param name="basePath">Base path name</param>
    /// <param name="pathType">Path type to use</param>
    /// <returns>File path</returns>
    public static string GetFilePath(string fileName, string basePath, PathType pathType)
    {
        switch (pathType)
        {
            case PathType.FullPath:
                return fileName;

            case PathType.FullPathNoDrive:
                var basePathInfo = new FileInfo(basePath);
                return Path.Combine(basePathInfo.FullName[..3], fileName);

            case PathType.RelativePath:
            default:
                return Path.Combine(basePath, fileName);
        }
    }

    /// <summary>
    /// Initializes the FileChecksum class after being loaded from XML
    /// </summary>
    /// <param name="basePath">Base path</param>
    /// <param name="pathType">Path type</param>
    public void InitFromXml(string basePath, PathType pathType)
    {
        FilePath = FileChecksum.GetFilePath(ResolvedFileName, basePath, pathType);
    }        
}