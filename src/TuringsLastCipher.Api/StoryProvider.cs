using TuringsLastCipher.Core.Scenes;

namespace TuringsLastCipher.Api;

/// <summary>
/// Loads the story content from disk once at startup, validates it, and hands the in-memory
/// <see cref="Story"/> to the endpoints. Validation failures throw so the container fails fast
/// rather than serving a broken, unsolvable game.
/// </summary>
public sealed class StoryProvider
{
    public Story Story { get; }

    public StoryProvider(IConfiguration config, IHostEnvironment env)
    {
        string path = ResolveStoryPath(config, env);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Story content not found at '{path}'.", path);

        Story = StoryJson.Parse(File.ReadAllText(path));

        var problems = Story.Validate();
        if (problems.Count > 0)
            throw new InvalidOperationException(
                "Story content failed validation:" + Environment.NewLine +
                string.Join(Environment.NewLine, problems));
    }

    private static string ResolveStoryPath(IConfiguration config, IHostEnvironment env)
    {
        // STORY_PATH overrides; otherwise look next to the binary, then in the project tree.
        string? configured = config["STORY_PATH"];
        if (!string.IsNullOrWhiteSpace(configured))
            return configured;

        string relative = Path.Combine("content", "scenes", "story.json");
        string beside = Path.Combine(AppContext.BaseDirectory, relative);
        if (File.Exists(beside)) return beside;

        // Dev fallback: walk up from the content root to the repo's content folder.
        return Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", relative));
    }
}
