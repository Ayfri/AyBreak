using System.Drawing;

namespace Breakout;

public class BrickType {
	public string Name { get; set; } = "";
	public Color Color { get; set; }

	public int Score { get; set; }

	public bool IsEmpty => Name == "Empty";
}
