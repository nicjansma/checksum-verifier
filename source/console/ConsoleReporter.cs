// <copyright file="ConsoleReporter.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;

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
            string outputString = String.Format(CultureInfo.InvariantCulture, format, list);

            if (Console.IsOutputRedirected)
            {
                Console.WriteLine(outputString);
            }
            else
            {
                // trim string if needed
                if (outputString.Length > Console.BufferWidth - 1)
                {
                    outputString = outputString.Substring(0, Console.BufferWidth - 4) + "...";
                }
        
                Console.CursorLeft = 0;
                Console.Write(outputString.PadRight(Console.BufferWidth - 1, ' '));
            }
        }

        /// <inheritdoc />
        public void LoadingDatabaseFromFile()
        {
            Console.Write("Scanning for matching files on disk... ");
        }

        /// <inheritdoc />
        public void LoadingDatabaseFromFileCompleted(int fileCount)
        {
            Console.WriteLine("done.");
            Console.WriteLine();

            Console.WriteLine("Disk:    {0} files", fileCount);
        }

        /// <inheritdoc />
        public void UpdatingChecksums(int fileCount)
        {
            Console.WriteLine("XML DB:  {0} files", fileCount);

            Console.WriteLine();
            Console.WriteLine("Updating...");
        }

        /// <inheritdoc />
        public void UpdatingChecksumsCompleted(bool hadChanges)
        {
            if (!hadChanges)
            {
                Console.WriteLine("No changes required.");
            }
            else
            {
                // NOP
            }
        }

        /// <inheritdoc />
        public void AddingFile(string fileName)
        {
            Console.Write("Adding {0}: ", fileName);
        }

        /// <inheritdoc />
        public void AddingFileCompleted(string fileName, string checksum)
        {
            // assume there is a single thread for scanning, and the last file we got is this one
            Console.WriteLine(checksum);
        }

        /// <inheritdoc />
        public void RemovedFile(string fileName)
        {
            Console.WriteLine("Removing {0} (does not exist)", fileName);
        }

        /// <inheritdoc />
        public void WritingDatabase(string xmlFileName)
        {
            Console.Write("Writing {0}... ", xmlFileName);
        }

        /// <inheritdoc />
        public void WritingDatabaseCompleted()
        {
            Console.Write("done.\n");
        }

        /// <inheritdoc />
        public void VerifyingChecksums(int fileCount)
        {
            Console.WriteLine("XML DB:  {0} files", fileCount);
            Console.WriteLine();
            Console.WriteLine("Verifying files:");
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
            Console.WriteLine("\n");
            Console.WriteLine("Results:");

            // if we had bad files, notify the user
            if (badFiles == null || badFiles.Count == 0)
            {
                Console.WriteLine("\tAll files verified.");
            }
            else
            {
                foreach (BadFile badFile in badFiles)
                {
                    switch (badFile.BadFileType)
                    {
                        case BadFileType.Missing:
                            Console.WriteLine("\tMissing: {0}", badFile.FileName);
                            break;

                        case BadFileType.DifferentChecksum:
                            Console.WriteLine("\tMismatch: {0}: {1} (database) vs. {2} (disk)", badFile.FileName, badFile.ChecksumDatabase, badFile.ChecksumDisk);
                            break;

                        case BadFileType.New:
                            Console.WriteLine("\tNew: {0}", badFile.FileName);
                            break;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void UpdatedFile(string fileName)
        {
            Console.WriteLine("Changes in {0}", fileName);
        }
    }
}
