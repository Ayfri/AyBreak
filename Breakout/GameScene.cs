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

/// <summary>
///     A enum for sides.
/// </summary>
public enum Side {
	Left,
	Right,
	Top,
	Bottom
}

/// <summary>
///     The GameScene is the main game scene.
/// </summary>
public sealed partial class GameScene : AbstractScene {
	/// <summary>
	///     The Timer interval for the game to have a 60 FPS.
	/// </summary>
	private const int TimerInterval = 1000 / 60;

	/// <summary>
	///     The speed of the paddle.
	/// </summary>
	private const double PaddleSpeed = 1.5;

	/// <summary>
	///     The List of Balls.
	/// </summary>
	private readonly List<Ball> _balls = new();

	/// <summary>
	///     The list of bricks.
	/// </summary>
	private readonly List<Brick> _bricks;

	/// <summary>
	///     The noClip Timer for timing the noClip powerup.
	/// </summary>
	private readonly Timer _noClipTimer = new() {
		Interval = 5000
	};

	/// <summary>
	///     The pauseMenu panel.
	/// </summary>
	private readonly PauseMenu _pauseMenu = new();

	/// <summary>
	///     The list of powerups actually on the screen.
	/// </summary>
	private readonly List<PowerUp> _powerUps = new();

	/// <summary>
	///     The score labels actually on the screen.
	/// </summary>
	private readonly List<ScoreLabel> _scoreLabels = new();

	/// <summary>
	///     The current level.
	/// </summary>
	public readonly Level Level;

	/// <summary>
	///     Is the game is accelerated.
	/// </summary>
	private bool _accelerate;

	/// <summary>
	///     Is the game paused.
	/// </summary>
	private bool _isPaused;

	/// <summary>
	///     The last mouse position.
	/// </summary>
	private Point _lastMouse = new(-1, -1);

	/// <summary>
	///     The speed multiplier of the ball;
	/// </summary>
	public double BallSpeedMultiplier = 1;

	/// <summary>
	///     The number of lives.
	/// </summary>
	public int Lives = 5;

	/// <summary>
	///     The score.
	/// </summary>
	public int Score;

