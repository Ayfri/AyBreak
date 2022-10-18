using System;
using System.Diagnostics;
using System.IO;

namespace Breakout;

internal static class LevelManager {
	private const string LevelsFile = "levels.txt";

	static LevelManager() {
		Debug.WriteLine("Levels:");

		for (var index = 0; index < Levels.Length; index++) {
			var level = Levels[index];
			Debug.WriteLine($"Level {index} :\n" + level);
		}
	}

	public static string[] Levels => File.ReadAllText(LevelsFile).Split(new[] { ";;;;;;;;;;;;;;;;;;;;;" }, StringSplitOptions.RemoveEmptyEntries);

	public static string LoadLevel(int index) => Levels[index];
}
