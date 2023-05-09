// <copyright file="ProgramAction.cs" company="Nic Jansma">
// Copyright (c) 2014 Nic Jansma All Right Reserved
// </copyright>
// <author>Nic Jansma</author>

namespace ChecksumVerifier.Console;

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