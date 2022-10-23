namespace Breakout;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Breakout.Entities;
using Timer = System.Timers.Timer;

public enum Side {
	Left,
	Right,
	Top,
	Bottom
}

public sealed partial class GameScene : AbstractScene {
	private const int TimerInterval = 1000 / 60;
	private const int PaddleSpeed = 1;

	private readonly List<Ball> _balls = new();
	private readonly List<Brick> _bricks;

	private readonly Timer _noClipTimer = new() {
		Interval = 5000
	};

	private readonly PauseMenu _pauseMenu = new();
	private readonly List<PowerUp> _powerUps = new();
	private readonly List<ScoreLabel> _scoreLabels = new();

	public readonly Level Level;

	private bool _accelerate;
	private int _score;

	public double BallSpeedMultiplier = 1;
	public int Lives = 5;
	public double ScoreMultiplier = 1;

	public GameScene(Level level) {
		Level = level;
		InitializeComponent();
		AddBall();

		for (var i = 0; i < 5; i++) {
			AddBall();
		}

		_pauseMenu.Location = new(ClientSize.Width / 2 - _pauseMenu.Width / 2, ClientSize.Height / 2 - _pauseMenu.Height / 2);

		Paddle.Left = ClientSize.Width / 2 - Paddle.Width / 2;
		Paddle.Top = (int) (ClientSize.Height * .9) - Paddle.Height;

		physicsTimer.Interval = TimerInterval;
		_bricks = new(Regex.Replace(Level.Layout, @"\s+", "").Length);
		GenerateBricks();

		_noClipTimer.Elapsed += (_, _) => {
			IsNoClip = false;
			_noClipTimer.Stop();
		};
	}

	private bool LeftPressed { get; set; }
	private bool RightPressed { get; set; }

	public int BallCount => _balls.Count;

	public bool IsNoClip { get; private set; }

	public void AddBall() {
		var ball = new Ball();
		ball.Reset();
		_balls.Add(ball);
		Controls.Add(ball);
	}

	public void AddPowerUp(PowerUp powerUp) {
		_powerUps.Add(powerUp);
		Controls.Add(powerUp);
		powerUp.BringToFront();
	}

