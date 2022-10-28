namespace Breakout.Entities;

using System;
using System.Drawing;
using System.Windows.Forms;
using Breakout.Properties;

/// <summary> A Control for controlling the Ball. </summary>
public sealed class Ball : PictureBox {
	private static readonly Random Random = new();
	public PointF Velocity;

	/// <summary> The Ball function creates a new Ball object. </summary>
	public Ball() {
		Image = Resources.ball;
		Width = Image.Width;
		Height = Image.Height;
	}

	/// <summary> Is the ball waiting to be launched. </summary>
	public bool Waiting { get; private set; }

	/// <summary> The speed of the ball. </summary>
	public double Speed { get; set; }

	/// <summary> The MoveToPaddle function moves the ball to the center of a paddle. </summary>
	/// <param name="paddle"> The paddle to check against </param>
	public void MoveToPaddle(Control paddle) {
		Location = new(paddle.CenterX() - Width / 2, paddle.Top - Height);
	}

	/// <summary> The Move function moves the object by a given amount of time. </summary>
	/// <param name="deltaTime"> Time since last frame </param>
	public new void Move(int deltaTime) {
		Left += (int) Math.Round(Velocity.X * Speed * deltaTime);
		Top += (int) Math.Round(Velocity.Y * Speed * deltaTime);
	}

	/// <summary> The Reset function resets the ball's position and velocity. </summary>
	public void Reset() {
		Waiting = true;
		Velocity = new(0, 0);
	}

	/// <summary> The LaunchBallFromPaddle function is used to launch the ball from the paddle. </summary>
	public void LaunchBallFromPaddle() {
		Waiting = false;
		const int delta = 15;
		var angle = Random.Next(180 + delta, 360 - delta);
		Velocity = new((float) Math.Cos(angle * Math.PI / 180d), (float) Math.Sin(angle * Math.PI / 180d));
	}
}