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

public sealed class PauseMenu : Panel {
	public PauseMenu() {
		BackColor = Color.FromArgb(100, 40, 40, 40);
		Size = new(600, 450);

		var label = new Label {
			AutoSize = true,
			Font = new("Candara", 40),
			ForeColor = Color.White,
			Text = "Paused",
			Width = 40 * 7
		};

		var textMeasure = TextRenderer.MeasureText(label.Text, label.Font);
		label.Location = new((Width - textMeasure.Width) / 2, 50);

		var resumeButton = new PauseMenuButton(
			"Resume",
			(_, _) => { (Parent as GameScene)?.HidePauseMenu(); }
		);

		resumeButton.Location = new((Width - resumeButton.Width) / 2, (Height - resumeButton.Height) / 2);

		var restartButton = new PauseMenuButton(
			"Restart",
			(_, _) => {
				var layout = (Parent as GameScene)?.BricksLayout;

				if (layout == null) Program.MainForm.ChangeScene(new LevelSelectionScene());
				else Program.MainForm.ChangeScene(new GameScene(layout));
			}
		);

		restartButton.Location = new((Width - restartButton.Width) / 2, (Height - restartButton.Height) / 2 + 75);

		var exitButton = new PauseMenuButton(
			"Quit",
			static (_, _) => Program.MainForm.ChangeScene(new MainMenuScene())
		);

		exitButton.Location = new((Width - exitButton.Width) / 2, (Height - exitButton.Height) / 2 + 150);


		Controls.Add(label);
		Controls.Add(resumeButton);
		Controls.Add(restartButton);
		Controls.Add(exitButton);
	}

	private sealed class PauseMenuButton : Button {
		public PauseMenuButton(string text, EventHandler clickHandler) {
			Text = text;
			Click += clickHandler;
			Width = 200;
			Height = 50;
			BackColor = Color.FromArgb(30, 30, 30);
			ForeColor = Color.White;
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 0;
			Font = new("Candara", 20);
		}
	}
}

public partial class GameScene : AbstractScene {
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
	private readonly PauseMenu _pauseMenu = new();
	private readonly List<ScoreLabel> _scoreLabels = new();

	public readonly Ball Ball = new();

	public readonly string BricksLayout;

	private bool _accelerate;
	private int _lives = 5;
	private int _score;

	public GameScene(string bricksLayout) {
		BricksLayout = bricksLayout;
		InitializeComponent();
		Controls.Add(Ball);
		Ball.Reset();
		_pauseMenu.Location = new(ClientSize.Width / 2 - _pauseMenu.Width / 2, ClientSize.Height / 2 - _pauseMenu.Height / 2);

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
	private void GenerateBricks() {
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

	private void GameLoop(object sender, ElapsedEventArgs e) {
		var deltaTime = (int)(e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;

		debugLabel.Visible = debugLabel.Text.Length > 0;

		if (_accelerate) Ball.Speed = 2;
		else Ball.Speed = .9f;

		ScoreLabel.Text = $"Score: {_score}";
		LivesLabel.Text = $"Lives: {_lives}";

		MovePaddle(deltaTime);
		MoveScoreLabels(deltaTime);

		if (Ball.Waiting) {
			Ball.Location = new(paddle.CenterX() - Ball.Width / 2, paddle.Top - Ball.Height);
			return;
		}

		Ball.Move(deltaTime);

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
		var ballRect = Ball.Bounds;
		var nextBallRect = new Rectangle((int)(Ball.Left + Ball.Velocity.X), (int)(Ball.Top + Ball.Velocity.Y), Ball.Width, Ball.Height);
		var touchingBricks = _bricks.Where(brick => brick.Bounds.IntersectsWith(nextBallRect)).ToList();
		if (touchingBricks.Count == 0) return;
		
		// search the most touched brick
		var mostTouchedBrick = touchingBricks.Select(
			brick => new {
				brick,
				brickRectIntersect = Rectangle.Intersect(brick.Bounds, ballRect)
			}
		).OrderByDescending(static brick => brick.brickRectIntersect.Width * brick.brickRectIntersect.Height).FirstOrDefault();

		if (mostTouchedBrick?.brick == null) return;
		var brick = mostTouchedBrick.brick!;

		var touchingSide =
			mostTouchedBrick.brickRectIntersect.Width > mostTouchedBrick.brickRectIntersect.Height
				? Ball.Top < brick.Top ? Side.Top : Side.Bottom
				: Ball.Left < brick.Left ? Side.Left : Side.Right;

		// determine the new velocity based on the side
		switch (touchingSide) {
			case Side.Bottom or Side.Top:
				Ball.Velocity.Y *= -1;
				break;

			case Side.Left or Side.Right:
				Ball.Velocity.X *= -1;
				break;
		}

		// hit the brick
		mostTouchedBrick.brick.Hit(this, touchingSide);
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
		var bounceAngle = Math.Atan2(-Ball.Velocity.Y, Ball.Velocity.X);
		var paddleAngle = (Ball.CenterX() - paddle.CenterX()) * 90 / (paddle.Width / 2d) - 90;
		var finalAngle = bounceAngle * .1 + paddleAngle * .9;

		const int delta = 15;
		const int minAngle = -180 + delta;
		const int maxAngle = 0 - delta;
		finalAngle = Math.Max(minAngle, Math.Min(maxAngle, finalAngle));

		Ball.Velocity = new((float)Math.Cos(finalAngle * Math.PI / 180d), (float)Math.Sin(finalAngle * Math.PI / 180d));
	}

	public override void KeyDown(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.B) _accelerate = true;
		if (e.KeyCode == Keys.Space && Ball.Waiting) Ball.LaunchBallFromPaddle();
		if (e.KeyCode == Keys.R) Ball.Reset();

		if (e.KeyCode == Keys.Escape) {
			if (Controls.Contains(_pauseMenu)) HidePauseMenu();
			else ShowPauseMenu();
		}
	}

	public override void KeyUp(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
		if (e.KeyCode == Keys.B) _accelerate = false;
	}

	private void ShowPauseMenu() {
		Controls.Add(_pauseMenu);
		_pauseMenu.BringToFront();
		timer.Stop();
	}

	public void HidePauseMenu() {
		Controls.Remove(_pauseMenu);
		timer.Start();
	}
}
