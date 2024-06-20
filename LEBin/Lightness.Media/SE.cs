using Lightness.Core;
using System;

namespace Lightness.Media {
	public class SE : BGM {
		public SE() {
			this.Alias = MediaCommon.GetRandom();
			Debug.Log('I', "Sound", "Initialize Sound Engine (SE): {0}", new object[]
			{
				this.Alias
			});
		}

		public new void Play() {
			if(MediaCommon.SameFrameSECount == 0) {
				Debug.Log('I', "Sound", "Play SE: {0}", new object[]
				{
					this.FileName
				});
				base.Send("seek " + this.Alias + " to start");
				base.Send("play " + this.Alias);
				return;
			}
			Debug.Log('W', "Sound", "Only play one SE in one frame", new object[0]);
		}
	}
}
