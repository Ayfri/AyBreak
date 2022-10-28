namespace Breakout {
	using System.Windows.Forms;

	public sealed partial class GameScene {
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
			this.Paddle = new System.Windows.Forms.PictureBox();
			this.physicsTimer = new System.Timers.Timer();
			this.ScoreLabel = new System.Windows.Forms.Label();
			this.LivesLabel = new System.Windows.Forms.Label();
			this.movingObjectsTimer = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize) (this.Paddle)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.physicsTimer)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.movingObjectsTimer)).BeginInit();
			this.SuspendLayout();
			// 
			// Paddle
			// 
			this.Paddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.Paddle.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (224)))), ((int) (((byte) (224)))), ((int) (((byte) (224)))));
			this.Paddle.Location = new System.Drawing.Point(430, 896);
			this.Paddle.Name = "Paddle";
			this.Paddle.Size = new System.Drawing.Size(110, 20);
			this.Paddle.TabIndex = 0;
			this.Paddle.TabStop = false;
			// 
			// physicsTimer
			// 
			this.physicsTimer.Enabled = true;
			this.physicsTimer.SynchronizingObject = this;
			this.physicsTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.GameLoop);
			// 
			// ScoreLabel
			// 
			this.ScoreLabel.AutoSize = true;
			this.ScoreLabel.BackColor = System.Drawing.Color.Black;
			this.ScoreLabel.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ScoreLabel.ForeColor = System.Drawing.Color.White;
			this.ScoreLabel.Location = new System.Drawing.Point(18, 25);
			this.ScoreLabel.Name = "ScoreLabel";
			this.ScoreLabel.Size = new System.Drawing.Size(90, 32);
			this.ScoreLabel.TabIndex = 2;
			this.ScoreLabel.Text = "Score";
			this.ScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LivesLabel
			// 
			this.LivesLabel.AutoSize = true;
			this.LivesLabel.BackColor = System.Drawing.Color.Transparent;
			this.LivesLabel.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.LivesLabel.ForeColor = System.Drawing.Color.White;
			this.LivesLabel.Location = new System.Drawing.Point(300, 25);
			this.LivesLabel.Name = "LivesLabel";
			this.LivesLabel.Size = new System.Drawing.Size(90, 32);
			this.LivesLabel.TabIndex = 4;
			this.LivesLabel.Text = "Lives";
			// 
			// movingObjectsTimer
			// 
			this.movingObjectsTimer.Enabled = true;
			this.movingObjectsTimer.Interval = 33D;
			this.movingObjectsTimer.SynchronizingObject = this;
			this.movingObjectsTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.MovingObjectsLoop);
			// 
			// GameScene
			// 
			this.AccessibleDescription = "A game";
			this.AccessibleName = "Breakout";
			this.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (3)))), ((int) (((byte) (3)))), ((int) (((byte) (8)))));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Controls.Add(this.LivesLabel);
			this.Controls.Add(this.ScoreLabel);
			this.Controls.Add(this.Paddle);
			this.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ForeColor = System.Drawing.Color.Transparent;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Size = new System.Drawing.Size(1920, 1061);
			this.Text = "Breakout";
			((System.ComponentModel.ISupportInitialize) (this.Paddle)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.physicsTimer)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.movingObjectsTimer)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Timers.Timer movingObjectsTimer;

		private System.Windows.Forms.Label LivesLabel;

		private System.Windows.Forms.Label ScoreLabel;

		private System.Timers.Timer physicsTimer;

		public System.Windows.Forms.PictureBox Paddle;

		#endregion
	}
}
