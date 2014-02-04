// <copyright file="UpdateChecksumsResult.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
namespace ChecksumVerifier
{
    /// <summary>
    /// The results of an Engine.UpdateChecksum() call
    /// </summary>
    public class UpdateChecksumsResult
    {
        /// <summary>
        /// Whether or not the result was a success
        /// </summary>
        private readonly bool _success;

        /// <summary>
        /// Bad files
        /// </summary>
        private readonly int _filesUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateChecksumsResult" /> class.
        /// </summary>
        /// <param name="success">Whether or not the update was a success</param>
        /// <param name="filesUpdated">Number of files updated</param>
        public UpdateChecksumsResult(bool success, int filesUpdated)
        {
            _success = success;
            _filesUpdated = filesUpdated;
        }

        /// <summary>
        /// Gets a value indicating whether or not the Update was a success
        /// </summary>
        public bool Success
        {
            get
            {
                return _success;
            }
        }

        /// <summary>
        /// Gets the number of files Updated
        /// </summary>
        public int FilesUpdated
        {
            get
            {
                return _filesUpdated;
            }
        }
    }
}
