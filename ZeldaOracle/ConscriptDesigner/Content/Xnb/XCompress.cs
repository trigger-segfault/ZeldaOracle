/*******************************************************************************
 *	Copyright (C) 2017  sullerandras
 *	
 *	This program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU General Public License as published by
 *	the Free Software Foundation, either version 3 of the License, or
 *	(at your option) any later version.
 *	
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU General Public License for more details.
 *	
 *	You should have received a copy of the GNU General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ******************************************************************************/
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ConscriptDesigner.Content.Xnb {
	/// <summary>A static class for xcompress-based compression and decompression.</summary>
	public static class XCompress {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The path of the temporary dll.</summary>
		private static readonly string TempXCompressDll =
			Path.Combine(Path.GetTempPath(), "TriggersToolsGames", "xcompress32.dll");
		/// <summary>True if xcompress is available to use.</summary>
		public static readonly bool IsAvailable;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Extracts and loads the xcompress dll.</summary>
		static XCompress() {
			IsAvailable = false;
			/*try {
				EmbeddedResources.Extract(TempXCompressDll, Resources.xcompress32);
				EmbeddedResources.LoadDll(TempXCompressDll);
				Compress(new byte[1]);
				IsAvailable = true;
			}
			catch (DllNotFoundException) {
				IsAvailable = false;
			}*/
		}


		//-----------------------------------------------------------------------------
		// Compression
		//-----------------------------------------------------------------------------

		/// <summary>Compresses the data.</summary>
		public static byte[] Compress(byte[] decompressedData) {
			// Setup our compression context
			int compressionContext = 0;

			XMEMCODEC_PARAMETERS_LZX codecParams;
			codecParams.Flags = 0;
			codecParams.WindowSize = 64 * 1024;
			codecParams.CompressionPartitionSize = 256 * 1024;

			XMemCreateCompressionContext(
				XMEMCODEC_TYPE.XMEMCODEC_LZX,
				ref codecParams, 0, ref compressionContext);

			// Now lets compress
			int compressedLen = decompressedData.Length * 2;
			byte[] compressed = new byte[compressedLen];
			int decompressedLen = decompressedData.Length;
			XMemCompress(compressionContext,
				compressed, ref compressedLen,
				decompressedData, decompressedLen);
			// Go ahead and destory our context
			XMemDestroyCompressionContext(compressionContext);

			// Resize our compressed data
			Array.Resize<byte>(ref compressed, compressedLen);

			// Now lets return it
			return compressed;
		}

		/// <summary>Decompresses the data.</summary>
		public static byte[] Decompress(byte[] compressedData, byte[] decompressedData) {
			// Setup our decompression context
			int DecompressionContext = 0;

			XMEMCODEC_PARAMETERS_LZX codecParams;
			codecParams.Flags = 0;
			codecParams.WindowSize = 64 * 1024;
			codecParams.CompressionPartitionSize = 256 * 1024;

			XMemCreateDecompressionContext(
				XMEMCODEC_TYPE.XMEMCODEC_LZX,
				ref codecParams, 0, ref DecompressionContext);

			// Now lets decompress
			int compressedLen = compressedData.Length;
			int decompressedLen = decompressedData.Length;
			try {
				XMemDecompress(DecompressionContext,
					decompressedData, ref decompressedLen,
					compressedData, compressedLen);
			}
			catch {
			}
			// Go ahead and destory our context
			XMemDestroyDecompressionContext(DecompressionContext);
			// Return our decompressed bytes
			return decompressedData;
		}


		//-----------------------------------------------------------------------------
		// Native
		//-----------------------------------------------------------------------------

		private enum XMEMCODEC_TYPE {
			XMEMCODEC_DEFAULT = 0,
			XMEMCODEC_LZX = 1
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct XMEMCODEC_PARAMETERS_LZX {
			[FieldOffset(0)]
			public int Flags;
			[FieldOffset(4)]
			public int WindowSize;
			[FieldOffset(8)]
			public int CompressionPartitionSize;
		}

		[DllImport("xcompress32.dll", EntryPoint = "XMemCompress")]
		private static extern int XMemCompress(int Context,
											  byte[] pDestination, ref int pDestSize,
											  byte[] pSource, int pSrcSize);

		[DllImport("xcompress32.dll", EntryPoint = "XMemCreateCompressionContext")]
		private static extern int XMemCreateCompressionContext(
			XMEMCODEC_TYPE CodecType, ref XMEMCODEC_PARAMETERS_LZX pCodecParams,
			int Flags, ref int pContext);

		[DllImport("xcompress32.dll", EntryPoint = "XMemDestroyCompressionContext")]
		private static extern void XMemDestroyCompressionContext(int Context);

		[DllImport("xcompress32.dll", EntryPoint = "XMemDecompress")]
		private static extern int XMemDecompress(int Context,
												byte[] pDestination, ref int pDestSize,
												byte[] pSource, int pSrcSize);

		[DllImport("xcompress32.dll", EntryPoint = "XMemCreateDecompressionContext")]
		private static extern int XMemCreateDecompressionContext(
			XMEMCODEC_TYPE CodecType,
			ref XMEMCODEC_PARAMETERS_LZX pCodecParams,
			int Flags, ref int pContext);

		[DllImport("xcompress32.dll", EntryPoint = "XMemDestroyDecompressionContext")]
		private static extern void XMemDestroyDecompressionContext(int Context);
	}
}
