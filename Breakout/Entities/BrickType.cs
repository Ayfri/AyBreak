namespace Breakout;

using System;
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
