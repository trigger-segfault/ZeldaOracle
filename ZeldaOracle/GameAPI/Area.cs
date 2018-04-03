using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaAPI {
	/// <summary>Access to the current area.</summary>
	public interface Area : ApiObject {
		/// <summary>Completes the area.</summary>
		void Complete();

		/// <summary>Gets the readable name of the area.</summary>
		string Name { get; }
		/// <summary>Gets if the area has been completed.</summary>
		bool IsCompleted { get; }
	}
}
