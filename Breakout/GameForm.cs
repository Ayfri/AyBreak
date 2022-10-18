using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		{ "-23", new() { Name = "Brick", Color = Color.Red, MaxHealthColor = Color.Orange, Score = 10, MaxHealth = 3 } }, {
			"*", new() {
				Name = "Explosion", Color = Color.Purple, Score = 30, OnCollision = static collision => {
					var brickPos = collision.BrickPosition;
					Debug.WriteLine($"Explosion at {brickPos}");
					collision.Game.GetBrick(brickPos with { X = brickPos.X + 1 })?.Hit(collision.Game, Side.Left);
					collision.Game.GetBrick(brickPos with { X = brickPos.X - 1 })?.Hit(collision.Game, Side.Right);
					collision.Game.GetBrick(brickPos with { Y = brickPos.Y + 1 })?.Hit(collision.Game, Side.Top);
					collision.Game.GetBrick(brickPos with { Y = brickPos.Y - 1 })?.Hit(collision.Game, Side.Bottom);
				}
			}
		}
	};

	private readonly List<Brick> _bricks;
	private readonly List<ScoreLabel> _scoreLabels = new();

	public readonly Ball Ball = new();

	private bool _accelerate;
	private int _lives = 5;
	private int _score;

	public string BricksLayout = new Level(6).Layout;

	public GameForm() {
		InitializeComponent();
		Controls.Add(Ball);
		Ball.Reset();

		paddle.Left = ClientSize.Width / 2 - paddle.Width / 2;
		paddle.Top = (int)(ClientSize.Height * .9) - paddle.Height;

		timer.Interval = TimerInterval;
		_bricks = new(Regex.Replace(BricksLayout, @"\s+", "").Length);
		GenerateBricks();
	}

	public int PaddleSpeed { get; set; } = 1;

	private bool LeftPressed { get; set; }
	private bool RightPressed { get; set; }

	public void RemoveBrick(Brick brick) {
		_bricks.Remove(brick);
		Controls.Remove(brick);
		_score += brick.Type.Score;

		var scoreLabel = new ScoreLabel(brick.Location, brick.Top - 80, brick.Type.Score);
		_scoreLabels.Add(scoreLabel);
		Controls.Add(scoreLabel);
		Controls.SetChildIndex(scoreLabel, 0);
	}

	public Brick? GetBrick(Point pos) =>
		(from brick in _bricks let brickPos = new Point(brick.Left / Brick.BrickWidth, brick.Top / Brick.BrickHeight) where brickPos == pos select brick)
	   .FirstOrDefault();

	public Point GetBrickPos(Brick brick) => new(brick.Left / Brick.BrickWidth, brick.Top / Brick.BrickHeight);


	/**
	 * Creates bricks from the BricksLayout string.
	 * A Brick is a PictureBox with a Tag of "Brick".
	 * The bricks are added to the Controls collection.
	 */
	public void GenerateBricks() {
		var rows = BricksLayout.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		var longestRow = rows.Max(row => row.Length);

		for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++) {
			var row = rows[rowIndex];
			if (row.Length == 0) continue;

			for (var colIndex = 0; colIndex < row.Length; colIndex++) {
				var startX = (ClientSize.Width - (Brick.BrickWidth + Brick.BrickMargin) * longestRow) / 2;
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

		if (_accelerate) Ball.Speed = 2;
		else Ball.Speed = .65f;

		ScoreLabel.Text = $"Score: {_score}";
		LivesLabel.Text = $"Lives: {_lives}";

		MovePaddle(deltaTime);
		MoveScoreLabels(deltaTime);

		if (Ball.Waiting) Ball.Location = new(paddle.CenterX() - Ball.Width / 2, paddle.Top - Ball.Height);
		else Ball.Move(deltaTime);

		if (Ball.Left < 0 || Ball.Left > ClientSize.Width - Ball.Width) Ball.Velocity.X *= -1;

		if (Ball.Top < 0) {
			Ball.Velocity.Y *= -1;
		} else if (Ball.Top > ClientSize.Height - Ball.Height) {
			Ball.Reset();
			_lives--;
			Invalidate();
			return;
		}

		BricksPhysics();

		if (!Ball.Bounds.IntersectsWith(paddle.Bounds)) return;
		PaddlePhysic();
	}

	private void MovePaddle(int deltaTime) {
		if (LeftPressed && paddle.Left > 0) paddle.Left -= PaddleSpeed * deltaTime;
		if (RightPressed && paddle.Bounds.Right < ClientSize.Width) paddle.Left += PaddleSpeed * deltaTime;
	}

	private void BricksPhysics() {
		var touchingBricks = _bricks.Where(brick => Ball.Bounds.IntersectsWith(brick.Bounds)).ToList();
		if (touchingBricks.Count == 0) return;

		// determine the side of the brick that the ball hit using the distance between the centers and the velocity
		var ballRect = Ball.Bounds;

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
				Ball.Velocity.Y *= -1;
				break;

			case Side.Left or Side.Right:
				Ball.Velocity.X *= -1;
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

		// hit the brick
		brick?.Hit(this, touchedSide.Value);
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
		var angle = (Ball.CenterX() - paddle.CenterX()) * 90 / (paddle.Width / 2) - 90;

		const int delta = 15;
		const int minAngle = -180 + delta;
		const int maxAngle = 0 - delta;
		angle = Math.Max(minAngle, Math.Min(maxAngle, angle));

		Ball.Velocity = new((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180));
	}

	public override void KeyDown(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.B) _accelerate = true;
		if (e.KeyCode == Keys.Space && Ball.Waiting) Ball.LaunchBallFromPaddle();
		if (e.KeyCode == Keys.R) Ball.Reset();
	}

	public override void KeyUp(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
		if (e.KeyCode == Keys.B) _accelerate = false;
	}
}
