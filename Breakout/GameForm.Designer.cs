namespace Breakout {
	partial class GameForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.paddle = new System.Windows.Forms.PictureBox();
			this.ball = new System.Windows.Forms.PictureBox();
			this.timer = new System.Timers.Timer();
			this.ScoreLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.paddle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ball)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.timer)).BeginInit();
			this.SuspendLayout();
			// 
			// paddle
			// 
			this.paddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.paddle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.paddle.Location = new System.Drawing.Point(430, 896);
			this.paddle.Name = "paddle";
			this.paddle.Size = new System.Drawing.Size(110, 20);
			this.paddle.TabIndex = 0;
			this.paddle.TabStop = false;
			// 
			// ball
			// 
			this.ball.BackColor = System.Drawing.Color.Transparent;
			this.ball.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ball.Image = global::Breakout.Properties.Resources.ball;
			this.ball.Location = new System.Drawing.Point(437, 776);
			this.ball.Name = "ball";
			this.ball.Size = new System.Drawing.Size(16, 16);
			this.ball.TabIndex = 1;
			this.ball.TabStop = false;
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.SynchronizingObject = this;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// ScoreLabel
			// 
			this.ScoreLabel.AutoSize = true;
			this.ScoreLabel.BackColor = System.Drawing.Color.Black;
			this.ScoreLabel.ForeColor = System.Drawing.Color.White;
			this.ScoreLabel.Location = new System.Drawing.Point(18, 25);
			this.ScoreLabel.Name = "ScoreLabel";
			this.ScoreLabel.Size = new System.Drawing.Size(48, 18);
			this.ScoreLabel.TabIndex = 2;
			this.ScoreLabel.Text = "Score";
			this.ScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// GameForm
			// 
			this.AccessibleDescription = "A game";
			this.AccessibleName = "Breakout";
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(3)))), ((int)(((byte)(8)))));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(1920, 1061);
			this.Controls.Add(this.ScoreLabel);
			this.Controls.Add(this.ball);
			this.Controls.Add(this.paddle);
			this.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.Color.Transparent;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MinimizeBox = false;
			this.Name = "GameForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Breakout";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.paddle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ball)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.timer)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.Label ScoreLabel;

		private System.Timers.Timer timer;

		private System.Windows.Forms.PictureBox ball;

		private System.Windows.Forms.PictureBox paddle;

		#endregion
	}
}
