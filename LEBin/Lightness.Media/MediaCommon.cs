using DFramework;
using Lightness.Core;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Lightness.Media
{

	public class MediaServiceNotInitialized : Exception
	{
		public override string Message
		{
			get
			{
				return "Initialize MediaService before using BGM, SE, ...";
			}
		}
	}

	public static class MediaCommon
	{

		public static int SameFrameSECount = 0;

		internal static bool Initialized;

		public static void Initialize(LEWindow MainWindow)
		{
			Audio.CSInitialize();
			MediaCommon.Initialized = true;
		}

		public static void CloseAll()
		{
			if (!MediaCommon.Initialized)
			{
				throw new MediaServiceNotInitialized();
			}
			Audio.CloseAll();
		}
	}
}
