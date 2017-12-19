using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Anchorables {
	public interface IRequestClosePanel {

		void ForceClose();

		void Close();

		object Content { get; set; }

		bool IsActive { get; set; }

	}
}
