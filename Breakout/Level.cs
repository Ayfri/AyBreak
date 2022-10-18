using System;
using System.Diagnostics;
using System.IO;
using Breakout.Properties;

namespace Breakout; 

public class Level {
	public readonly int Index;
	public readonly string Layout;

	public Level(int index) {
		Index = index;
		Layout = LevelLoader.LoadLevel(index);
	}
}

static class LevelLoader {
	private const string LevelsFile = "levels.txt";

	// public static string[] Levels => Resources.levels.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
	public static string[] Levels => File.ReadAllText(LevelsFile).Split(new[] { ";;;;;;;;;;;;;;;;;;;;;" }, StringSplitOptions.RemoveEmptyEntries);
	
	public static string LoadLevel(int index) => Levels[index];

	static LevelLoader() {
		Debug.WriteLine("Levels:");

		for (var index = 0; index < Levels.Length; index++) {
			var level = Levels[index];
			Debug.WriteLine($"Level {index} :\n" + level);
		}
	}
}
