// <copyright file="Engine.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>

using System.Linq;

namespace ChecksumVerifier;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Checksum engine
/// </summary>
public class Engine
{                              
    /// <summary>
    /// The base path of the files
    /// </summary>
    private readonly string _basePath;

    /// <summary>
    /// The exclude pattern for files
    /// </summary>
    private readonly string _excludePattern;

    /// <summary>
    /// The match pattern for files
    /// </summary>
    private readonly string _matchPattern;

    /// <summary>
    /// The match type for files
    /// </summary>
    private readonly MatchType _matchType;

    /// <summary>
    /// The path type of files
    /// </summary>
    private readonly PathType _pathType;
        
    /// <summary>
    /// The checksum type to use
    /// </summary>
    private readonly ChecksumType _checksumType;

    /// <summary>
    /// The checksum Database
    /// </summary>
    private readonly Database _db;

    /// <summary>
    /// The reporter to use for any progress updates
    /// </summary>
    private readonly IEngineReporter _reporter = new NullReporter();

    /// <summary>
    /// Files to check
    /// </summary>
    private List<string> _filesToCheck = null;
        
    /// <summary>
    /// XML file database path
    /// </summary>
    private readonly string _xmlDatabasePath;

    /// <summary>
    /// List of bad files
    /// </summary>
    private List<BadFile> _badFiles = new List<BadFile>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Engine" /> class
    /// </summary>
    /// <param name="xmlDatabasePath">XML database path</param>
    /// <param name="reporter">Reporter to use</param>
    /// <param name="basePath">Base path</param>
    /// <param name="excludePattern">Exclude pattern</param>
    /// <param name="matchPattern">Match pattern</param>
    /// <param name="matchType">Match type</param>
    /// <param name="pathType">Path type</param>
    /// <param name="checksumType">Checksum type</param>
    public Engine(
        string xmlDatabasePath, 
        IEngineReporter reporter,
        string basePath,
        string excludePattern = "",
        string matchPattern = "*",
        MatchType matchType = MatchType.Wildcard,
        PathType pathType = PathType.RelativePath,
        ChecksumType checksumType = ChecksumType.MD5)
    {
        _xmlDatabasePath = xmlDatabasePath;
        _reporter = reporter;
        _basePath = basePath;
        _excludePattern = excludePattern;
        _matchPattern = matchPattern;
        _matchType = matchType;
        _pathType = pathType;
        _checksumType = checksumType;

        _db = Database.FromFile(_xmlDatabasePath, _basePath, _pathType);
    }

    /// <summary>
    /// Gets the Checksum Database
    /// </summary>
    public Database Database => _db;

    /// <summary>
    /// Scans the file system for files to check that match the criteria.
    /// <para />
    /// Should be called prior to UpdateChecksums() or VerifyChecksums() to scan for files on the file system.
    /// </summary>
    /// <param name="recurse">Whether or not to scan directories recursively</param>
    public void ScanFiles(bool recurse)
    {
        _reporter.LoadingDatabaseFromFile();

        _filesToCheck = new List<string>();

        switch (_matchType)
        {
            //
            // single file to match
            //
            case MatchType.File:
                _filesToCheck.Add(_matchPattern);
                break;
            case MatchType.Wildcard:
                //
                // wildcard list of files
                //
                _filesToCheck.AddRange(FileUtils.GetFilesRecursive(_basePath, _excludePattern, _matchPattern, false));
                break;
            case MatchType.Directory:
                //
                // add a directory
                //
                _filesToCheck.AddRange(FileUtils.GetFilesRecursive(_basePath, _excludePattern, _matchPattern, recurse));
                break;
        }

        // sort list
        _filesToCheck.Sort();

        _reporter.LoadingDatabaseFromFileCompleted(_filesToCheck.Count);
    }

