using System;
using System.Drawing;
using System.Windows.Forms;

namespace Breakout.Entities;

public enum PowerUpType {
	BallNoClip,
	BallSpeedUp,
	IncreasePaddleSize,
	MoreBall,
	MoreLife,
	ScoreMultiplier
}

public sealed class PowerUp : PictureBox {
	private readonly PowerUpType _type;
	private readonly double _value;

	private const double Speed = .4;

	public PowerUp(PowerUpType type, double value) {
		_type = type;
		_value = value;
		Width = 25;
		Height = 25;
		BackColor = type switch {
			PowerUpType.BallNoClip => Color.Red,
			PowerUpType.BallSpeedUp => Color.Blue,
			PowerUpType.IncreasePaddleSize => Color.Green,
			PowerUpType.MoreBall => Color.Yellow,
			PowerUpType.MoreLife => Color.Purple,
			PowerUpType.ScoreMultiplier => Color.Orange,
			var _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}

	public new bool Move(int deltaTime) {
		Top += (int)(deltaTime * Speed);
		return Bottom > Parent.Height;
	}

	public void Apply(CollisionPayload collisionPayloadPayload) {
		switch (_type) {
			case PowerUpType.BallNoClip:
				collisionPayloadPayload.Game.NoClip();
				break;

			case PowerUpType.BallSpeedUp:
				collisionPayloadPayload.Game.BallSpeedMultiplier += _value;
				break;

			case PowerUpType.IncreasePaddleSize:
				collisionPayloadPayload.Game.Paddle.Width += (int)_value;
				break;

			case PowerUpType.MoreBall:
				collisionPayloadPayload.Game.AddBall();
				break;

			case PowerUpType.MoreLife:
				collisionPayloadPayload.Game.Lives += (int)_value;
				break;

			case PowerUpType.ScoreMultiplier:
				collisionPayloadPayload.Game.ScoreMultiplier += (int)_value;
				break;

			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
