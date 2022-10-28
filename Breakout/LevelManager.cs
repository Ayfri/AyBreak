namespace Breakout;

using System;
using System.IO;

/// <summary>
///     The Level record structure contains information about a level.
/// </summary>
public record struct Level {
	/// <summary>
	///     The layout of the bricks in the level as a string.
	/// </summary>
	public string Layout;

	/// <summary>
	///     The index of the Level.
	/// </summary>
	public int Number;
}

/// <summary>
///     The static LevelManager class contains methods for loading levels from files.
/// </summary>
public static class LevelManager {
	/// <summary>
	///     The path to the levels file.
	/// </summary>
	private const string LevelsFile = "levels.txt";

	/// <summary>
	///     The getter for the levels array.
	/// </summary>
	public static string[] Levels => File.ReadAllText(LevelsFile)
		.Split(
			new[] {
				";;;;;;;;;;;;;;;;;;;;;"
			},
			StringSplitOptions.RemoveEmptyEntries
		);

	/// <summary>
	///     Loads a level from the levels file.
	/// </summary>
	/// <param name="index"> The index of the level to load. </param>
	/// <returns> The level loaded from the file. </returns>
	public static Level LoadLevel(int index) => new() {
		Layout = Levels[index],
		Number = index
	};
}
