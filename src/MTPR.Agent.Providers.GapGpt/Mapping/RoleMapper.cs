using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Providers.GapGpt.Mapping;

/// <summary>
/// Converts core chat roles to GapGPT role names.
/// </summary>
internal static class RoleMapper
{
    /// <summary>
    /// Maps a chat role to its GapGPT name.
    /// </summary>
    /// <param name="role">Core chat role.</param>
    /// <returns>The role name used by the API.</returns>
    public static string Map(ChatRole role)
    {
        return role switch
        {
            ChatRole.System => "system",
            ChatRole.User => "user",
            ChatRole.Assistant => "assistant",
            _ => throw new ArgumentOutOfRangeException(nameof(role),
                role,
                "Unsupported chat role."),
        };
    }
}
