
namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing a collection of ZeldaAPI variables.</summary>
	public interface IVariableObject {

		/// <summary>Gets the collection of Zelda variables.</summary>
		Variables Variables { get; }
	}
}
