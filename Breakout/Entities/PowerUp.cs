using Breakout.Properties;

namespace Breakout.Entities;

using System;
using System.Windows.Forms;

public enum PowerUpType {
	BallNoClip,
	BallSpeedUp,
	BallSpeedDown,
	IncreasePaddleSize,
	MoreBall,
	MoreLife,
	ScoreMultiplier
}

public sealed class PowerUp : PictureBox {
	private const double Speed = .4;
	private readonly PowerUpType _type;
	private readonly double _value;

	public PowerUp(PowerUpType type, double value) {
		_type = type;
		_value = value;

		Width = 45;
		Height = 24;
		SizeMode = PictureBoxSizeMode.StretchImage;

		Image = type switch {
			PowerUpType.BallNoClip => Resources.powerupnoclip,
			PowerUpType.BallSpeedUp => Resources.powerupballaccelerate,
			PowerUpType.BallSpeedDown => Resources.powerupballslow,
			PowerUpType.IncreasePaddleSize => Resources.poweruppaddlelarge,
			PowerUpType.MoreBall => Resources.powerupball,
			PowerUpType.MoreLife => Resources.poweruplife,
			PowerUpType.ScoreMultiplier => value switch {
				200 => Resources.powerup200,
				500 => Resources.powerup500,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid value for score multiplier powerup.")
			},
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid powerup type.")
		};
	}

	public new bool Move(int deltaTime) {
		Top += (int) (deltaTime * Speed);
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
			
			case PowerUpType.BallSpeedDown:
				collisionPayloadPayload.Game.BallSpeedMultiplier -= _value;
				break;

			case PowerUpType.IncreasePaddleSize:
				collisionPayloadPayload.Game.Paddle.Width += (int) _value;
				break;

			case PowerUpType.MoreBall:
				collisionPayloadPayload.Game.AddBall();
				break;

			case PowerUpType.MoreLife:
				collisionPayloadPayload.Game.Lives += (int) _value;
				break;

			case PowerUpType.ScoreMultiplier:
				collisionPayloadPayload.Game.Score += (int) _value;
				break;

			default: throw new ArgumentOutOfRangeException();
		}
	}
}
