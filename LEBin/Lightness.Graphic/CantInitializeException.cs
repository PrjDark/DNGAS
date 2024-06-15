using System;

namespace Lightness.Graphic
{
	public class CantInitializeException : Exception
	{
		public override string Message
		{
			get
			{
				return "Failed to initialize Graphic Engine. Your system may do not satisfy the system requirements.";
			}
		}
	}
}
