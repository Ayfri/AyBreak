namespace Breakout;

using System;
using System.Collections.Generic;
using System.Drawing;
using Breakout.Entities;

public sealed class CollisionPayload {
	public Ball Ball;
	public Brick Brick;
	public Point BrickPosition;
	public GameScene Game = null!;
	public Side Side;
}

public sealed class BrickType {
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
		}
	};

	public Action<CollisionPayload>? OnCollision;
	public BrickType() => MaxHealthColor = Color;
	public string Name { get; set; } = "";
	public Color Color { get; set; }
	public Color MaxHealthColor { get; set; }

	public int Score { get; set; }

	public int MaxHealth { get; set; } = 1;

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