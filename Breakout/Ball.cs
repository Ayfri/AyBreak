using System;
using System.Drawing;
using System.Windows.Forms;
using Breakout.Properties;

namespace Breakout;

public class Ball : PictureBox {
	private static readonly Random Random = new();
	public PointF Velocity;


	public Ball() {
		Image = Resources.ball;
		Width = Image.Width;
		Height = Image.Height;
	}

	public bool Waiting { get; private set; }

	public float Speed { get; set; }

	public new void Move(int deltaTime) {
		Left += (int)Math.Round(Velocity.X * Speed * deltaTime);
		Top += (int)Math.Round(Velocity.Y * Speed * deltaTime);
	}

	public void Reset() {
		Waiting = true;
		Velocity = new PointF(0, 0);
	}

	public void LaunchBallFromPaddle() {
		Waiting = false;
		const int delta = 15;
		var angle = Random.Next(180 - delta, 360 - delta);
		Velocity = new PointF((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}
}
