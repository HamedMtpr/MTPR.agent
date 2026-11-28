namespace MTPR.Agent.Core.Models;

/// <summary>
/// A single message of a conversation.
/// </summary>
/// <param name="Role">Author role of the message.</param>
/// <param name="Content">Text content of the message.</param>
public sealed record ChatMessage(ChatRole Role, string Content);
