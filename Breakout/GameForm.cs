using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

namespace Breakout;

public enum Side {
	Left,
	Right,
	Top,
	Bottom
}

public static class Extensions {
	public static int CenterX(this Rectangle rect) => rect.X + rect.Width / 2;
	public static int CenterY(this Rectangle rect) => rect.Y + rect.Height / 2;

	public static int CenterX(this Control control) => control.Left + control.Width / 2;
	public static int CenterY(this Control control) => control.Top + control.Height / 2;

	public static Side? GetTouchingSide(this Rectangle rect, Rectangle other) {
		if (rect.CenterY() <= other.CenterY() - other.Height / 2) return Side.Top;
		if (rect.CenterY() >= other.CenterY() + other.Height / 2) return Side.Bottom;
		if (rect.CenterX() > other.CenterX()) return Side.Right;
		if (rect.CenterX() < other.CenterX()) return Side.Left;
		return null;
	}
}

public partial class GameForm : AbstractScene {
	private const int TimerInterval = 1000 / 60;

	private static readonly Dictionary<string, BrickType> BricksTypes = new() {
		{ " ", new() { Name = "Empty", Color = Color.Transparent } },
		{ "-23", new() { Name = "Brick", Color = Color.Red, MaxHealthColor = Color.Orange, Score = 10, MaxHealth = 3 } }
	};

	private readonly Ball _ball = new();
	private readonly List<Brick> _bricks;
	private readonly List<ScoreLabel> _scoreLabels = new();

	private bool _accelerate;
	private int _lives = 5;
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
		Controls.Add(_ball);
		_ball.Reset();

		paddle.Left = ClientSize.Width / 2 - paddle.Width / 2;
		paddle.Top = (int)(ClientSize.Height * .9) - paddle.Height;

		timer.Interval = TimerInterval;
		_bricks = new(Regex.Replace(BricksLayout, @"\s+", "").Length);
		GenerateBricks();
	}

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

	private void timer_Elapsed(object sender, ElapsedEventArgs e) {
		var deltaTime = (int)(e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;

		debugLabel.Visible = debugLabel.Text.Length == 0;

		if (_accelerate) _ball.Speed = 2;
		else _ball.Speed = .65f;

		ScoreLabel.Text = $"Score: {_score}";
		LivesLabel.Text = $"Lives: {_lives}";

		MovePaddle(deltaTime);
		MoveScoreLabels(deltaTime);
		
		if (_ball.Waiting) _ball.Location = new(paddle.CenterX() - _ball.Width / 2, paddle.Top - _ball.Height);
		else _ball.Move(deltaTime);

		if (_ball.Left < 0 || _ball.Left > ClientSize.Width - _ball.Width) _ball.Velocity.X *= -1;

		if (_ball.Top < 0) {
			_ball.Velocity.Y *= -1;
		} else if (_ball.Top > ClientSize.Height - _ball.Height) {
			_ball.Reset();
			_lives--;
			Invalidate();
			return;
		}

		BricksPhysics();

		if (!_ball.Bounds.IntersectsWith(paddle.Bounds)) return;
		PaddlePhysic();
	}

	private void MovePaddle(int deltaTime) {
		if (LeftPressed && paddle.Left > 0) paddle.Left -= PaddleSpeed * deltaTime;
		if (RightPressed && paddle.Bounds.Right < ClientSize.Width) paddle.Left += PaddleSpeed * deltaTime;
	}

	private void BricksPhysics() {
		var touchingBricks = _bricks.Where(brick => _ball.Bounds.IntersectsWith(brick.Bounds)).ToList();
		if (touchingBricks.Count == 0) return;

		// determine the side of the brick that the ball hit using the distance between the centers and the velocity
		var ballRect = _ball.Bounds;

		// create a rectangle that is the size of all the bricks the ball is touching
		var bricksRect = new Rectangle(
			touchingBricks.Min(static b => b.Left),
			touchingBricks.Min(static b => b.Top),
			touchingBricks.Max(static b => b.Right) - touchingBricks.Min(static b => b.Left),
			touchingBricks.Max(static b => b.Bottom) - touchingBricks.Min(static b => b.Top)
		);

		// determine the side (only up, down, left or right) of the brick that the ball hit, search the most touching side
		var touchedSide = ballRect.GetTouchingSide(bricksRect);

		// determine the new velocity based on the side
		switch (touchedSide) {
			case Side.Bottom or Side.Top:
				_ball.Velocity.Y *= -1;
				break;

			case Side.Left or Side.Right:
				_ball.Velocity.X *= -1;
				break;

			case null:
				return;
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
		var angle = (_ball.CenterX() - paddle.CenterX()) * 90 / (paddle.Width / 2) - 90;

		// clamp the angle to -165 to 15
		const int delta = 15;
		const int minAngle = -180 + delta;
		const int maxAngle = 0 - delta;
		angle = Math.Max(minAngle, Math.Min(maxAngle, angle));

		_ball.Velocity = new((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}
	
	public override void KeyDown(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.B) _accelerate = true;
		if (e.KeyCode == Keys.Space && _ball.Waiting) _ball.LaunchBallFromPaddle();
		if (e.KeyCode == Keys.R) _ball.Reset();
	}
	
	public override void KeyUp(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
		if (e.KeyCode == Keys.B) _accelerate = false;
	}
}
