using DFramework;
using System;

namespace Lightness.Media
{
	public class SE : BGM
	{
		public SE()
		{
			Lightness.Debug.Log('I', "Media", "Initialize Sound Engine (SE)...", new object[0]);
			if (!MediaService.Initialized)
			{
				throw new MediaServiceNotInitialized();
			}
		}

		public new void LoadFile(string FileName)
		{
			this.DFrameworkAudioID = Audio.CreatePlayer();
			Audio.LoadFile(this.DFrameworkAudioID, "./Contents/Sound/" + FileName);
		}

		public new void Play()
		{
			Audio.SetPosition(this.DFrameworkAudioID, 0u);
			Audio.Play(this.DFrameworkAudioID);
		}

		private new void SetLoopStart(uint p)
		{
		}
	}
}