	public void RemoveBrick(Brick brick) {
		_bricks.Remove(brick);
		Controls.Remove(brick);
		_score += (int) (brick.Type.Score * ScoreMultiplier);

		if (_scoreLabels.Count > 10) return;
		var scoreLabel = new ScoreLabel(brick.Location, brick.Top - 80, (int) (brick.Type.Score * ScoreMultiplier));
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
		var rows = Level.Layout.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		var longestRow = rows.Max(static row => row.Length);

		for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++) {
			var row = rows[rowIndex];
			if (row.Length == 0) continue;

			for (var colIndex = 0; colIndex < row.Length; colIndex++) {
				var startX = (ClientSize.Width - (Brick.BrickWidth + Brick.BrickMargin) * longestRow) / 2;
				var startY = (ClientSize.Height - (Brick.BrickHeight + Brick.BrickMargin) * rows.Length) / 5;

				// search if character is included in a brick key
				foreach (var brick in
					(
						from brickEntry in BrickType.BricksTypes
						let character = row[colIndex].ToString()
						where brickEntry.Key.Contains(character)
						where !brickEntry.Value.IsEmpty
						let brickIndex = brickEntry.Key.IndexOf(character, StringComparison.Ordinal)
						select new Brick(brickEntry.Value) {
							Left = startX + (Brick.BrickWidth + Brick.BrickMargin) * colIndex,
							Top = startY + (Brick.BrickHeight + Brick.BrickMargin) * rowIndex,
							Health = brickEntry.Value.MaxHealth / brickEntry.Key.Length * (brickIndex + 1)
						}
					).Where(static brick => brick != null)
				) {
					Controls.Add(brick);
					Controls.SetChildIndex(brick, 1);
					_bricks.Add(brick);
					break;
				}
			}
		}
	}

	private void GameLoop(object _, ElapsedEventArgs e) {
		var deltaTime = (int) (e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;
		MovePaddle(deltaTime);

		if (_bricks.Count == 0) Win();

		foreach (var ball in _balls) {
			if (ball.Waiting) {
				ball.Location = new(Paddle.CenterX() - ball.Width / 2, Paddle.Top - ball.Height);
				continue;
			}
#if DEBUG
			if (_accelerate) ball.Speed = 2 * BallSpeedMultiplier;
			else
#endif
				ball.Speed = .9f * BallSpeedMultiplier;

			ball.Move(deltaTime);

			if (ball.Left < 0 || ball.Left > ClientSize.Width - ball.Width) ball.Velocity.X *= -1;

			if (ball.Top < 0) {
				ball.Velocity.Y *= -1;
			} else if (ball.Top > ClientSize.Height - ball.Height) {
				ball.Reset();
				Lives--;
				Invalidate();
				continue;
			}

			BricksPhysics(ball);

			if (!ball.Bounds.IntersectsWith(Paddle.Bounds)) continue;
			PaddlePhysic(ball);
		}
	}

	private void Win() {
		physicsTimer.Close();
		movingObjectsTimer.Close();

		var winLabel = new Label {
			Text = "You win!",
			Font = new(Program.MainFont, 60),
			ForeColor = Color.White,
			AutoSize = true
		};

		winLabel.Location = new(ClientSize.Width / 2 - 180, ClientSize.Height / 2 - 100);

		Controls.Add(winLabel);
		Controls.SetChildIndex(winLabel, 0);

		var timer = new Timer(4 * 1000) {
			Enabled = true,
			AutoReset = false
		};

		timer.Elapsed += (_, _) => {
			AbstractScene scene = new LevelSelectionScene();

			if (Level.Number < LevelManager.Levels.Length - 1) {
				var level = LevelManager.LoadLevel(Level.Number + 1);
				scene = new GameScene(level);
			}

			Program.MainForm.ChangeScene(scene);
		};
	}

	public void NoClip() {
		IsNoClip = true;
		if (_noClipTimer.Enabled) _noClipTimer.Stop();
		_noClipTimer.Start();
	}

	private void MovePaddle(int deltaTime) {
		if (LeftPressed && Paddle.Left > 0) Paddle.Left -= PaddleSpeed * deltaTime;
		if (RightPressed && Paddle.Bounds.Right < ClientSize.Width) Paddle.Left += PaddleSpeed * deltaTime;
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

	private void MovePowerUps(int deltaTime) {
		for (var i = 0; i < _powerUps.Count; i++) {
			var powerUp = _powerUps[i];
			if (!powerUp.Move(deltaTime)) continue;

			_powerUps.Remove(powerUp);
			Controls.Remove(powerUp);
		}
	}

	private void BricksPhysics(Ball ball) {
		var ballRect = ball.Bounds;
		var nextBallRect = new Rectangle((int) (ball.Left + ball.Velocity.X), (int) (ball.Top + ball.Velocity.Y), ball.Width, ball.Height);
		var touchingBricks = _bricks.Where(brick => brick.Bounds.IntersectsWith(nextBallRect)).ToList();
		if (touchingBricks.Count == 0) return;

		// search the most touched brick
		var mostTouchedBrick = touchingBricks.Select(
				brick => new {
					brick,
					brickRectIntersect = Rectangle.Intersect(brick.Bounds, ballRect)
				}
			)
			.OrderByDescending(static brick => brick.brickRectIntersect.Width * brick.brickRectIntersect.Height)
			.FirstOrDefault();

		if (mostTouchedBrick?.brick == null) return;
		var brick = mostTouchedBrick.brick!;

		var touchingSide =
			mostTouchedBrick.brickRectIntersect.Width > mostTouchedBrick.brickRectIntersect.Height
				? ball.Top < brick.Top ? Side.Top : Side.Bottom
				: ball.Left < brick.Left
					? Side.Left
					: Side.Right;

		if (!IsNoClip) {
			// determine the new velocity based on the side
			switch (touchingSide) {
				case Side.Bottom or Side.Top:
					ball.Velocity.Y *= -1;
					break;

				case Side.Left or Side.Right:
					ball.Velocity.X *= -1;
					break;
			}
		}

		// hit the brick
		mostTouchedBrick.brick.Hit(this, ball, touchingSide);
	}

	private void PowerUpPhysics() {
		var collisionPayload = new CollisionPayload {
			Game = this
		};

		var paddleBounds = Paddle.Bounds;

		for (var i = 0; i < _powerUps.Count; i++) {
			var powerUp = _powerUps[i];
			if (!powerUp.Bounds.IntersectsWith(paddleBounds)) continue;

			powerUp.Apply(collisionPayload);
			_powerUps.Remove(powerUp);
			Controls.Remove(powerUp);
		}
	}

	private void PaddlePhysic(Ball ball) {
		var bounceAngle = Math.Atan2(-ball.Velocity.Y, ball.Velocity.X);
		var paddleAngle = (ball.CenterX() - Paddle.CenterX()) * 90 / (Paddle.Width / 2d) - 90;
		var finalAngle = bounceAngle * .1 + paddleAngle * .9;

		const int delta = 15;
		const int minAngle = -180 + delta;
		const int maxAngle = 0 - delta;
		finalAngle = Math.Max(minAngle, Math.Min(maxAngle, finalAngle));

		ball.Velocity = new((float) Math.Cos(finalAngle * Math.PI / 180d), (float) Math.Sin(finalAngle * Math.PI / 180d));
	}

	public override void KeyDown(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = true;
		if (e.KeyCode == Keys.Right) RightPressed = true;
		if (e.KeyCode == Keys.B) _accelerate = true;

		if (e.KeyCode == Keys.Space) {
			foreach (var ball in _balls.Where(static ball => ball.Waiting)) ball.LaunchBallFromPaddle();
		}

		if (e.KeyCode == Keys.R)
			foreach (var ball in _balls) {
				ball.Reset();
			}

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
		physicsTimer.Stop();
		movingObjectsTimer.Stop();
	}

	public void HidePauseMenu() {
		Controls.Remove(_pauseMenu);
		physicsTimer.Start();
		movingObjectsTimer.Start();
	}

	private void MovingObjectsLoop(object sender, ElapsedEventArgs e) {
		var deltaTime = (int) (e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;

		MovePowerUps(deltaTime);
		MoveScoreLabels(deltaTime);
		PowerUpPhysics();

		debugLabel.Visible = debugLabel.Text.Length > 0;

		ScoreLabel.Text = $"Score: {_score}";
		LivesLabel.Text = $"Lives: {Lives}";
	}
}
