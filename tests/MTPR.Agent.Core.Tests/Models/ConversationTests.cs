using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class ConversationTests : UnitTest
{
    [Fact]
    public void CreateRequest_Should_PutSystemInstructionFirst()
    {
        var conversation = new Conversation("Be concise.");

        conversation.AddUserMessage("Hi");

        var actual = conversation.CreateRequest("model-a");

        actual.Messages.Should().HaveCount(2);
        actual.Messages[0].Should().Be(new ChatMessage(ChatRole.System, "Be concise."));
        actual.Messages[1].Should().Be(new ChatMessage(ChatRole.User, "Hi"));
    }

    [Fact]
    public void CreateRequest_Should_OmitSystemMessage_When_NoInstructionProvided()
    {
        var conversation = new Conversation();

        conversation.AddUserMessage("Hi");

        var actual = conversation.CreateRequest("model-a");

        actual.Messages.Should().ContainSingle();
    }

    [Fact]
    public void CreateRequest_Should_KeepMessagesInOrder()
    {
        var conversation = new Conversation();

        conversation.AddUserMessage("Question");
        conversation.AddAssistantMessage("Answer");
        conversation.AddUserMessage("Follow-up");

        var actual = conversation.CreateRequest("model-a", 0.2, 100);

        actual.Messages.Select(message => message.Role)
            .Should().Equal(ChatRole.User, ChatRole.Assistant, ChatRole.User);
        actual.Temperature.Should().Be(0.2);
        actual.MaxOutputTokens.Should().Be(100);
    }

    [Fact]
    public void Messages_Should_ExposeAddedMessages()
    {
        var conversation = new Conversation();

        conversation.AddUserMessage("Hi");

        var actual = conversation.Messages;

        actual.Should().Equal(new ChatMessage(ChatRole.User, "Hi"));
    }

    [Fact]
    public void AddUserMessage_Should_Throw_When_ContentIsBlank()
    {
        var conversation = new Conversation();

        var actual = () => conversation.AddUserMessage(" ");

        actual.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateRequest_Should_Throw_When_ModelIsBlank()
    {
        var conversation = new Conversation();

        var actual = () => conversation.CreateRequest(" ");

        actual.Should().Throw<ArgumentException>();
    }
}
