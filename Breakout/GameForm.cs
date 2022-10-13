namespace Breakout;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

public static class Extensions {
	public static int CenterX(this Rectangle rect) => rect.X + rect.Width / 2;
	public static int CenterY(this Rectangle rect) => rect.Y + rect.Height / 2;
}

public partial class GameForm : Form {
	private const int TimerInterval = 1000 / 60;
	private static readonly Random Random = new();

	private static readonly Dictionary<string, BrickType> BricksTypes = new() {
		{ " ", new BrickType { Name = "Empty", Color = Color.Transparent } },
		{ "-23", new BrickType { Name = "Brick", Color = Color.Red, MaxHealthColor = Color.Orange, Score = 10, MaxHealth = 3 } }
	};

	private readonly List<Brick> _bricks;
	private readonly List<ScoreLabel> _scoreLabels = new();

	private bool _accelerate;
	private PointF _ballVelocity;
	private bool _ballWaiting;
	private int _score;

	public string BricksLayout = @"
		--------------------
		----222222222222----
		---22222333322222---
		---22333333333322---
		---22333333333322---
		---22233333333222---
		-----2222332222-----
		-222-----33-----222-
		-232-----33-----232-
		-222-----22-----222-
		--------------------
	";

	public GameForm() {
		InitializeComponent();

		paddle.Left = ClientSize.Width / 2 - paddle.Width / 2;
		paddle.Top = (int)(ClientSize.Height * .9) - paddle.Height;
		ResetBall();

		timer.Interval = TimerInterval;
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

		for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++) {
			var row = rows[rowIndex];

			for (var colIndex = 0; colIndex < row.Length; colIndex++) {
				var startX = (ClientSize.Width - (Brick.BrickWidth + Brick.BrickMargin) * rows[0].Length) / 2;
				var startY = (ClientSize.Height - (Brick.BrickHeight + Brick.BrickMargin) * rows.Length) / 5;

				// search if character is included in a brick key
				foreach (var brick in from brickEntry in BricksTypes
					let character = row[colIndex].ToString()
					where brickEntry.Key.Contains(character)
					where !brickEntry.Value.IsEmpty
					let brickIndex = brickEntry.Key.IndexOf(character, StringComparison.Ordinal)
					select new Brick(brickEntry.Value) {
						Left = startX + (Brick.BrickWidth + Brick.BrickMargin) * colIndex,
						Top = startY + (Brick.BrickHeight + Brick.BrickMargin) * rowIndex,
						Health = brickEntry.Value.MaxHealth / brickEntry.Key.Length * (brickIndex + 1)
					}
				) {
					Controls.Add(brick);
					Controls.SetChildIndex(brick, 1);
					_bricks.Add(brick);
					break;
				}
			}
		}
	}

	private void timer1_Elapsed(object sender, ElapsedEventArgs e) {
		var deltaTime = (int)(e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;

		if (_accelerate) BallSpeed = 2;
		else BallSpeed = .65f;

		ScoreLabel.Text = $"Score: {_score}";

		if (_ballWaiting) ball.Location = new Point(paddle.Left + paddle.Width / 2 - ball.Width / 2, paddle.Top - ball.Height);

		MoveBall(deltaTime);
		MovePaddle(deltaTime);
		MoveScoreLabels(deltaTime);

		if (ball.Left < 0 || ball.Left > ClientSize.Width - ball.Width) _ballVelocity.X *= -1;

		if (ball.Top < 0) {
			_ballVelocity.Y *= -1;
		} else if (ball.Top > ClientSize.Height - ball.Height) {
			ResetBall();
			Invalidate();
			return;
		}

		BricksPhysics();

		if (!ball.Bounds.IntersectsWith(paddle.Bounds)) return;
		PaddlePhysic();
	}

	private void MovePaddle(int deltaTime) {
		if (LeftPressed && paddle.Left > 0) paddle.Left -= PaddleSpeed * deltaTime;
		if (RightPressed && paddle.Bounds.Right < ClientSize.Width) paddle.Left += PaddleSpeed * deltaTime;
	}

	private void BricksPhysics() {
		var touchingBricks = _bricks.Where(brick => ball.Bounds.IntersectsWith(brick.Bounds)).ToList();
		if (touchingBricks.Count == 0) return;

		// determine the side of the brick that the ball hit using the distance between the centers and the velocity
		var ballRect = ball.Bounds;

		// create a rectangle that is the size of all the bricks the ball is touching
		var bricksRect = new Rectangle(
			touchingBricks.Min(static b => b.Left),
			touchingBricks.Min(static b => b.Top),
			touchingBricks.Max(static b => b.Right) - touchingBricks.Min(static b => b.Left),
			touchingBricks.Max(static b => b.Bottom) - touchingBricks.Min(static b => b.Top)
		);

		// determine the side (only up, down, left or right) of the brick that the ball hit, search the most touching side
		ArrowDirection touchedSide;
		
		if (
			ballRect.CenterX() < bricksRect.CenterX() && ballRect.CenterY() < bricksRect.CenterY() + bricksRect.Height / 2 &&
		    ballRect.CenterY() > bricksRect.CenterY() - bricksRect.Height / 2
		) {
			touchedSide = ArrowDirection.Left;
		} else if (
			ballRect.CenterX() > bricksRect.CenterX() && ballRect.CenterY() < bricksRect.CenterY() + bricksRect.Height / 2 &&
			ballRect.CenterY() > bricksRect.CenterY() - bricksRect.Height / 2
		) {
			touchedSide = ArrowDirection.Right;
		} else if (
			ballRect.CenterY() < bricksRect.CenterY() && ballRect.CenterX() < bricksRect.CenterX() + bricksRect.Width / 2 &&
           ballRect.CenterX() > bricksRect.CenterX() - bricksRect.Width / 2
		) {
			touchedSide = ArrowDirection.Up;
		} else if (
			ballRect.CenterY() > bricksRect.CenterY() && ballRect.CenterX() < bricksRect.CenterX() + bricksRect.Width / 2 &&
		    ballRect.CenterX() > bricksRect.CenterX() - bricksRect.Width / 2
		) {
			touchedSide = ArrowDirection.Down;
		} else {
			return;
		}

		// determine the new velocity based on the side
		switch (touchedSide) {
			case ArrowDirection.Down or ArrowDirection.Up:
				_ballVelocity.Y *= -1;
				break;

			case ArrowDirection.Left or ArrowDirection.Right:
				_ballVelocity.X *= -1;
				break;
		}

		// find the brick that was the most inside the ball
		var brick = touchingBricks.Aggregate(
			(current, next) => {
				var currentDistance = Math.Sqrt(
					Math.Pow(ballRect.CenterX() - current.Bounds.CenterX(), 2) + Math.Pow(ballRect.CenterY() - current.Bounds.CenterY(), 2)
				);

				var nextDistance = Math.Sqrt(
					Math.Pow(ballRect.CenterX() - next.Bounds.CenterX(), 2) + Math.Pow(ballRect.CenterY() - next.Bounds.CenterY(), 2)
				);

				return currentDistance < nextDistance ? current : next;
			}
		);

		// remove the brick
		if (!brick.Hit()) return;
		_bricks.Remove(brick);
		Controls.Remove(brick);
		_score += brick.Type.Score;

		var scoreLabel = new ScoreLabel(brick.Location, brick.Top - 80, brick.Type.Score);
		_scoreLabels.Add(scoreLabel);
		Controls.Add(scoreLabel);
		Controls.SetChildIndex(scoreLabel, 0);
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

		_ballVelocity = new PointF((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}

	private void MoveBall(int deltaTime) {
		ball.Left += (int)Math.Round(_ballVelocity.X * BallSpeed * deltaTime);
		ball.Top += (int)Math.Round(_ballVelocity.Y * BallSpeed * deltaTime);
	}

	private void ResetBall() {
		_ballWaiting = true;
		_ballVelocity = new PointF(0, 0);
	}

	private void LaunchBallFromPaddle() {
		_ballWaiting = false;
		var angle = Random.Next(165, 345);
		_ballVelocity = new PointF((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}

	private void GameForm_KeyUp(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
		if (e.KeyCode == Keys.B) _accelerate = false;
	}

	private void GameForm_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.B) _accelerate = true;

		if (e.KeyCode == Keys.Space && _ballWaiting) {
			_ballWaiting = false;
			LaunchBallFromPaddle();
		}

		if (e.KeyCode == Keys.Escape) Close();
		if (e.KeyCode == Keys.R) ResetBall();
	}
}
