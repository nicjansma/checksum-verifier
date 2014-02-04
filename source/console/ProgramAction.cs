// <copyright file="ProgramAction.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>

namespace ChecksumVerifier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Command-line action to take
    /// </summary>
    public enum ProgramAction
    {
        /// <summary>
        /// Invalid Action
        /// </summary>
        Invalid,

        /// <summary>
        /// Update database
        /// </summary>
        Update,

        /// <summary>
        /// Verify database
        /// </summary>
        Verify
    }
}
