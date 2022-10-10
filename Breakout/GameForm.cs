using System;
using System.Timers;
using System.Windows.Forms;

namespace Breakout;

public partial class GameForm : Form {
	private static readonly Random Random = new();
	
	public GameForm() {
		InitializeComponent();
		paddle.Left = ClientSize.Width / 2 - paddle.Width / 2;
		paddle.Top = (int)(ClientSize.Height * .9) - paddle.Height;
		ResetBall();

		const int timerInterval = 1000 / 60;
		timer.Interval = timerInterval;
	}

	/**
	 * The angle in degrees.
	 */
	public int BallAngle { get; set; }

	public int BallSpeed { get; set; } = 12;
	public int PaddleSpeed { get; set; } = 14;

	private bool LeftPressed { get; set; }
	private bool RightPressed { get; set; }

	private void timer1_Elapsed(object sender, ElapsedEventArgs e) {
		
		if (LeftPressed) paddle.Left -= PaddleSpeed;
		if (RightPressed) paddle.Left += PaddleSpeed;
		
		// Move the ball
		ball.Left += (int)(BallSpeed * Math.Cos(BallAngle * Math.PI / 180));
		ball.Top += (int)(BallSpeed * Math.Sin(BallAngle * Math.PI / 180));

		// Check if the ball is out of bounds
		if (ball.Left < 0 || ball.Left > ClientSize.Width - ball.Width) BallAngle = 180 - BallAngle;

		if (ball.Top < 0) BallAngle = 360 - BallAngle;
		if (ball.Top > ClientSize.Height - ball.Height) {
			ResetBall();
			return;
		}

		if (!ball.Bounds.IntersectsWith(paddle.Bounds)) return;

		// if the ball is touching the upper half of the paddle, bounce it up
		if (ball.Top + ball.Height < paddle.Top + paddle.Height / 2) {
			var ballCenter = ball.Left + ball.Width / 2;
			var paddleCenter = paddle.Left + paddle.Width / 2;
			var angle = (ballCenter - paddleCenter) * 90 / (paddle.Width / 2) + -90;
				
			// clamp the angle to -165 to 15
			const int minAngle = -165;
			const int maxAngle = 15;
			angle = Math.Max(minAngle, Math.Min(maxAngle, angle));
			BallAngle = angle;
			return;
		}
			
		ResetBall();
	}

	private void ResetBall() {
		ball.Left = ClientSize.Width / 2 + ball.Width / 2;
		ball.Top = ClientSize.Height / 2 + ball.Height / 2;
		BallAngle = Random.Next(10, 170);
	}

	private void GameForm_KeyUp(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
	}

	private void GameForm_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.Escape) Close();
		if (e.KeyCode == Keys.R) ResetBall();
	}
}
