using DFramework;
using Lightness.Core;
using System;

namespace Lightness.Media {
	public class BGM {
		protected internal IntPtr DFrameworkAudioID = IntPtr.Zero;

		public BGM() {
			Lightness.Core.Debug.Log('I', "Media", "Initialize Sound Engine (BGM)...", new object[0]);
			if(!MediaCommon.Initialized) {
				throw new MediaServiceNotInitialized();
			}
		}


		public void LoadFile(string FileName) {
			this.DFrameworkAudioID = Audio.CreatePlayer();
			Audio.LoadFile(this.DFrameworkAudioID, "./Data/Sound/" + FileName);
			Audio.SetLoop(this.DFrameworkAudioID, 4294967295u, 0u);
		}

		public void Play() {
			Audio.Play(this.DFrameworkAudioID);
		}

		public void Stop() {
			Audio.Stop(this.DFrameworkAudioID);
		}

		public void Pause() {
			Audio.Pause(this.DFrameworkAudioID);
		}

		public void Close() {
			Audio.Close(this.DFrameworkAudioID);
		}

		public int GetLength() {
			return Audio.GetLength(this.DFrameworkAudioID);
		}

		public int GetPosition() {
			return Audio.GetPosition(this.DFrameworkAudioID);
		}

		public void SetPosition(uint Position) {
			Audio.SetPosition(this.DFrameworkAudioID, Position);
		}

		public int GetVolume() {
			return Audio.GetPosition(this.DFrameworkAudioID);
		}

		public void SetVolume(int Volume) {
			Audio.SetPosition(this.DFrameworkAudioID, (uint)Volume);
		}

		public void SetLoopStart(uint p) {
			Audio.SetLoop(this.DFrameworkAudioID, 4294967295u, p);
		}
	}
}
