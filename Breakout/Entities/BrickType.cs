namespace Breakout;

using System;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
///     The class for a BrickType.
/// </summary>
public sealed class BrickType {
	/// <summary>
	///     The dictionary of all the BrickTypes, mapped by their characters.
	/// </summary>
	public static readonly Dictionary<string, BrickType> BricksTypes = new() {
		{
			" ", new() {
				Name = "Empty",
				Color = Color.Transparent
			}
		}, {
			"-23", new() {
				Name = "Brick",
				Color = Color.Red,
				MaxHealthColor = Color.Orange,
				Score = 10,
				MaxHealth = 3
			}
		}, {
			"*", new() {
				Name = "Explosion",
				Color = Color.Purple,
				Score = 30,
				OnCollision = static collisionPayload => {
					var brickPos = collisionPayload.BrickPosition;
					var game = collisionPayload.Game;

					game.GetBrick(
							brickPos with {
								X = brickPos.X + 1
							}
						)
						?.Hit(game, collisionPayload.Ball, Side.Left);

					game.GetBrick(
							brickPos with {
								X = brickPos.X - 1
							}
						)
						?.Hit(game, collisionPayload.Ball, Side.Right);

					game.GetBrick(
							brickPos with {
								Y = brickPos.Y + 1
							}
						)
						?.Hit(game, collisionPayload.Ball, Side.Top);

					game.GetBrick(
							brickPos with {
								Y = brickPos.Y - 1
							}
						)
						?.Hit(game, collisionPayload.Ball, Side.Bottom);
				}
			}
		}, {
			"x", new() {
				Name = "Indestructible",
				Color = Color.FromArgb(90, 90, 100),
				Score = 0,
				ValidateHit = static _ => false
			}
		}, {
			"@", new() {
				Name = "Semi-Destructible",
				Color = Color.FromArgb(50, 50, 70),
				Score = 20,
				ValidateHit = static collisionPayload => collisionPayload.Game.IsNoClip
			}
		}, {
			"g", new() {
				Name = "Green",
				Color = Color.Green,
				Score = 10
			}
		}, {
			"b", new() {
				Name = "Blue",
				Color = Color.Blue,
				Score = 10
			}
		}, {
			"m", new() {
				Name = "Magenta",
				Color = Color.Magenta,
				Score = 10
			}
		}, {
			"t", new() {
				Name = "Turquoise",
				Color = Color.Turquoise,
				Score = 10
			}
		}, {
			"w", new() {
				Name = "White",
				Color = Color.White,
				Score = 10
			}
		}, {
			"d", new() {
				Name = "Dark",
				Color = Color.FromArgb(150, 150, 170),
				Score = 10
			}
		}
	};

	/// <summary>
	///     The Action to be executed when the brick is hit.
	/// </summary>
	public Action<CollisionPayload>? OnCollision;

	/// <summary>
	///     The Function to be executed when the brick is hit to validate if the hit is valid.
	/// </summary>
	public Func<CollisionPayload, bool>? ValidateHit;

	/// <summary>
	///     The constructor for the BrickType.
	/// </summary>
	private BrickType() => MaxHealthColor = Color;

	/// <summary>
	///     The name of the BrickType.
	/// </summary>
	private string Name { get; set; } = "";

	/// <summary>
	///     The color of the BrickType.
	/// </summary>
	private Color Color { get; set; }

	/// <summary>
	///     The color of the BrickType when it has the maximum health.
	/// </summary>
	private Color MaxHealthColor { get; set; }

	/// <summary>
	///     The score of the BrickType.
	/// </summary>
	public int Score { get; private set; }

	/// <summary>
	///     The maximum health of the BrickType.
	/// </summary>
	public int MaxHealth { get; private set; } = 1;

	/// <summary>
	///     Is the brick is an empty brick.
	/// </summary>
	public bool IsEmpty => Name == "Empty";

	/// <summary>
	///     Generate the color of the brick based on the health.
	/// </summary>
	/// <param name="brickType"> The BrickType to get the color from. </param>
	/// <param name="currentHealth"> The current health of the brick. </param>
	/// <returns> The color of the brick. </returns>
	public static Color GetColor(BrickType brickType, int currentHealth) {
		if (currentHealth == 1 || brickType.MaxHealth == 1) return brickType.Color;
		if (currentHealth == brickType.MaxHealth) return brickType.MaxHealthColor;
		var r = (brickType.Color.R * (brickType.MaxHealth - currentHealth) + brickType.MaxHealthColor.R * currentHealth) / brickType.MaxHealth;
		var g = (brickType.Color.G * (brickType.MaxHealth - currentHealth) + brickType.MaxHealthColor.G * currentHealth) / brickType.MaxHealth;
		var b = (brickType.Color.B * (brickType.MaxHealth - currentHealth) + brickType.MaxHealthColor.B * currentHealth) / brickType.MaxHealth;

		return Color.FromArgb(r, g, b);
	}
}