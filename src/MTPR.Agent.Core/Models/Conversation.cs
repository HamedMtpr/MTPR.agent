namespace MTPR.Agent.Core.Models;

/// <summary>
/// Keeps the ordered messages of a chat together with optional system instructions.
/// </summary>
public sealed class Conversation
{
    private readonly List<ChatMessage> _messages = [];

    /// <summary>
    /// Initializes a new conversation.
    /// </summary>
    /// <param name="systemInstruction">Optional system instructions sent before every request.</param>
    public Conversation(string? systemInstruction = null)
    {
        SystemInstruction = systemInstruction;
    }

    /// <summary>
    /// Gets the system instructions sent before every request.
    /// </summary>
    public string? SystemInstruction { get; }

    /// <summary>
    /// Gets the messages added so far, in order.
    /// </summary>
    public IReadOnlyList<ChatMessage> Messages => _messages;

    /// <summary>
    /// Appends a user message.
    /// </summary>
    /// <param name="content">Text of the message.</param>
    public void AddUserMessage(string content)
    {
        Add(ChatRole.User, content);
    }

    /// <summary>
    /// Appends an assistant message.
    /// </summary>
    /// <param name="content">Text of the message.</param>
    public void AddAssistantMessage(string content)
    {
        Add(ChatRole.Assistant, content);
    }

    /// <summary>
    /// Builds a request containing the system instructions and all messages so far.
    /// </summary>
    /// <param name="model">Identifier of the model to use.</param>
    /// <param name="temperature">Optional sampling temperature.</param>
    /// <param name="maxOutputTokens">Optional upper bound on generated tokens.</param>
    /// <returns>The request ready to be sent to a language model.</returns>
    public LanguageModelRequest CreateRequest(
        string model,
        double? temperature = null,
        int? maxOutputTokens = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        var messages = new List<ChatMessage>();

        if (!string.IsNullOrWhiteSpace(SystemInstruction))
        {
            messages.Add(new ChatMessage(ChatRole.System, SystemInstruction));
        }

        messages.AddRange(_messages);

        return new LanguageModelRequest(model, messages, temperature, maxOutputTokens);
    }

    private void Add(ChatRole role, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        _messages.Add(new ChatMessage(role, content));
    }
}
