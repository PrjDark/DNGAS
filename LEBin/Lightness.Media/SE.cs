using DFramework;
using Lightness.Core;
using System;

namespace Lightness.Media
{
	public class SE : BGM
	{
		public SE()
		{
			Lightness.Core.Debug.Log('I', "Media", "Initialize Sound Engine (SE)...", new object[0]);
			if (!MediaCommon.Initialized)
			{
				throw new MediaServiceNotInitialized();
			}
		}

		public new void LoadFile(string FileName)
		{
			this.DFrameworkAudioID = Audio.CreatePlayer();
			Audio.LoadFile(this.DFrameworkAudioID, "./Data/Sound/" + FileName);
			Audio.SetLoop(this.DFrameworkAudioID, 0u, 0u);
		}

		public new void Play()
		{
			Audio.SetPosition(this.DFrameworkAudioID, 0u);
			Audio.Play(this.DFrameworkAudioID);
		}
	}
}
