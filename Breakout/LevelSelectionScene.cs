namespace Breakout;

using System.Drawing;
using System.Windows.Forms;

/// <summary>
///     The LevelSelectionScene is the scene that is displayed when the user wants to select a level to play.
/// </summary>
public sealed class LevelSelectionScene : AbstractScene {
	/// <summary>
	///     The number of levels in the game.
	/// </summary>
	private readonly int _levelCount = LevelManager.Levels.Length;

	/// <summary>
	///     The constructor for the LevelSelectionScene.
	///     Creates the Scene, sets the background color and adds the buttons.
	/// </summary>
	/// <returns> A LevelSelectionScene object. </returns>
	public LevelSelectionScene() {
		BackColor = Color.FromArgb(20, 20, 25);
		Size = Program.MainForm.Size;

		var subtitle = new Label {
			Text = "Select a level",
			Width = 350,
			Height = 100,
			Font = new(Program.MainFont, 40),
			ForeColor = Color.White
		};

		subtitle.Location = new(ClientSize.Width / 2 - subtitle.Width / 2, ClientSize.Height / 6);

		Controls.Add(subtitle);

		const int totalWidth = LevelButton.BtnWidth * 5 + LevelButton.BtnMargin * 4;

		for (var i = 0; i < _levelCount; i++) {
			var button = new LevelButton(i);
			if (i == 0) button.Focus();
			var x = i % 5 * (LevelButton.BtnWidth + LevelButton.BtnMargin);
			var y = i / 5 * (LevelButton.BtnHeight + LevelButton.BtnMargin);
			button.Location = new(ClientSize.Width / 2 - totalWidth / 2 + x, ClientSize.Height / 2 - 150 + y);
			Controls.Add(button);
		}
	}

	/// <summary>
	///     A button that represents a level.
	/// </summary>
	private sealed class LevelButton : Button {
		/// <summary>
		///     The width of the button.
		/// </summary>
		public const int BtnWidth = 300;

		/// <summary>
		///     The height of the button.
		/// </summary>
		public const int BtnHeight = 120;

		/// <summary>
		///     The margin between the buttons.
		/// </summary>
		public const int BtnMargin = 20;

		/// <summary>
		///     The LevelButton function creates a button that will load the level specified by the parameter 'levelNumber' when clicked.
		/// </summary>
		/// <param name="levelNumber"> The number of the level to load. </param>
		/// <returns> A button object. </returns>
		public LevelButton(int levelNumber) {
			Text = $"Level {levelNumber + 1}";
			Width = BtnWidth;
			Height = BtnHeight;
			Font = new(Program.MainFont, 30);
			BackColor = Color.FromArgb(20, 20, 25);
			ForeColor = Color.White;
			FlatStyle = FlatStyle.Flat;

			Click += (_, _) => {
				var level = LevelManager.LoadLevel(levelNumber);
				Program.MainForm.ChangeScene(new GameScene(level));
			};
		}
	}
}
