using TuringsLastCipher.Core.Ciphers;
using TuringsLastCipher.Core.Game;
using TuringsLastCipher.Core.Scenes;
using Xunit;

namespace TuringsLastCipher.Core.Tests;

public class StoryAndSessionTests
{
    private static Story BuildStory()
    {
        var scenes = new Scene[]
        {
            new("intro", "A letter arrives.", Choices: new[] { new Choice("Open it", "puzzle1") }),
            new("puzzle1", "Decode the message.",
                Puzzle: new Puzzle("p1", new CipherSpec(CipherKind.Caesar, Shift: 3), "MEET AT NOON", "ending")),
            new("ending", "The truth settles in.")
        };
        return new Story(scenes, "intro");
    }

    [Fact]
    public void Valid_story_reports_no_problems()
        => Assert.Empty(BuildStory().Validate());

    [Fact]
    public void Validation_flags_dangling_links_and_unsolvable_puzzles()
    {
        var scenes = new Scene[]
        {
            new("intro", "Start.", Choices: new[] { new Choice("Go", "nowhere") }),
            new("bad", "Broken puzzle.",
                Puzzle: new Puzzle("pX", new CipherSpec(CipherKind.Caesar, Shift: 3), "HELLO", "ghost"))
        };
        var problems = new Story(scenes, "missing-start").Validate();

        Assert.Contains(problems, p => p.Contains("missing-start"));
        Assert.Contains(problems, p => p.Contains("nowhere"));
        Assert.Contains(problems, p => p.Contains("ghost"));
    }

    [Fact]
    public void Session_walks_choice_then_puzzle_to_the_ending()
    {
        var session = new GameSession(BuildStory());
        Assert.Equal("intro", session.CurrentSceneId);

        Assert.True(session.Choose("puzzle1"));
        Assert.False(session.Choose("ending"), "cannot jump to a scene that is not a declared choice");

        Assert.False(session.TrySolve("WRONG ANSWER"));
        Assert.Equal("puzzle1", session.CurrentSceneId);

        // The player solves the Caesar-3 message; case and spacing are forgiven by the oracle.
        Assert.True(session.TrySolve("meet at noon"));
        Assert.Equal("ending", session.CurrentSceneId);
        Assert.True(session.Current.IsEnding);
        Assert.Contains("p1", session.SolvedPuzzleIds);
    }

    [Fact]
    public void Ciphertext_shown_to_player_is_not_the_plaintext()
    {
        var puzzle = new Puzzle("p1", new CipherSpec(CipherKind.Caesar, Shift: 3), "MEET AT NOON", "ending");
        Assert.Equal("PHHW DW QRRQ", puzzle.Ciphertext);
        Assert.True(puzzle.IsSolvedBy("Meet At Noon"));
    }

    [Fact]
    public void Json_round_trips_into_a_valid_walkable_story()
    {
        const string json = """
        {
          "start": "intro",
          "scenes": [
            {
              "id": "intro",
              "chapter": 1,
              "text": "A letter arrives, postmarked June 21st.",
              "choices": [ { "text": "Open the letter", "next": "puzzle1" } ]
            },
            {
              "id": "puzzle1",
              "chapter": 1,
              "text": "The page is scrambled.",
              "puzzle": {
                "id": "p1",
                "cipher": { "kind": "Vigenere", "key": "ENIGMA" },
                "plaintext": "I AM NOT WHO YOU THINK",
                "next": "ending",
                "brief": "A faded note."
              }
            },
            { "id": "ending", "chapter": 4, "text": "Was it ever really him?" }
          ]
        }
        """;

        var story = StoryJson.Parse(json);
        Assert.Empty(story.Validate());

        var puzzleScene = story.Get("puzzle1");
        Assert.Equal(CipherKind.Vigenere, puzzleScene.Puzzle!.Cipher.Kind);
        Assert.True(puzzleScene.Puzzle.IsSolvedBy("i am not who you think"));

        // Chapter defaults drive the assistant's trust: Ch1 truthful, Ch4 withholding.
        Assert.Equal(HintPolicy.Truthful, story.Get("intro").EffectiveHintPolicy);
        Assert.Equal(HintPolicy.Withholding, story.Get("ending").EffectiveHintPolicy);
        Assert.True(story.Get("ending").IsEnding);
    }
}