	/// <summary>
	///     The GameScene function is the main function of the game.
	///     It initializes all of the components and sets up the scene.
	/// </summary>
	/// <param name="level"> The level to load. </param>
	/// <returns> A level object. </returns>
	public GameScene(Level level) {
		Level = level;
		InitializeComponent();
		AddBall();

		#if DEBUG
		for (var i = 0; i < 1; i++) AddBall();
		#endif

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

	/// <summary>
	///     Is the left key pressed.
	/// </summary>
	private bool LeftPressed { get; set; }

	/// <summary>
	///     Is the right key pressed.
	/// </summary>
	private bool RightPressed { get; set; }

	/// <summary>
	///     the number of balls.
	/// </summary>
	public int BallCount => _balls.Count;

	/// <summary>
	///     Is the game in noClip mode.
	/// </summary>
	public bool IsNoClip { get; private set; }

	/// <summary> The AddBall function adds a ball to the game. </summary>
	public void AddBall() {
		var ball = new Ball();
		ball.Reset();
		_balls.Add(ball);
		Controls.Add(ball);
	}

	/// <summary> The AddPowerUp function adds a power up to the game. </summary>
	/// <param name="powerUp"> The power up to add. </param>
	public void AddPowerUp(PowerUp powerUp) {
		_powerUps.Add(powerUp);
		Controls.Add(powerUp);
		powerUp.BringToFront();
	}

	/// <summary>
	///     The RemoveBrick function removes the brick from the game, adds its score to the player's total and adds a score Label if there is currently less than
	///     10 score Labels.
	/// </summary>
	/// <param name="brick"> The brick to be removed. </param>
	public void RemoveBrick(Brick brick) {
		_bricks.Remove(brick);
		Controls.Remove(brick);
		Score += brick.Type.Score;

		if (_scoreLabels.Count > 10) return;
		var scoreLabel = new ScoreLabel(brick.Location, brick.Top - 80, brick.Type.Score);
		_scoreLabels.Add(scoreLabel);
		Controls.Add(scoreLabel);
		Controls.SetChildIndex(scoreLabel, 0);
	}

	/// <summary>
	///     Returns a Brick at the given location.
	/// </summary>
	/// <param name="pos"> The position of the brick. </param>
	/// <returns> The brick at the given location or null if there is no brick. </returns>
	public Brick? GetBrick(Point pos) =>
		(from brick in _bricks let brickPos = new Point(brick.Left / Brick.BrickWidth, brick.Top / Brick.BrickHeight) where brickPos == pos select brick)
		.FirstOrDefault();

	public Point GetBrickPos(Brick brick) => new(brick.Left / Brick.BrickWidth, brick.Top / Brick.BrickHeight);

	/// <summary>
	///     Creates and display bricks from the BricksLayout string.
	/// </summary>
	private void GenerateBricks() {
		var rows = Level.Layout.Split(
			new[] {
				Environment.NewLine
			},
			StringSplitOptions.None
		);

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

	/// <summary>
	///     The GameLoop function is the main game loop.
	///     It runs every 1/60th of a second, and handles all of the physics for each ball in play.
	/// </summary>
	/// <param name="_"> The sender. </param>
	/// <param name="e"> The event arguments. </param>
	private void GameLoop(object _, ElapsedEventArgs e) {
		var deltaTime = (int) (e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;
		MovePaddle(deltaTime);

		if (_bricks.Count(static b => b.Type.ValidateHit == null) == 0) Win();

		if (Lives == 0) Lose();

		for (var index = 0; index < _balls.Count; index++) {
			var ball = _balls[index];

			if (ball.Waiting) {
				ball.MoveToPaddle(Paddle);
				continue;
			}

			if (_accelerate) ball.Speed = 2 * BallSpeedMultiplier;
			else ball.Speed = .9f * BallSpeedMultiplier;

			ball.Move(deltaTime);

			if (ball.Left < 0 || ball.Left > ClientSize.Width - ball.Width) ball.Velocity.X *= -1;

			if (ball.Top < 0) {
				ball.Velocity.Y *= -1;
			} else if (ball.Top > ClientSize.Height - ball.Height) {
				if (_balls.Count > 1) {
					_balls.Remove(ball);
					Controls.Remove(ball);
				} else {
					ball.Reset();
					Lives--;
					_noClipTimer.Stop();
					IsNoClip = false;
				}

				continue;
			}

			BricksPhysics(ball);

			if (!ball.Bounds.IntersectsWith(Paddle.Bounds)) continue;
			PaddlePhysic(ball);
		}
	}

	/// <summary>
	///     The Lose function is called when the player loses all of their lives.
	///     It stops the game loop, and displays the lose Label, then waits for 4 seconds before restarting the game.
	/// </summary>
	private void Lose() {
		physicsTimer.Close();
		movingObjectsTimer.Close();

		var winLabel = new Label {
			Text = "You loose :p",
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

		_isPaused = true;

		timer.Elapsed += (_, _) => Program.MainForm.ChangeScene(new GameScene(Level));
	}

	/// <summary>
	///     The Win function is called when the player has destroyed all of the bricks.
	///     It stops the game loop, and displays the win Label, then waits for 4 seconds before going to the next level or the level select screen if there is no next
	///     level.
	/// </summary>
	private void Win() {
		physicsTimer.Close();
		movingObjectsTimer.Close();

		var winLabel = new Label {
			Text = "You won!",
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

		_isPaused = true;

		timer.Elapsed += (_, _) => {
			AbstractScene scene = new LevelSelectionScene();

			if (Level.Number < LevelManager.Levels.Length - 1) {
				var level = LevelManager.LoadLevel(Level.Number + 1);
				scene = new GameScene(level);
			}

			Program.MainForm.ChangeScene(scene);
		};
	}

	/// <summary>
	///     Starts the NoClip timer.
	/// </summary>
	public void NoClip() {
		IsNoClip = true;
		if (_noClipTimer.Enabled) _noClipTimer.Stop();
		_noClipTimer.Start();
	}

	/// <summary>
	///     Moves the paddle according to the user input with the given deltaTime.
	/// </summary>
	/// <param name="deltaTime"> The time since the last frame. </param>
	private void MovePaddle(int deltaTime) {
		if (LeftPressed && Paddle.Left > 0) Paddle.Left -= (int) (PaddleSpeed * deltaTime);
		if (RightPressed && Paddle.Bounds.Right < ClientSize.Width) Paddle.Left += (int) (PaddleSpeed * deltaTime);
	}

	/// <summary>
	///     Moves the ScoreLabels according to the user input with the given deltaTime.
	/// </summary>
	/// <param name="deltaTime"> The time since the last frame. </param>
	private void MoveScoreLabels(int deltaTime) {
		for (var i = 0; i < _scoreLabels.Count; i++) {
			var scoreLabel = _scoreLabels[i];
			var shouldDispose = scoreLabel.Move(deltaTime);

			if (!shouldDispose) continue;

			_scoreLabels.Remove(scoreLabel);
			Controls.Remove(scoreLabel);
		}
	}

	/// <summary>
	///     Moves the PowerUps according to the user input with the given deltaTime.
	/// </summary>
	/// <param name="deltaTime"> The time since the last frame. </param>
	private void MovePowerUps(int deltaTime) {
		for (var i = 0; i < _powerUps.Count; i++) {
			var powerUp = _powerUps[i];
			if (!powerUp.Move(deltaTime)) continue;

			_powerUps.Remove(powerUp);
			Controls.Remove(powerUp);
		}
	}

	/// <summary>
	///     Handle the physics of the ball and the bricks.
	/// </summary>
	/// <param name="ball"> The ball to handle the physics of. </param>
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

		void BounceBall() {
			switch (touchingSide) {
				case Side.Bottom or Side.Top:
					ball.Velocity.Y *= -1;
					break;

				case Side.Left or Side.Right:
					ball.Velocity.X *= -1;
					break;
			}
		}

		// if the ball noClip, always set health to 1 (as no brick can be destroyed via noclip has more than 1 health)
		// if the brick is not broken (so not destructible by noClip), bounce the ball
		// else just bounce the ball
		if (IsNoClip) {
			mostTouchedBrick.brick.Health = 1;
			if (!mostTouchedBrick.brick.Hit(this, ball, touchingSide)) BounceBall();
		} else {
			BounceBall();
			mostTouchedBrick.brick.Hit(this, ball, touchingSide);
		}
	}

	/// <summary>
	///     Handle the physics of the PowerUps.
	/// </summary>
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

	/// <summary>
	///     Handle the physics of the ball and the paddle.
	/// </summary>
	/// <param name="ball"> The ball to handle the physics of. </param>
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

	/// <summary>
	///     The KeyDown event handler for moving the paddle, accelerating the balls, pausing the game, launching the balls and relaunching the balls.
	/// </summary>
	/// <param name="e"> The event arguments. </param>
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
			if (_isPaused) HidePauseMenu();
			else ShowPauseMenu();
		}
	}

	/// <summary>
	///     The KeyUp event handler for moving the paddle and accelerating the balls.
	/// </summary>
	/// <param name="e"> The event arguments. </param>
	public override void KeyUp(KeyEventArgs e) {
		if (e.KeyCode == Keys.Left) LeftPressed = false;
		if (e.KeyCode == Keys.Right) RightPressed = false;
		if (e.KeyCode == Keys.B) _accelerate = false;
	}

	/// <summary>
	///     Show the pause menu.
	/// </summary>
	private void ShowPauseMenu() {
		Controls.Add(_pauseMenu);
		_pauseMenu.BringToFront();
		physicsTimer.Stop();
		movingObjectsTimer.Stop();
		_isPaused = true;
	}

	/// <summary>
	///     Hide the pause menu.
	/// </summary>
	public void HidePauseMenu() {
		Controls.Remove(_pauseMenu);
		physicsTimer.Start();
		movingObjectsTimer.Start();
		_isPaused = false;
	}

	/// <summary>
	///     The MovingObjectsTimer loop, move the moving objects.
	/// </summary>
	/// <param name="sender"> </param>
	/// <param name="e"> </param>
	private void MovingObjectsLoop(object sender, ElapsedEventArgs e) {
		var deltaTime = (int) (e.SignalTime - e.SignalTime.AddMilliseconds(-TimerInterval)).TotalMilliseconds;

		MovePowerUps(deltaTime);
		MoveScoreLabels(deltaTime);
		PowerUpPhysics();

		ScoreLabel.Text = $"Score: {Score}";
		LivesLabel.Text = $"Lives: {Lives}";
	}

	/// <summary>
	///     The MouseMove event handler for moving the paddle.
	/// </summary>
	/// <param name="e"> The event arguments. </param>
	public override void MouseMove(MouseEventArgs e) {
		if (_isPaused) return;

		if (e.Location == _lastMouse) return;
		_lastMouse = e.Location;

		if (e.X < Paddle.Width / 2) Paddle.Left = 0;
		else if (e.X > Width - Paddle.Width / 2) Paddle.Left = Width - Paddle.Width;
		else Paddle.Left = e.X - Paddle.Width / 2;

		foreach (var ball in _balls.Where(static ball => ball.Waiting)) {
			ball.MoveToPaddle(Paddle);
		}
	}
}
