using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

namespace Breakout;

public partial class GameForm : Form {
	private const int TimerInterval = 1000 / 120;
	private static readonly Random Random = new();

	private static readonly Dictionary<char, BrickType> BricksTypes = new() {
		{ '-', new BrickType { Name = "Brick", Color = Color.Red, Score = 10 } },
		{ ' ', new BrickType { Name = "Empty", Color = Color.Transparent } }
	};

	private readonly List<Brick> _bricks;

	private readonly List<ScoreLabel> _scoreLabels = new();

	private bool _accelerate;
	private int _score;

	public PointF BallVelocity;

	public string BricksLayout = @"
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
		--------------------
	";

	public GameForm() {
		InitializeComponent();

		paddle.Left = ClientSize.Width / 2 - paddle.Width / 2;
		paddle.Top = (int)(ClientSize.Height * .9) - paddle.Height;
		ResetBall();

		timer.Interval = TimerInterval;

		Debug.WriteLine("debug");
		_bricks = new List<Brick>(Regex.Replace(BricksLayout, @"\s+", "").Length);
		GenerateBricks();
	}

	public float BallSpeed { get; set; }

	public int PaddleSpeed { get; set; } = 1;

	private bool LeftPressed { get; set; }
	private bool RightPressed { get; set; }


	/**
	 * Creates bricks from the BricksLayout string.
	 * A Brick is a PictureBox with a Tag of "Brick".
	 * The bricks are added to the Controls collection.
	 */
	public void GenerateBricks() {
		var rows = BricksLayout.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		var startX = (ClientSize.Width - (Brick.BrickWidth + Brick.BrickMargin) * rows[0].Length) / 2;
		var startY = (ClientSize.Height - (Brick.BrickHeight + Brick.BrickMargin) * rows.Length) / 5;

		for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++) {
			var row = rows[rowIndex];

			for (var colIndex = 0; colIndex < row.Length; colIndex++) {
				if (!BricksTypes.ContainsKey(row[colIndex])) continue;

				var brickType = BricksTypes[row[colIndex]];
				if (brickType.IsEmpty) continue;

				var brick = new Brick(brickType) {
					Left = startX + colIndex * (Brick.BrickWidth + Brick.BrickMargin),
					Top = startY + rowIndex * (Brick.BrickHeight + Brick.BrickMargin)
				};
				_bricks.Add(brick);
			}
		}

		Controls.AddRange(_bricks.ToArray());

		// set index for all bricks
		foreach (var brick in _bricks) {
			Controls.SetChildIndex(brick, 1);
		}
	}

	private void timer1_Elapsed(object sender, ElapsedEventArgs e) {
		var deltaTime = (int)(e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;

		if (_accelerate) BallSpeed = 2;
		else BallSpeed = .65f;
		
		ScoreLabel.Text = $"Score: {_score}";

		MoveBall(deltaTime);
		MovePaddle(deltaTime);
		MoveScoreLabels(deltaTime);

		if (ball.Left < 0 || ball.Left > ClientSize.Width - ball.Width) BallVelocity.X *= -1;

		if (ball.Top < 0) {
			BallVelocity.Y *= -1;
		} else if (ball.Top > ClientSize.Height - ball.Height) {
			ResetBall();
			Invalidate();
			return;
		}

		BricksPhysics(deltaTime);

		if (!ball.Bounds.IntersectsWith(paddle.Bounds)) return;
		PaddlePhysic();
	}

	private void MovePaddle(int deltaTime) {
		if (LeftPressed && paddle.Left > 0) paddle.Left -= PaddleSpeed * deltaTime;
		if (RightPressed && paddle.Bounds.Right < ClientSize.Width) paddle.Left += PaddleSpeed * deltaTime;
	}

	private void BricksPhysics(int deltaTime) {
		foreach (var brick in _bricks) {
			if (!ball.Bounds.IntersectsWith(brick.Bounds)) continue;
			var ballRect = ball.Bounds;
			var brickRect = brick.Bounds;

			var ballCenter = new PointF(ballRect.X + ballRect.Width / 2f, ballRect.Y + ballRect.Height / 2f);
			var brickCenter = new PointF(brickRect.X + brickRect.Width / 2f, brickRect.Y + brickRect.Height / 2f);

			_bricks.Remove(brick);
			Controls.Remove(brick);
			_score += brick.Type.Score;

			var scoreLabel = new ScoreLabel(brick.Location, brick.Top - 80, brick.Type.Score);
			_scoreLabels.Add(scoreLabel);
			Controls.Add(scoreLabel);
			Controls.SetChildIndex(scoreLabel, 0);

			// if the ball hit the brick from the side
			if ((ballCenter.X < brickCenter.X && BallVelocity.X > 0) || (ballCenter.X > brickCenter.X && BallVelocity.X < 0)) BallVelocity.X *= -1;

			// if the ball hit the brick from the top or bottom
			if ((ballCenter.Y < brickCenter.Y && BallVelocity.Y > 0) || (ballCenter.Y > brickCenter.Y && BallVelocity.Y < 0)) BallVelocity.Y *= -1;

			MoveBall(deltaTime);
			break;
		}
	}

	private void MoveScoreLabels(int deltaTime) {
		for (var i = 0; i < _scoreLabels.Count; i++) {
			var scoreLabel = _scoreLabels[i];
			var shouldDispose = scoreLabel.Move(deltaTime);

			if (!shouldDispose) continue;

			_scoreLabels.Remove(scoreLabel);
			Controls.Remove(scoreLabel);
		}
	}

	private void PaddlePhysic() {
		var ballCenterX = ball.Left + ball.Width / 2;
		var paddleCenter = paddle.Left + paddle.Width / 2;
		var angle = (ballCenterX - paddleCenter) * 90 / (paddle.Width / 2) - 90;

		// clamp the angle to -165 to 15
		const int delta = 15;
		const int minAngle = -180 + delta;
		const int maxAngle = 0 - delta;
		angle = Math.Max(minAngle, Math.Min(maxAngle, angle));

		BallVelocity = new PointF((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}

	private void MoveBall(int deltaTime) {
		ball.Left += (int)Math.Round(BallVelocity.X * BallSpeed * deltaTime);
		ball.Top += (int)Math.Round(BallVelocity.Y * BallSpeed * deltaTime);
	}

	private void ResetBall() {
		ball.Left = ClientSize.Width / 2 + ball.Width / 2;
		ball.Top = ClientSize.Height / 2 + ball.Height / 2;
		// set velocity to speed but in a random direction

		var angle = Random.Next(0, 180);
		BallVelocity = new PointF((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}

	private void GameForm_KeyUp(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
		if (e.KeyCode == Keys.Space) _accelerate = false;
	}

	private void GameForm_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.Space) _accelerate = true;
		if (e.KeyCode == Keys.Escape) Close();
		if (e.KeyCode == Keys.R) ResetBall();
	}
}
