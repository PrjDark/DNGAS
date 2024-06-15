using DFramework;
using System;

namespace Lightness.Media
{
	public class MediaService
	{
		internal static bool Initialized;

		public static void Initialize(LEWindow MainWindow)
		{
			Audio.CSInitialize();
			MediaService.Initialized = true;
		}

		public static void CloseAll()
		{
			if (!MediaService.Initialized)
			{
				throw new MediaServiceNotInitialized();
			}
			Audio.CloseAll();
		}
	}
}
