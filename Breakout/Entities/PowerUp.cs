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
	ScoreMultiplier,
	Random
}

public sealed class PowerUp : PictureBox {
	private const double Speed = .4;
	private readonly PowerUpType _type;
	private readonly double _value;
	private static readonly int[] ScoreValues = { 200, 500, 1000 };
	private static readonly Random Random = new();

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
			PowerUpType.Random => Resources.poweruprandom,
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid powerup type.")
		};
	}

	public static PowerUp GeneratePowerUp(GameScene game) {
		var powerUpCount = typeof(PowerUpType).GetEnumValues().Length;

		PowerUpType powerUpType;
		bool invalid;

		do {
			powerUpType = (PowerUpType) Random.Next(powerUpCount);

			invalid = powerUpType switch {
				PowerUpType.BallNoClip => game.IsNoClip,
				PowerUpType.BallSpeedUp => game.BallSpeedMultiplier >= 1.8,
				PowerUpType.BallSpeedDown => game.BallSpeedMultiplier <= .4,
				PowerUpType.IncreasePaddleSize => game.Paddle.Width >= game.ClientSize.Width / 2,
				PowerUpType.MoreBall => game.BallCount >= 4,
				PowerUpType.MoreLife => false,
				PowerUpType.ScoreMultiplier => false,
				PowerUpType.Random => false,
				_ => throw new ArgumentOutOfRangeException()
			};
		} while (invalid);


		return new(powerUpType, GeneratePowerUpValue(powerUpType));
	}

	private static double GeneratePowerUpValue(PowerUpType powerUpType) => powerUpType switch {
		PowerUpType.BallNoClip => 1,
		PowerUpType.BallSpeedUp => Random.NextDouble() * .2,
		PowerUpType.BallSpeedDown => Random.NextDouble() * .2,
		PowerUpType.IncreasePaddleSize => Random.Next(10, 50),
		PowerUpType.MoreBall => 1,
		PowerUpType.MoreLife => 1,
		PowerUpType.ScoreMultiplier => ScoreValues[Random.Next(ScoreValues.Length - 1)],
		PowerUpType.Random => 0,
		_ => throw new ArgumentOutOfRangeException()
	};

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
			
			case PowerUpType.Random:
				PowerUp randomPowerUp;
				do {
					randomPowerUp = GeneratePowerUp(collisionPayloadPayload.Game);
				} while (randomPowerUp._type == PowerUpType.Random);
				
				randomPowerUp.Apply(collisionPayloadPayload);
				break;

			default: throw new ArgumentOutOfRangeException();
		}
	}
}
