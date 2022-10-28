namespace Breakout;

using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
///     The MainMenuScene class is the first scene that is loaded when the game starts.
/// </summary>
public sealed class MainMenuScene : AbstractScene {
	/// <summary>
	///     The credits label, placed at the bottom right of the screen.
	/// </summary>
	private readonly Label _creditsLabel = new() {
		AutoSize = true,
		Text = "Ayfri",
		Font = new(Program.MainFont, 30),
		ForeColor = Color.White,
		Location = new((int) (MainForm.GameSize.Width * .9), (int) (MainForm.GameSize.Height * .9))
	};

	/// <summary>
	///     The Exit button, placed at the below the start button.
	/// </summary>
	private readonly MenuButton _exitButton = new(
		"Exit",
		150,
		static (_, _) => { Application.Exit(); }
	);

	/// <summary>
	///     The Play button, placed at the middle of the screen.
	/// </summary>
	private readonly MenuButton _startButton = new(
		"Start",
		0,
		static (_, _) => Program.MainForm.ChangeScene(new LevelSelectionScene())
	);

	/// <summary>
	///     The title label, placed at the top of the screen.
	/// </summary>
	private readonly Label _titleLabel = new() {
		Text = "AyBreak",
		Font = new(Program.MainFont, 80),
		Width = 450,
		Height = 130,
		ForeColor = Color.White,
		Location = new(MainForm.GameSize.Width / 2 - 210, MainForm.GameSize.Height / 6)
	};

	/// <summary> The MainMenuScene function creates the MainMenuScene object and adds all of its controls to the form. </summary>
	/// <returns> A MainMenuScene object. </returns>
	public MainMenuScene() {
		BackColor = Color.FromArgb(15, 15, 20);
		Size = MainForm.GameSize;

		Controls.AddRange(
			new Control[] {
				_startButton, _exitButton, _titleLabel, _creditsLabel
			}
		);
	}

	/// <summary>
	///     A MenuButton is a button that is used in the MainMenuScene.
	/// </summary>
	private sealed class MenuButton : Button {
		/// <summary> The MenuButton function creates a button that is used in the main menu. </summary>
		/// <param name="text"> The text to display on the button. </param>
		/// <param name="offsetY"> The offset from the center of the screen on the Y axis. </param>
		/// <param name="onClick"> The function to call when the button is clicked. </param>
		/// <returns> A MenuButton object. </returns>
		public MenuButton(string text, int offsetY, EventHandler onClick) {
			Text = text;
			Size = new(200, 80);
			Location = new(MainForm.GameSize.Width / 2 - Width / 2, MainForm.GameSize.Height / 2 - Height / 2 + offsetY);
			Font = new(Program.MainFont, 20);
			BackColor = Color.FromArgb(20, 20, 30);
			ForeColor = Color.White;
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 1;
			FlatAppearance.BorderColor = Color.White;
			Click += onClick;
		}
	}
}
