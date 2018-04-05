using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Content {
	/// <summary>The base container for content manager. Is to be overridden by
	/// Resources.</summary>
	public class ContentContainer {
		// CONTAINMENT:
		/// <summary>True if the resource manager has been fully initialized.</summary>
		protected static bool isInitialized;
		/// <summary>The game's content manager.</summary>
		protected static ContentManager contentManager;
		/// <summary>The game's graphics device.</summary>
		protected static GraphicsDevice graphicsDevice;
		/// <summary>The game's sprite batch for drawing.</summary>
		protected static SpriteBatch spriteBatch;
		/// <summary>True if the content manager was created by an IServiceProvider
		/// and should be disposed of when calling Uninitialize().</summary>
		protected static bool disposeContent;
		/// <summary>A collection of content that need to manually be disposed of
		/// during unload.<para/>
		/// This is a hashset to prevent ContentContainer from being added
		/// continuously.</summary>
		protected static HashSet<IDisposable> independentResources;


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Adds an indenpendent resource that will be disposed of during
		/// Unload().</summary>
		public static void AddDisposable(IDisposable disposable) {
			if (!isInitialized)
				throw new InvalidOperationException("Cannot add disposable when " +
					"ContentContainer has not been initialized!");
			independentResources.Add(disposable);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the graphics device for the game.</summary>
		public static GraphicsDevice GraphicsDevice {
			get { return graphicsDevice; }
		}

		/// <summary>Gets the sprite batch for the game.</summary>
		public static SpriteBatch SpriteBatch {
			get { return spriteBatch; }
		}

		/// <summary>Gets the content manager for the game.</summary>
		public static ContentManager ContentManager {
			get { return contentManager; }
		}

		/// <summary>Gets if the resource manager has been fully been initialized.</summary>
		public static bool IsInitialized {
			get { return isInitialized; }
		}

		/// <summary>Gets or sets the root directory associated with the
		/// ContentManager.</summary>
		public static string RootDirectory {
			get { return ContentManager.RootDirectory; }
			set { ContentManager.RootDirectory = value; }
		}
	}
}
