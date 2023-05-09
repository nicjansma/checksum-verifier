// <copyright file="Program.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>

using System;
using System.Globalization;
using System.IO;

namespace ChecksumVerifier.Console;

/// <summary>
/// ChecksumVerifier program
/// </summary>
public static class Program
{
    /// <summary>
    /// Action to take
    /// </summary>
    private static ProgramAction _action = ProgramAction.Invalid;

    /// <summary>
    /// Match string (glob)
    /// </summary>
    private static string _matchPattern = "*";

    /// <summary>
    /// Exclude string (glob)
    /// </summary>
    private static string _excludePattern = string.Empty;

    /// <summary>
    /// Base path
    /// </summary>
    private static string _basePath = Environment.CurrentDirectory;

    /// <summary>
    /// Match type
    /// </summary>
    private static MatchType _matchType = MatchType.File;

    /// <summary>
    /// Path type (relative, etc)
    /// </summary>
    private static PathType _pathType = PathType.RelativePath;

    /// <summary>
    /// Checksum type
    /// </summary>
    private static ChecksumType _checksumType = ChecksumType.MD5;

    /// <summary>
    /// XML file database name
    /// </summary>
    private static string _xmlFileName;

    /// <summary>
    /// Recursive directories
    /// </summary>
    private static bool _recurse = false;

    //
    // arguments: -verify
    //

    /// <summary>
    /// Ignore missing files
    /// </summary>
    private static bool _verifyIgnoreMissing = false;

    /// <summary>
    /// Show new files
    /// </summary>
    private static bool _verifyShowNew = false;

    /// <summary>
    /// Ignore checksum
    /// </summary>
    private static bool _verifyIgnoreChecksum = false;

    //
    // arguments: -update
    //

    /// <summary>
    /// Remove missing files
    /// </summary>
    private static bool _updateRemoveMissing = false;

    /// <summary>
    /// Don't add new files
    /// </summary>
    private static bool _updateIgnoreNew = false;

    /// <summary>
    /// Pretend but don't update
    /// </summary>
    private static bool _updatePretend = false;

    /// <summary>
    /// Updates existing files
    /// </summary>
    private static bool _updateExisting = false;

    /// <summary>
    /// ChecksumVerifier startup
    /// </summary>
    /// <param name="args">Argument list</param>
    /// <returns>0 if successful</returns>
    public static int Main(string[] args)
    {
        //
        // check usage
        //
        if (!CheckArguments(args))
        {
            return 1;
        }
            
        //
        // verify parsed command line arguments
        //
        if (!VerifyArguments())
        {
            return 1;
        }

        //
        // Show what the user selected
        //
        PrintArguments();

        var engine = new Engine(_xmlFileName, new ConsoleReporter(), _basePath, _excludePattern, _matchPattern, _matchType, _pathType, _checksumType);

        // ensure the engine's database is good
        if (engine.Database == null)
        {
            System.Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Error: Could not open database {0}", _xmlFileName));
            return 1;
        }

        //
        // Search path for all files to look at
        //
        engine.ScanFiles(_recurse);

        var returnCode = 0;
        if (_action == ProgramAction.Update)
        {
            //
            // Update checksums
            //
            var result = engine.UpdateChecksums(_updateIgnoreNew, _updateRemoveMissing, _updatePretend, _updateExisting);

            returnCode = result.Success ? 0 : 1;
        }
        else if (_action == ProgramAction.Verify)
        {
            //
            // Verify checksums
            //
            var result = engine.VerifyChecksums(_verifyIgnoreChecksum, _verifyIgnoreMissing, _verifyShowNew);

            returnCode = result.Success ? result.BadFiles.Count : -1;
        }            

        // good finish
        return returnCode;
    }

    /// <summary>
    /// Print out what the user selected
    /// </summary>
    public static void PrintArguments()
    {
        System.Console.WriteLine("Base path:   {0}", _basePath);
        System.Console.WriteLine("Match:       {0}", _matchPattern);
        System.Console.WriteLine("Checksum:    {0}", _checksumType);

        if (!string.IsNullOrEmpty(_excludePattern))
        {
            System.Console.WriteLine("Exclude:     {0}", _excludePattern);
        }

        System.Console.WriteLine();
    }

