namespace Breakout;

using System.Drawing;
using System.Windows.Forms;
using Breakout.Entities;

/// <summary>
///     The MainForm class is the main form for the Breakout game.
/// </summary>
public sealed partial class MainForm : Form {
	/// <summary>
	///     The Size of the game area.
	/// </summary>
	public static Size GameSize = new(1920, 1080);

	/// <summary>
	///     The Scene to be rendered.
	/// </summary>
	private AbstractScene _scene = new MainMenuScene();

	/// <summary>
	///     Creates a new instance of the MainForm class.
	///     Links the <see cref="GlobalMouseHandler" /> to the <see cref="Application" />.
	/// </summary>
	/// <returns> A new instance of the MainForm class. </returns>
	public MainForm() {
		InitializeComponent();
		Controls.Add(_scene);
		_scene.Location = new(0, 0);
		_scene.Dock = DockStyle.Fill;
		_scene.Size = Size;

		var gmh = new GlobalMouseHandler();
		gmh.MouseMovedEvent += args => _scene.MouseMove(args);
		Application.AddMessageFilter(gmh);
	}

	/// <summary> The ChangeScene function changes the scene to a new scene. </summary>
	/// <param name="newScene"> The scene to switch to. </param>
	public void ChangeScene(AbstractScene newScene) {
		if (_scene.InvokeRequired) {
			_scene.Invoke(new MethodInvoker(() => ChangeScene(newScene)));
		} else {
			_scene.Dispose();
			_scene = newScene;
			Controls.Add(_scene);
			_scene.Location = new(0, 0);
			_scene.Dock = DockStyle.Fill;
			_scene.Size = Size;
		}
	}

	/// <summary>
	///     The MainForm_KeyDown function is called when a key is pressed down.
	///     It calls the <see cref="AbstractScene.KeyDown" /> function of the current scene.
	/// </summary>
	/// <param name="sender"> The source of the event. </param>
	/// <param name="e"> The KeyEventArgs parameter is a reference to the event object that contains information about the key that was pressed. </param>
	/// <returns> The KeyEventArgs e. </returns>
	private void MainForm_KeyDown(object sender, KeyEventArgs e) {
		_scene.KeyDown(e);
	}

	/// <summary>
	///     The MainForm_KeyUp function is called when the user releases a key on the keyboard.
	///     It calls the <see cref="AbstractScene.KeyUp" /> function of the current scene.
	/// </summary>
	/// <param name="sender"> The source of the event. </param>
	/// <param name="e"> The KeyEventArgs parameter is a reference to the event object that contains information about the key that was pressed. </param>
	/// <returns> The KeyEventArgs.keycode value. </returns>
	private void MainForm_KeyUp(object sender, KeyEventArgs e) {
		_scene.KeyUp(e);
	}
}
