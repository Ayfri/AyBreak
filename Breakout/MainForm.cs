namespace Breakout;

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Entities;

public sealed partial class MainForm : Form {
	public static Size GameSize = new(1920, 1080);
	private AbstractScene _scene = new MainMenuScene();

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

	private void MainForm_KeyDown(object sender, KeyEventArgs e) {
		_scene.KeyDown(e);
	}

	private void MainForm_KeyUp(object sender, KeyEventArgs e) {
		_scene.KeyUp(e);
	}
}
