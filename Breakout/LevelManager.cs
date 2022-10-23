namespace Breakout;

using System;
using System.IO;

internal static class LevelManager {
	private const string LevelsFile = "levels.txt";

	public static string[] Levels => File.ReadAllText(LevelsFile).Split(new[] { ";;;;;;;;;;;;;;;;;;;;;" }, StringSplitOptions.RemoveEmptyEntries);

	public static string LoadLevel(int index) => Levels[index];
}
