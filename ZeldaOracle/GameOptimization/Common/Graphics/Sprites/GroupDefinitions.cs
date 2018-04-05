using System;
using System.Collections.Generic;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A definition of a group and what sprite to be use.</summary>
	public struct GroupDefinition {
		/// <summary>The type for this definition.</summary>
		public string Group { get; set; }
		/// <summary>The definition in the group for what sprite to be used.</summary>
		public string Definition { get; set; }

		/// <summary>Constructs the group definition.</summary>
		public GroupDefinition(string group, string definition) {
			this.Group      = group;
			this.Definition = definition;
		}
	}

	/// <summary>A collection of group definitions for what sprite to be use.</summary>
	public abstract class GroupDefinitions {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The collection of definitions.</summary>
		private Dictionary<string, string> definitions;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a new collection of group definitions.</summary>
		public GroupDefinitions() {
			this.definitions    = new Dictionary<string, string>();
		}

		/// <summary>Constructs a copy of the collection of group definitions.</summary>
		public GroupDefinitions(GroupDefinitions copy) {
			this.definitions    = new Dictionary<string, string>(copy.definitions);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the collection of group definitions.</summary>
		public IEnumerable<GroupDefinition> GetDefinitions() {
			foreach (var pair in definitions) {
				yield return new GroupDefinition(pair.Key, pair.Value);
			}
		}

		/// <summary>Gets the definition for the specified group.<para/>
		/// Returns null if that group is not defined.</summary>
		public string Get(string group) {
			if (group == null)
				throw new ArgumentNullException("Group cannot be null!");
			string definition;
			definitions.TryGetValue(group, out definition);
			return definition;
		}

		/// <summary>Returns true if a defintion for the specified group exists.</summary>
		public bool Contains(string group) {
			if (group == null)
				throw new ArgumentNullException("Group cannot be null!");
			return definitions.ContainsKey(group);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the definition for the specified group.</summary>
		public void Set(string group, string definition) {
			if (group == null)
				throw new ArgumentNullException("Group cannot be null!");
			if (definition == null)
				throw new ArgumentNullException("Definition cannot be null!");
			definitions[group] = definition;
		}

		/// <summary>Removes the definition for the specified group.</summary>
		public void Remove(string group) {
			if (group == null)
				throw new ArgumentNullException("Group cannot be null!");
			definitions.Remove(group);
		}

		/// <summary>Removes the definition collection.</summary>
		public void Clear() {
			definitions.Clear();
		}
	}

	// Define new classes to simplify SpriteDrawSettings overloads

	/// <summary>A collection of color definitions for what color groups to use.</summary>
	public class ColorDefinitions : GroupDefinitions {
		
		/// <summary>An empty collection of group definitions.</summary>
		public static ColorDefinitions Empty {
			get { return new ColorDefinitions(); }
		}


		/// <summary>Constructs a new collection of color definitions.</summary>
		public ColorDefinitions() { }

		/// <summary>Constructs a copy of the collection of color definitions.</summary>
		public ColorDefinitions(ColorDefinitions copy) : base(copy) { }

		/// <summary>Returns a color definition collection with all groups set to
		/// the same definition.</summary>
		public static ColorDefinitions All(string definition) {
			ColorDefinitions definitions = new ColorDefinitions();
			definitions.Set("all", definition);
			return definitions;
		}


		/// <summary>Sets all groups to the same definition.</summary>
		public void SetAll(string definition) {
			Set("all", definition);
		}

		/// <summary>Removes the all definition.</summary>
		public void RemoveAll() {
			Remove("all");
		}
	}

	/// <summary>A collection of style definitions for what styles to use.</summary>
	public class StyleDefinitions : GroupDefinitions {

		/// <summary>An empty collection of group definitions.</summary>
		public static StyleDefinitions Empty {
			get { return new StyleDefinitions(); }
		}


		/// <summary>Constructs a new collection of style definitions.</summary>
		public StyleDefinitions() { }

		/// <summary>Constructs a copy of the collection of style definitions.</summary>
		public StyleDefinitions(StyleDefinitions copy) : base(copy) { }
	}
}
