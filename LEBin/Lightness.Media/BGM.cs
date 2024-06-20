using Lightness.Core;
using System;

namespace Lightness.Media {
	public class BGM {
		public int DevID;

		public string Alias = "";

		public string FileName = "";

		public BGM() {
			this.Alias = MediaCommon.GetRandom();
			Debug.Log('I', "Sound", "Initialize Sound Engine (BGM): {0}", new object[]
			{
				this.Alias
			});
		}

		public int Send(string Command) {
			int num = MediaCommon.mciSendString(Command, null, 0, IntPtr.Zero);
			if(num != 0) {
				Debug.Log('W', "Sound", "Driver Returns: {0}", new object[]
				{
					num
				});
			}
			return num;
		}

		public int SendN(string Command) {
			int num = MediaCommon.mciSendString(Command, null, 0, MediaCommon.MediaWindow.Handle);
			if(num != 0) {
				Debug.Log('W', "Sound", "Driver Returns: {0}", new object[]
				{
					num
				});
			}
			return num;
		}

		public void LoadFile(string FileName) {
			Debug.Log('I', "Sound", "Load Sound: \"{0}\"", new object[]
			{
				FileName
			});
			this.FileName = FileName;
			this.Send("open \"./Data/Sound/" + FileName + "+\" type waveaudio Alias " + this.Alias);
			this.DevID = MediaCommon.mciGetDeviceID(this.Alias);
			MediaCommon.MCIDeviceIdToAlias[this.DevID] = this.Alias;
		}

		public void Play() {
			MediaCommon.MCIDeviceIdLoopFlag[this.DevID] = true;
			Debug.Log('I', "Sound", "Play BGM: {0}", new object[]
			{
				this.FileName
			});
			this.SendN("play " + this.Alias + " notify");
		}

		public void Stop() {
			Debug.Log('I', "Sound", "Stop: {0}", new object[]
			{
				this.FileName
			});
			this.Send("stop " + this.Alias);
			this.Send("seek " + this.Alias + " to start");
		}

		public void Pause() {
			Debug.Log('I', "Sound", "Pause: {0}", new object[]
			{
				this.FileName
			});
			this.Send("stop " + this.Alias);
		}

		public void Close() {
			Debug.Log('I', "Sound", "Close: {0}", new object[]
			{
				this.FileName
			});
			this.Send("close " + this.Alias);
			MediaCommon.MCIDeviceIdToAlias[this.DevID] = null;
		}
	}
}
