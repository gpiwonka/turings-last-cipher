namespace TuringsLastCipher.Core.Scenes;

/// <summary>A navigable option on a scene. <see cref="Next"/> is the id of the scene it leads to.</summary>
public record Choice(string Text, string Next);
