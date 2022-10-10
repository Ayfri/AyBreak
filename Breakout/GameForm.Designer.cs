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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
			this.paddle = new System.Windows.Forms.PictureBox();
			this.ball = new System.Windows.Forms.PictureBox();
			this.timer = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.paddle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ball)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.timer)).BeginInit();
			this.SuspendLayout();
			// 
			// paddle
			// 
			this.paddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.paddle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.paddle.Location = new System.Drawing.Point(427, 867);
			this.paddle.Name = "paddle";
			this.paddle.Size = new System.Drawing.Size(96, 21);
			this.paddle.TabIndex = 0;
			this.paddle.TabStop = false;
			// 
			// ball
			// 
			this.ball.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ball.BackgroundImage")));
			this.ball.Location = new System.Drawing.Point(463, 834);
			this.ball.Name = "ball";
			this.ball.Size = new System.Drawing.Size(16, 16);
			this.ball.TabIndex = 1;
			this.ball.TabStop = false;
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 20D;
			this.timer.SynchronizingObject = this;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// GameForm
			// 
			this.AccessibleDescription = "A game";
			this.AccessibleName = "Breakout";
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(3)))), ((int)(((byte)(8)))));
			this.ClientSize = new System.Drawing.Size(1914, 1051);
			this.Controls.Add(this.ball);
			this.Controls.Add(this.paddle);
			this.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
		}

		private System.Timers.Timer timer;

		private System.Windows.Forms.PictureBox ball;

		private System.Windows.Forms.PictureBox paddle;

		#endregion
	}
}