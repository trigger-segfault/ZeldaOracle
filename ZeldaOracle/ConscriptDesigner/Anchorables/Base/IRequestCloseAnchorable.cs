using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Anchorables {
	/// <summary>An interface for an anchorable or document
	/// that requests close without closing.</summary>
	public interface IRequestCloseAnchorable {

		/// <summary>Force-closes the anchorable.</summary>
		void ForceClose();

		/// <summary>Closes the anchorable with a request.</summary>
		void Close();

		/// <summary>Gets or sets if the anchorable is active.</summary>
		bool IsActive { get; set; }
	}
}
