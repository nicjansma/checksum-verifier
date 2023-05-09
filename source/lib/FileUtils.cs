﻿// <copyright file="FileUtils.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// File utilities
/// </summary>
public static class FileUtils
{
    /// <summary>
    /// Gets all files, ignoring UnauthorizedAccessException's
    /// </summary>
    /// <param name="basePath">Base path</param>
    /// <param name="exclude">Excluding pattern</param>
    /// <param name="match">Match pattern</param>
    /// <param name="recurse">Recursive into subdirectories</param>
    /// <returns>List of files found</returns>
    public static IEnumerable<string> GetFilesRecursive(string basePath, string exclude, string match, bool recurse)
    {
        var newFiles = new List<string>();

        try
        {
            // add all files in this path
            newFiles.AddRange(GetFilesInDirectory(basePath, exclude, match));

            if (recurse)
            {
                foreach (var subDir in GetDirectories(basePath))
                {
                    // add all files in subdirs
                    var newPath = Path.Combine(basePath, subDir);                    
                    newFiles.AddRange(GetFilesRecursive(newPath, exclude, match, recurse));
                }    
            }
        }
        catch (UnauthorizedAccessException)
        {
            // ignore any paths we can't get into
        }
            
        return newFiles;
    }

    /// <summary>
    /// Gets all files in the specified directory
    /// </summary>
    /// <param name="directory">Directory to scan</param>
    /// <param name="exclude">Exclude pattern</param>
    /// <param name="match">Match pattern</param>
    /// <returns>List of files in the specified directory</returns>
    public static string[] GetFilesInDirectory(string directory, string exclude, string match)
    {
        if (!DirectoryExistsLong(directory))
        {
            return Array.Empty<string>();
        }

        try
        {
            // gets all files in this directory
            var files = GetFiles(directory, match, SearchOption.TopDirectoryOnly);

            // now we have to apply our custom exlcusion patterns
            if (!string.IsNullOrEmpty(exclude))
            {
                //
                // Create a RegEx from a simple glob
                //
                var mask = new Regex(exclude
                    .Replace(".", "[.]")
                    .Replace("*", ".*")
                    .Replace("?", "."));

                return files.Where(file => !mask.IsMatch(file)).ToArray();
            }

            return files;
        }
        catch (UnauthorizedAccessException)
        {
            // ignore any paths we can't get into
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Gets a file's MD5 checksum
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="checksumType">Checksum type</param>
    /// <returns>File's checksum</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Reliability", 
        "CA2000:Dispose objects before losing scope",
        Justification = "We manually .Clear() and .Dispose() of hasher in cleanup due to .Dispose() being private on HashAlgorithm")]
    public static string GetFileChecksum(string fileName, ChecksumType checksumType)
    {
        HashAlgorithm hasher = null;
        string hash;

        try
        {
            switch (checksumType)
            {
                case ChecksumType.MD5:
                    hasher = MD5.Create();
                    break;
                    
                case ChecksumType.SHA1:
                    hasher = SHA1.Create();
                    break;
                    
                case ChecksumType.SHA256:
                    hasher = SHA256.Create();
                    break;

                case ChecksumType.SHA512:
                    hasher = SHA512.Create();
                    break;
            }

            try
            {
                // compute hash from file
                using var fileReader = new StreamReader(fileName);
                hash = ByteHashToString(hasher.ComputeHash(fileReader.BaseStream));
            }
            catch (IOException)
            {
                // try again with long path
                using var fileReader = new StreamReader(@"\\?\" + fileName);
                hash = ByteHashToString(hasher.ComputeHash(fileReader.BaseStream));
            }
        }
        catch (Exception)
        {
            hash = string.Empty;
        }

        if (hasher == null)
            return hash;

        // Clear() "Releases all resources used by the HashAlgorithm class"
        hasher.Clear();

        // NOTE: HashAlgorithm.Dispose is private, so let's cast to IDisposable anyways and it'll work
        ((IDisposable)hasher).Dispose();

        return hash;
    }
        
    /// <summary>
    /// Determines if the file exists (supporting Long Paths)
    /// </summary>
    /// <param name="path">File Path</param>
    /// <returns>True if the file exists</returns>
    public static bool ExistsLong(string path) => File.Exists(path) || File.Exists(@"\\?\" + path);

    /// <summary>
    /// Determines if the directory exists (supporting Long Paths)
    /// </summary>
    /// <param name="path">File Path</param>
    /// <returns>True if the directory exists</returns>
    public static bool DirectoryExistsLong(string path) => Directory.Exists(path) || Directory.Exists(@"\\?\" + path);

    /// <summary>
    /// Gets all of the directories under a path
    /// </summary>
    /// <param name="path">File Path</param>
    /// <returns>Directories under the path</returns>
    public static string[] GetDirectories(string path)
    {
        string[] directories;

        try
        {
            directories = Directory.GetDirectories(path);
        }
        catch (IOException)
        {
            // try again with long path
            directories = Directory.GetDirectories(@"\\?\" + path).Select(s => s.Replace(@"\\?\", string.Empty)).ToArray();
        }

        return directories;
    }

    /// <summary>
    /// Convert a hash from bytes to a string
    /// </summary>
    /// <param name="hash">Byte array of hash</param>
    /// <returns>String representation of hash</returns>
    private static string ByteHashToString(byte[] hash)
    {
        var str = new StringBuilder();
        foreach (var t in hash)
        {
            str.Append(t.ToString("x2", CultureInfo.InvariantCulture));
        }

        return str.ToString();
    }

    /// <summary>
    /// Gets all matching files
    /// </summary>
    /// <param name="dir">Directory location</param>
    /// <param name="match">Files match</param>
    /// <param name="searchOption">Search options</param>
    /// <returns>Matching files</returns>
    private static string[] GetFiles(string dir, string match, SearchOption searchOption)
    {
        string[] files;

        try
        {
            files = Directory.GetFiles(dir, match, searchOption);
        }
        catch (IOException)
        {
            // try again with long path
            files = Directory.GetFiles(@"\\?\" + dir, match, searchOption).Select(s => s.Replace(@"\\?\", string.Empty)).ToArray();
        }

        return files;
    }
}