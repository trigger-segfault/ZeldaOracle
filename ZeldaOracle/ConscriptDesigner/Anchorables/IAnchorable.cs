using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Content;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	public interface IContentAnchorable {

		RequestCloseAnchorable Anchorable { get; }

		ContentFile ContentFile { get; }

		bool IsModified { get; }

		bool CanUndo { get; }
		bool CanRedo { get; }

		void Save();

		void Undo();
		void Redo();
	}
}