    /// <summary>
    /// Updates checksums and finds new files
    /// </summary>
    /// <param name="ignoreNew">Ignore new files</param>
    /// <param name="removeMissing">Remove missing files</param>
    /// <param name="pretend">Don't save changes to the database</param>
    /// <param name="updateExisting">Update existing files's checksums</param>
    /// <returns>True on success</returns>
    public UpdateChecksumsResult UpdateChecksums(bool ignoreNew, bool removeMissing, bool pretend, bool updateExisting = false)
    {           
        var filesUpdated = 0;

        if (_filesToCheck == null)
        {
            throw new InvalidOperationException("You must scan files before updating checksums");
        }

        _reporter.UpdatingChecksums(_db.FileCount());

        // if updating existing checksums, do that first
        if (updateExisting)
        {
            foreach (var fc in _db.Files)
            {
                if (!FileUtils.ExistsLong(fc.FilePath))
                    continue;

                // build a list of files to remove first
                // add file to database
                if (!_db.UpdateFile(fc.FilePath, _basePath, _pathType, _checksumType))
                    continue;

                _reporter.UpdatedFile(fc.FilePath);
                filesUpdated++;
            }
        }

        // if adding new files, find those missing in the XML DB
        if (!ignoreNew)
        {
            foreach (var fileName in _filesToCheck)
            {
                var fileNameToCheck = FileChecksum.GetFileName(fileName, _basePath, _pathType);

                if (_db.HasFile(fileNameToCheck))
                    continue;

                _reporter.AddingFile(fileNameToCheck);
                      
                // add file to database
                var checksum = _db.AddFile(fileName, _basePath, _pathType, _checksumType);

                _reporter.AddingFileCompleted(fileNameToCheck, checksum);

                filesUpdated++;
            }
        }

        // determine if there are any files in the DB but not on the disk
        if (removeMissing)
        {
            var filesToRemove = new List<string>();

            foreach (var fc in _db.Files)
            {
                if (FileUtils.ExistsLong(fc.FilePath))
                    continue;

                // build a list of files to remove first
                filesToRemove.Add(fc.ResolvedFileName);
                _reporter.RemovedFile(fc.FilePath);

                filesUpdated++;
            }

            // now actually remove the files
            foreach (var fileToRemove in filesToRemove)
            {
                _db.RemoveFile(fileToRemove);
            }
        }
            
        // write out XML unless we're in pretend mode
        if (pretend)
            return new UpdateChecksumsResult(true, filesUpdated);

        if (_db.HasChanges())
        {
            _reporter.WritingDatabase(_xmlDatabasePath);

            _db.Write();

            _reporter.WritingDatabaseCompleted();
        }
        else
        {
            _reporter.UpdatingChecksumsCompleted(false);
        }

        return new UpdateChecksumsResult(true, filesUpdated);
    }
        
    /// <summary>
    /// Verify the checksums of files listed in the Database
    /// </summary>
    /// <param name="ignoreChecksum">Ignore checksum changes</param>
    /// <param name="ignoreMissing">Ignore missing files</param>
    /// <param name="showNew">Show new files</param>
    /// <returns>Number of bad files found, or -1 for other error</returns>
    public VerifyChecksumsResult VerifyChecksums(bool ignoreChecksum, bool ignoreMissing, bool showNew)
    {
        if (_filesToCheck == null)
        {
            throw new InvalidOperationException("You must scan files before verifying checksums");
        }

        if (!FileUtils.DirectoryExistsLong(_basePath))
        {
            return new VerifyChecksumsResult(false, null);
        }

        _reporter.VerifyingChecksums(_db.FileCount());

        // keep track of files we checked
        var checkedFiles = new Hashtable();

        // list of bad files
        _badFiles = new List<BadFile>();

        // verify the checksums of all of the files in the DB
        var fileChecked = 0;

        // save then cd to basepath
        var previousDirectory = Directory.GetCurrentDirectory();

        Directory.SetCurrentDirectory(_basePath);

        foreach (var fc in _db.Files)
        {
            fileChecked++;

            // display progress
            _reporter.VerifyingFile(fileChecked, _db.FileCount(), fc.ResolvedFileName);

            // keep trace of files we looked at
            checkedFiles.Add(fc.ResolvedFileName, 1);

            // determine if the file is missing from the dist
            if (!FileUtils.ExistsLong(fc.FilePath))
            {
                if (!ignoreMissing)
                {
                    _badFiles.Add(new BadFile() 
                    {
                        FileName = fc.ResolvedFileName,
                        BadFileType = BadFileType.Missing
                    });
                }

                continue;
            }

            if (ignoreChecksum)
                continue;

            // verify the checksum from the disk
            var fileChecksum = new FileChecksum(fc.FilePath, _basePath, _pathType, _checksumType);

            // if the checksum is bad, add bad files
            if (fc.Checksum == fileChecksum.Checksum)
                continue;

            _badFiles.Add(new BadFile() 
            {
                FileName = fc.ResolvedFileName,
                BadFileType = BadFileType.DifferentChecksum,
                ChecksumDisk = fileChecksum.Checksum,
                ChecksumDatabase = fc.Checksum
            });
        }

        // notify the user of new files
        if (showNew)
        {
            foreach (var fileName in _filesToCheck
                         .Select(file => FileChecksum.GetFileName(file, _basePath, _pathType))
                         .Where(fileName => !checkedFiles.ContainsKey(fileName)))
            {
                _badFiles.Add(new BadFile() 
                {
                    FileName = fileName,
                    BadFileType = BadFileType.New,
                });
            }
        }

        var badFiles = _badFiles.AsReadOnly();

        _reporter.VerifyingChecksumsCompleted(badFiles);

        // cd back to previous directory
        Directory.SetCurrentDirectory(previousDirectory);

        return new VerifyChecksumsResult(badFiles.Count == 0, badFiles);
    }
}