using ZeldaEditor.Control;

namespace ZeldaEditor.Undo {
	/// <summary>An action in the editor that makes changes to the world file.<para/>
	/// All changes to the world file must be represented as an action.</summary>
	public abstract class EditorAction : UndoAction<EditorControl> {

	}
}
