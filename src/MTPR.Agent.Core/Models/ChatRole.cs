namespace MTPR.Agent.Core.Models;

/// <summary>
/// Author role of a chat message.
/// </summary>
public enum ChatRole
{
    /// <summary>System instructions that guide the model.</summary>
    System,

    /// <summary>Message written by the user.</summary>
    User,

    /// <summary>Message produced by the model.</summary>
    Assistant,
}
