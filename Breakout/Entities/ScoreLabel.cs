namespace Breakout.Entities;

using System.Drawing;
using System.Windows.Forms;

/// <summary>
///     The ScoreLabel class is a Label that displays a temporary score, moving up.
/// </summary>
public sealed class ScoreLabel : Label {
	/// <summary>
	///     The speed at which the score moves up.
	/// </summary>
	private const float Speed = .4f;

	/// <summary>
	///     The maximum height at which the score will be displayed.
	/// </summary>
	private readonly int _maxY;

	/// <summary> The ScoreLabel function creates a new ScoreLabel object with the specified initial position, maximum Y value, and score. </summary>
	/// <param name="initialPosition"> The initial location of the label. </param>
	/// <param name="maxY"> The maximum Y value at which the label will be displayed. </param>
	/// <param name="score"> The score to display. </param>
	/// <returns> The score value. </returns>
	public ScoreLabel(Point initialPosition, int maxY, int score) {
		_maxY = maxY;
		Location = initialPosition;
		BackColor = Color.Transparent;
		ForeColor = Color.White;
		Text = $"+{score}";
		AutoSize = true;
		FontHeight = 20;
	}

	/// <summary> The Move function moves the object down by Speed * deltaTime. </summary>
	/// <param name="deltaTime"> Time since last frame </param>
	/// <returns>
	///     Returns true if the top of the sprite has reached its maximum y value, and false otherwise.
	/// </returns>
	public new bool Move(int deltaTime) {
		Top -= (int) (Speed * deltaTime);
		return Top <= _maxY;
	}
}