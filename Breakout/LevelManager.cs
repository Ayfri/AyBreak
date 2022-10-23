namespace Breakout;

using System;
using System.IO;

public record struct Level {
	public string Layout;
	public int Number;
}

internal static class LevelManager {
	private const string LevelsFile = "levels.txt";

	public static string[] Levels => File.ReadAllText(LevelsFile).Split(new[] { ";;;;;;;;;;;;;;;;;;;;;" }, StringSplitOptions.RemoveEmptyEntries);

	public static Level LoadLevel(int index) => new() {
		Layout = Levels[index],
		Number = index
	};
}
