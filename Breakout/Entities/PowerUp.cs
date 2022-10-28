namespace Breakout.Entities;

using System;
using System.Windows.Forms;
using Breakout.Properties;

/// <summary>
///     An enum for the different types of powerups.
/// </summary>
public enum PowerUpType {
	BallNoClip,
	BallSpeedUp,
	BallSpeedDown,
	IncreasePaddleSize,
	MoreBall,
	MoreLife,
	ScoreAdd,
	Random
}

/// <summary>
///     The Control class for the PowerUp class.
/// </summary>
public sealed class PowerUp : PictureBox {
	/// <summary>
	///     The speed at which the powerup falls.
	/// </summary>
	private const double Speed = .4;

	/// <summary>
	///     The score values possible for the ScoreAdd powerup.
	/// </summary>
	private static readonly int[] ScoreValues = {
		200, 500, 1000
	};

	/// <summary>
	///     A Random object used for randomizing the powerup.
	/// </summary>
	private static readonly Random Random = new();

	/// <summary>
	///     The type of powerup.
	/// </summary>
	private readonly PowerUpType _type;

	/// <summary>
	///     The value of the powerup.
	/// </summary>
	private readonly double _value;

	/// <summary> The PowerUp function creates a new PowerUp object. </summary>
	/// <param name="type"> The type of powerup to create. </param>
	/// <param name="value"> The value of the powerup. </param>
	/// <returns> An object of type powerup. </returns>
	private PowerUp(PowerUpType type, double value) {
		_type = type;
		_value = value;

		Width = 52;
		Height = 32;
		SizeMode = PictureBoxSizeMode.StretchImage;

		Image = type switch {
			PowerUpType.BallNoClip => Resources.powerupnoclip,
			PowerUpType.BallSpeedUp => Resources.powerupballaccelerate,
			PowerUpType.BallSpeedDown => Resources.powerupballslow,
			PowerUpType.IncreasePaddleSize => Resources.poweruppaddlelarge,
			PowerUpType.MoreBall => Resources.powerupball,
			PowerUpType.MoreLife => Resources.poweruplife,
			PowerUpType.ScoreAdd => value switch {
				200 => Resources.powerup200,
				500 => Resources.powerup500,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid value for score multiplier powerup.")
			},
			PowerUpType.Random => Resources.poweruprandom,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid powerup type.")
		};
	}

	/// <summary> The GeneratePowerUp function generates a random power up. </summary>
	/// <param name="game"> The game. </param>
	/// <returns> A powerup struct with a type and value. </returns>
	/// <summary> The GeneratePowerUp function generates a random power up. </summary>
	/// <param name="game"> The game. </param>
	/// <returns> A powerup object. </returns>
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
				PowerUpType.ScoreAdd => false,
				PowerUpType.Random => false,
				_ => throw new ArgumentOutOfRangeException()
			};
		} while (invalid);

		return new(powerUpType, GeneratePowerUpValue(powerUpType));
	}

	/// <summary>
	///     The GeneratePowerUpValue function generates a random value for the powerup.
	/// </summary>
	/// <param name="powerUpType"> The type of powerup to generate a value for. </param>
	/// <returns> A random value for the powerup. </returns>
	/// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
	private static double GeneratePowerUpValue(PowerUpType powerUpType) => powerUpType switch {
		PowerUpType.BallNoClip => 1,
		PowerUpType.BallSpeedUp => Random.NextDouble() * .2,
		PowerUpType.BallSpeedDown => Random.NextDouble() * .2,
		PowerUpType.IncreasePaddleSize => Random.Next(10, 50),
		PowerUpType.MoreBall => 1,
		PowerUpType.MoreLife => 1,
		PowerUpType.ScoreAdd => ScoreValues[Random.Next(ScoreValues.Length - 1)],
		PowerUpType.Random => 0,
		_ => throw new ArgumentOutOfRangeException()
	};

	/// <summary> The Move function moves the object by a given amount of time. </summary>
	/// <param name="deltaTime"> How much time has passed since the last frame? </param>
	/// <returns> A bool value. if the move function returns true, then the object has moved off of the screen and is ready to be removed. </returns>
	public new bool Move(int deltaTime) {
		Top += (int) (deltaTime * Speed);
		return Bottom > Parent.Height;
	}

	/// <summary> The Apply function applies the power up to the game. </summary>
	/// <param name="collisionPayloadPayload"> The payload that contains the game. </param>
	/// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
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

			case PowerUpType.ScoreAdd:
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