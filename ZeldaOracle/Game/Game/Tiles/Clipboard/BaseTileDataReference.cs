using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles.Clipboard {
	[Serializable]
	public abstract class BaseTileDataReference<TData, TInstance>
		where TData : BaseTileData where TInstance : BaseTileDataInstance
	{

		/// <summary>The name of the tile data resource.</summary>
		private string				name;
		/// <summary>The properties of the tile data.</summary>
		private Properties			properties;
		/// <summary>The events of the tile data.</summary>
		protected EventCollection	events;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Consctructs an instance of </summary>
		public BaseTileDataReference(BaseTileDataInstance baseTileData) {
			this.name		= baseTileData.BaseData.Name;
			this.properties	= new Properties(baseTileData);
			this.events		= new EventCollection(baseTileData.Events, baseTileData);
			this.properties.Clone(baseTileData.Properties);
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
			TInstance baseTileData = SetupInstance(Resources.Get<TData>(name));
			baseTileData.Properties	= properties;
			baseTileData.Events		= events;
			properties.RestoreFromClipboard(baseTileData.BaseData.Properties, baseTileData);
			events.RestoreFromClipboard(baseTileData.BaseData.Events, baseTileData);
			return baseTileData;
		}


		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates the tile data instance and sets up any extended members.</summary>
		protected abstract TInstance SetupInstance(TData tileData);


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