    /// <summary>
    /// Checks command line arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>True if arguments are good</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Maintainability", 
        "CA1502:AvoidExcessiveComplexity")]
    public static bool CheckArguments(string[] args)
    {
        // show usage if no command line was given
        if (args == null || args.Length == 0)
        {
            Usage(string.Empty);
            return false;
        }

        //
        // loop through all passed in arguments
        //
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToUpperInvariant())
            {
                //
                // action
                //
                case "-UPDATE":
                    _action = ProgramAction.Update;
                    break;

                case "-VERIFY":
                    _action = ProgramAction.Verify;
                    break;

                //
                // db
                //
                case "-DB":
                    _xmlFileName = args[++i];
                    break;

                //
                // generic options
                //
                case "-MATCH":
                    _matchPattern = args[++i];
                    break;

                case "-EXCLUDE":
                    _excludePattern = args[++i];
                    break;

                case "-BASEPATH":
                    _basePath = args[++i];
                    break;

                case "-R":
                case "-RECURSE":
                    _recurse = true;
                    break;

                //
                // path type
                //
                case "-RELATIVEPATH":
                    _pathType = PathType.RelativePath;
                    break;

                case "-FULLPATH":
                    _pathType = PathType.FullPath;
                    break;

                case "-FULLPATHNODRIVE":
                    _pathType = PathType.FullPathNoDrive;
                    break;

                //
                // checksum type
                //
                case "-MD5":
                    _checksumType = ChecksumType.MD5;
                    break;

                case "-SHA1":
                    _checksumType = ChecksumType.SHA1;
                    break;

                case "-SHA256":
                    _checksumType = ChecksumType.SHA256;
                    break;

                case "-SHA512":
                    _checksumType = ChecksumType.SHA512;
                    break;

                //
                // -verify options
                //
                case "-IGNOREMISSING":
                    _verifyIgnoreMissing = true;
                    break;

                case "-SHOWNEW":
                    _verifyShowNew = true;
                    break;

                case "-IGNORECHECKSUM":
                    _verifyIgnoreChecksum = true;
                    break;

                //
                // -update options
                //
                case "-REMOVEMISSING":
                    _updateRemoveMissing = true;
                    break;

                case "-IGNORENEW":
                    _updateIgnoreNew = true;
                    break;

                case "-PRETEND":
                    _updatePretend = true;
                    break;

                case "-UPDATEEXISTING":
                    _updateExisting = true;
                    break;

                default:
                    Usage("Unknown argument: " + args[i]);
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Verify command line arguments
    /// </summary>
    /// <returns>True if arguments are verified</returns>
    public static bool VerifyArguments()
    {
        //
        // Command Line: action
        //
        if (_action == ProgramAction.Invalid)
        {
            Usage("You must either run the -update or -verify action");
            return false;
        }

        //
        // Command Line: match
        //
        if (_matchPattern.Contains("*") || _matchPattern.Contains("?"))
        {
            // if the match is a path, split into path and file match
            if (_matchPattern.Contains(@"\"))
            {
                _basePath   = _matchPattern[.._matchPattern.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase)];
                _matchPattern      = _matchPattern[(_matchPattern.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1)..];
            }

            _matchType = MatchType.Wildcard;
        }
        else if (FileUtils.ExistsLong(_matchPattern))
        {
            _matchType = MatchType.File;
        }
        else if (FileUtils.DirectoryExistsLong(_matchPattern))
        {
            _matchType = MatchType.Directory;
        }
        else
        {
            Usage("File or directory does not exist: " + _matchPattern);
            return false;
        }

        //
        // Command Line: recursion
        //

        // means we're doing directory match
        if (_recurse)
        {
            _matchType = MatchType.Directory;
        }

        //
        // Command Line: XML db
        //            
        //  verify we can open it first
        if (string.IsNullOrEmpty(_xmlFileName))
        {
            Usage("Must specify the XML DB file");
            return false;
        }

        FileStream file;
        if (_action == ProgramAction.Update)
        {
            try
            {
                // only complain if the file exists but we can't read it
                if (FileUtils.ExistsLong(_xmlFileName))
                {
                    file = File.OpenRead(_xmlFileName);
                    file.Close();
                }
            }
            catch (IOException)
            {    
                System.Console.WriteLine("Cannot open XML DB: {0}", _xmlFileName);
                return false;
            }
        }            
        else if (_action == ProgramAction.Verify)
        {
            // complain if the file doesn't exist or we can't read it
            try
            {
                file = File.OpenRead(_xmlFileName);
                file.Close();
            }
            catch (IOException)
            {
                System.Console.WriteLine("Cannot open XML DB: {0}", _xmlFileName);
                return false;
            }
        }    

        //
        // -basePath
        //
        _basePath = _basePath.TrimEnd('\\');
        if (!FileUtils.DirectoryExistsLong(_basePath))
        {
            System.Console.WriteLine("Base path {0} does not exist", _basePath);
            return false;
        }

        var basePathInfo = new FileInfo(_basePath);
        _basePath = basePathInfo.FullName;
       
        return true;
    }

    /// <summary>
    /// Program usage
    /// </summary>
    /// <param name="errorMessage">Error message to display</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming", 
        "CA2204:Literals should be spelled correctly",
        Justification = "Ignore command-line options")]
    public static void Usage(string errorMessage)
    {
        // output error if given
        if (!string.IsNullOrEmpty(errorMessage))
        {
            System.Console.WriteLine("Error: {0}\n", errorMessage);
        }

        // general usage
        System.Console.WriteLine("Usage: ChecksumVerifier.exe [-update | -verify] -db [xml file] [options]");
        System.Console.WriteLine();
        System.Console.WriteLine("actions:");
        System.Console.WriteLine("     -update:                Update checksum database");
        System.Console.WriteLine("     -verify:                Verify checksum database");
        System.Console.WriteLine();
        System.Console.WriteLine("required:");
        System.Console.WriteLine("     -db [xml file]          XML database file");
        System.Console.WriteLine();
        System.Console.WriteLine("options:");
        System.Console.WriteLine("     -match [match]          Files to match (glob pattern such as * or *.jpg or ??.foo) (default: *)");
        System.Console.WriteLine("     -exclude [match]        Files to exclude (glob pattern such as * or *.jpg or ??.foo) (default: empty)");
        System.Console.WriteLine("     -basePath [path]        Base path for matching (default: current directory)");
        System.Console.WriteLine("     -r, -recurse            Recurse (directories only, default: off)");
        System.Console.WriteLine();
        System.Console.WriteLine("path storage options:");
        System.Console.WriteLine("     -relativePath           Relative path (default)");
        System.Console.WriteLine("     -fullPath               Full path");
        System.Console.WriteLine("     -fullPathNodrive        Full path - no drive letter");
        System.Console.WriteLine();
        System.Console.WriteLine("checksum options:");
        System.Console.WriteLine("     -md5                    MD5 (default)");
        System.Console.WriteLine("     -sha1                   SHA-1");
        System.Console.WriteLine("     -sha256                 SHA-2 256 bits");
        System.Console.WriteLine("     -sha512                 SHA-2 512 bits");
        System.Console.WriteLine();
        System.Console.WriteLine("-verify options:");
        System.Console.WriteLine("     -ignoreMissing          Ignore missing files (default: off)");
        System.Console.WriteLine("     -showNew                Show new files (default: off)");
        System.Console.WriteLine("     -ignoreChecksum         Don't calculate checksum (default: off)");
        System.Console.WriteLine();
        System.Console.WriteLine("-update options:");
        System.Console.WriteLine("     -removeMissing          Remove missing files (default: off)");
        System.Console.WriteLine("     -ignoreNew              Don't add new files (default: off)");
        System.Console.WriteLine("     -updateExisting         Verifies the Checksums of all existing files and updates the database on changes (default: off)");
        System.Console.WriteLine("     -pretend                Show what would happen - don't write out XML (default: off)");
    }        
}