﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Breakout;

public class MainMenuForm : AbstractScene {
	private class MenuButton : Button {
		public MenuButton(string text, int offsetY, EventHandler onClick) {
			Text = text;
			Size = new(200, 80);
			Location = new(MainForm.GameSize.Width / 2 - Width / 2, MainForm.GameSize.Height / 2 - Height / 2 + offsetY);
			Font = new("Arial", 20);
			BackColor = Color.FromArgb(20, 20, 30);
			ForeColor = Color.White;
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 1;
			FlatAppearance.BorderColor = Color.White;
			Click += onClick;
		}
	}

	private MenuButton _exitButton = new(
		"Exit",
		150,
		static (_, _) => {
			Application.Exit();
		}
	);

	private MenuButton _startButton = new("Start", 0, static (_, _) => {
		Program.MainForm.ChangeScene(new GameForm());
	});
	
	private Label _titleLabel = new() {
		Text = "Breakout",
		Font = new("Candara", 80),
		Width = 480,
		Height = 120,
		ForeColor = Color.White,
		// TextAlign = ContentAlignment.MiddleCenter,
		Location = new(MainForm.GameSize.Width / 2 - 230, MainForm.GameSize.Height / 6)
	};

	private Label _creditsLabel = new() {
		AutoSize = true,
		Text = "Ayfri",
		Font = new("Candara", 30),
		ForeColor = Color.White,
		Location = new((int)(MainForm.GameSize.Width * .9), (int)(MainForm.GameSize.Height * .9))
	};
	
	public MainMenuForm() {
		BackColor = Color.FromArgb(15, 15, 20);
		Size = MainForm.GameSize;
		Controls.AddRange(new Control[] {_startButton, _exitButton, _titleLabel, _creditsLabel});
	}
}
