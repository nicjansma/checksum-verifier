// <copyright file="ConsoleReporter.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>

using System.Collections.ObjectModel;
using System.Globalization;

namespace ChecksumVerifier.Console;

/// <summary>
/// Reports engine progress to the console
/// </summary>
public class ConsoleReporter : IEngineReporter
{
    /// <summary>
    /// Writes an over-written progress line to the console
    /// </summary>
    /// <param name="format">Format string for line</param>
    /// <param name="list">List of parameters</param>
    public static void WriteConsoleProgressLine(string format, params object[] list)
    {
        var outputString = string.Format(CultureInfo.InvariantCulture, format, list);

        if (System.Console.IsOutputRedirected)
        {
            System.Console.WriteLine(outputString);
        }
        else
        {
            // trim string if needed
            if (outputString.Length > System.Console.BufferWidth - 1)
            {
                outputString = outputString[..(System.Console.BufferWidth - 4)] + "...";
            }
        
            System.Console.CursorLeft = 0;
            System.Console.Write(outputString.PadRight(System.Console.BufferWidth - 1, ' '));
        }
    }

    /// <inheritdoc />
    public void LoadingDatabaseFromFile()
    {
        System.Console.Write("Scanning for matching files on disk... ");
    }

    /// <inheritdoc />
    public void LoadingDatabaseFromFileCompleted(int fileCount)
    {
        System.Console.WriteLine("done.");
        System.Console.WriteLine();

        System.Console.WriteLine("Disk:    {0} files", fileCount);
    }

    /// <inheritdoc />
    public void UpdatingChecksums(int fileCount)
    {
        System.Console.WriteLine("XML DB:  {0} files", fileCount);

        System.Console.WriteLine();
        System.Console.WriteLine("Updating...");
    }

    /// <inheritdoc />
    public void UpdatingChecksumsCompleted(bool hadChanges)
    {
        if (!hadChanges)
        {
            System.Console.WriteLine("No changes required.");
        }
        else
        {
            // NOP
        }
    }

    /// <inheritdoc />
    public void AddingFile(string fileName)
    {
        System.Console.Write("Adding {0}: ", fileName);
    }

    /// <inheritdoc />
    public void AddingFileCompleted(string fileName, string checksum)
    {
        // assume there is a single thread for scanning, and the last file we got is this one
        System.Console.WriteLine(checksum);
    }

    /// <inheritdoc />
    public void RemovedFile(string fileName)
    {
        System.Console.WriteLine("Removing {0} (does not exist)", fileName);
    }

    /// <inheritdoc />
    public void WritingDatabase(string xmlFileName)
    {
        System.Console.Write("Writing {0}... ", xmlFileName);
    }

    /// <inheritdoc />
    public void WritingDatabaseCompleted()
    {
        System.Console.Write("done.\n");
    }

    /// <inheritdoc />
    public void VerifyingChecksums(int fileCount)
    {
        System.Console.WriteLine("XML DB:  {0} files", fileCount);
        System.Console.WriteLine();
        System.Console.WriteLine("Verifying files:");
    }

    /// <inheritdoc />
    public void VerifyingFile(int current, int max, string fileName)
    {
        WriteConsoleProgressLine("[{0,4} / {1,4}] {2}", current, max, fileName);
    }

    /// <inheritdoc />
    public void VerifyingChecksumsCompleted(ReadOnlyCollection<BadFile> badFiles)
    {
        // show final results
        System.Console.WriteLine("\n");
        System.Console.WriteLine("Results:");

        // if we had bad files, notify the user
        if (badFiles == null || badFiles.Count == 0)
        {
            System.Console.WriteLine("\tAll files verified.");
        }
        else
        {
            foreach (var badFile in badFiles)
            {
                switch (badFile.BadFileType)
                {
                    case BadFileType.Missing:
                        System.Console.WriteLine("\tMissing: {0}", badFile.FileName);
                        break;

                    case BadFileType.DifferentChecksum:
                        System.Console.WriteLine("\tMismatch: {0}: {1} (database) vs. {2} (disk)", badFile.FileName, badFile.ChecksumDatabase, badFile.ChecksumDisk);
                        break;

                    case BadFileType.New:
                        System.Console.WriteLine("\tNew: {0}", badFile.FileName);
                        break;
                }
            }
        }
    }

    /// <inheritdoc />
    public void UpdatedFile(string filePath)
    {
        System.Console.WriteLine("Changes in {0}", filePath);
    }
}