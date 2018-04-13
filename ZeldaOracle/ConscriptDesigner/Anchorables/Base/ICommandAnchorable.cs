
namespace ConscriptDesigner.Anchorables {
	public interface ICommandAnchorable {

		/// <summary>Performs the cut command.</summary>
		void Cut();

		/// <summary>Performs the copy command.</summary>
		void Copy();

		/// <summary>Performs the paste command.</summary>
		void Paste();

		/// <summary>Performs the delete command.</summary>
		void Delete();

		/// <summary>Performs the undo command.</summary>
		void Undo();

		/// <summary>Performs the redo command.</summary>
		void Redo();


		/// <summary>Returns true if the anchorable can cut.</summary>
		bool CanCut { get; }

		/// <summary>Returns true if the anchorable can copy.</summary>
		bool CanCopy { get; }

		/// <summary>Returns true if the anchorable can paste.</summary>
		bool CanPaste { get; }

		/// <summary>Returns true if the anchorable can delete.</summary>
		bool CanDelete { get; }

		/// <summary>Returns true if the anchorable can undo.</summary>
		bool CanUndo { get; }

		/// <summary>Returns true if the anchorable can redo.</summary>
		bool CanRedo { get; }
	}
}
