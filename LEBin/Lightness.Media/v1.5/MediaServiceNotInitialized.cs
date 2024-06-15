using System;

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
}
