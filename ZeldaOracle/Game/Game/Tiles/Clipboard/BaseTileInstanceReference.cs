using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles.Clipboard {
	/// <summary>The base class for tile instance references.</summary>
	[Serializable]
	public abstract class BaseTileInstanceReference<TData, TInstance>
		where TData : BaseTileData where TInstance : BaseTileDataInstance
	{
		/// <summary>The name of the tile data resource.</summary>
		private string				name;
		/// <summary>The properties of the tile data.</summary>
		private Properties			properties;
		/// <summary>The events of the tile data.</summary>
		private EventCollection		events;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a reference to the tile or action tile instance.</summary>
		public BaseTileInstanceReference(BaseTileDataInstance baseTile) {
			name		= baseTile.BaseData.ResourceName;
			properties	= new Properties(baseTile.Properties, baseTile);
			events		= new EventCollection(baseTile.Events, baseTile);
		}
		

		//-----------------------------------------------------------------------------
		// Pasting
		//-----------------------------------------------------------------------------

		/// <summary>Confirms that the resource exists in the database.</summary>
		public bool ConfirmResourceExists() {
			return Resources.Contains<TData>(name);
		}

		/// <summary>Creates and returns the dereferenced tile data.</summary>
		public TInstance Dereference() {
			TData data = Resources.Get<TData>(name);
			TInstance baseTile	= SetupInstance(data);
			baseTile.Properties	= properties;
			baseTile.Events		= events;
			properties.RestoreFromClipboard(data.Properties, baseTile);
			events.RestoreFromClipboard(data.Events, baseTile);
			return baseTile;
		}


		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates the tile data instance and sets up any extended members.</summary>
		protected abstract TInstance SetupInstance(TData baseData);


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the name of the tile data resource.</summary>
		public string Name {
			get { return name; }
		}

		/// <summary>Gets the properties of the tile data.</summary>
		public Properties Properties {
			get { return properties; }
		}

		/// <summary>Gets the events of the tile data.</summary>
		public EventCollection Events {
			get { return events; }
		}
	}
}
