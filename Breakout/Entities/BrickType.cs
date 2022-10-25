namespace Breakout;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using Entities;

public sealed class CollisionPayload {
	public Ball Ball;
	public Brick Brick;
	public Point BrickPosition;
	public GameScene Game = null!;
	public Side Side;
}

public sealed class BrickType {
	private static readonly Timer _timer = new(30) { Enabled = true };
	private static readonly List<Brick> _bricksGoingToBeHit = new();

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

	public Action<CollisionPayload>? OnCollision;
	public Func<CollisionPayload, bool>? ValidateHit;
	private BrickType() => MaxHealthColor = Color;
	private string Name { get; set; } = "";
	private Color Color { get; set; }
	private Color MaxHealthColor { get; set; }

	public int Score { get; private set; }

	public int MaxHealth { get; private set; } = 1;

	public bool IsEmpty => Name == "Empty";

	// return gradient between max health color and current health color, gradiant is 0 to 256
	// when currentHealth is 1, Color is returned, when health is MaxHealth, MaxHealthColor is returned
	public Color GetColor(int currentHealth) {
		if (currentHealth == 1 || MaxHealth == 1) return Color;
		if (currentHealth == MaxHealth) return MaxHealthColor;
		var r = (Color.R * (MaxHealth - currentHealth) + MaxHealthColor.R * currentHealth) / MaxHealth;
		var g = (Color.G * (MaxHealth - currentHealth) + MaxHealthColor.G * currentHealth) / MaxHealth;
		var b = (Color.B * (MaxHealth - currentHealth) + MaxHealthColor.B * currentHealth) / MaxHealth;

		return Color.FromArgb(r, g, b);
	}
}
