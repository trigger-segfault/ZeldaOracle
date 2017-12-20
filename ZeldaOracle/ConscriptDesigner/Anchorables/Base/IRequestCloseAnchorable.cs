using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Anchorables {
	public interface IRequestCloseAnchorable {

		void ForceClose();

		void Close();

		bool IsActive { get; set; }

	}
}
