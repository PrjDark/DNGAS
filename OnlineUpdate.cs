using Ionic.Zip;
using Lightness.Core;
using Lightness.DNetwork;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.Resources;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace LEContents {
	public static class OnlineUpdate {
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

		public static Texture MenuTitleText = null;

		public static Texture MainText = null;

		public static Texture BG = null;

		public static Paint PGreen;

		public static Texture Green;

		public static Paint PGreenL;

		public static Texture GreenL;

		public static ZipFile Z = null;

		public static long Pct = 0L;

		public static string FilePath = "";

		public static string[] FileList = null;

		public static int Count = 0;

		public static int TotalCount = 0;

		public static int State = 0;

		public static DNet D = null;

		public static ContentReturn Initialize() {
			OnlineUpdate.PGreen = new Paint(0, 96, 0, 255);
			OnlineUpdate.Green = OnlineUpdate.PGreen.ToTexture();
			OnlineUpdate.PGreenL = new Paint();
			OnlineUpdate.PGreenL.SetColor(0, 96, 0, 255);
			OnlineUpdate.PGreenL.Box(0, 0, 904, 29);
			OnlineUpdate.GreenL = OnlineUpdate.PGreenL.ToTexture();
			OnlineUpdate.BG = Texture.CreateFromFile("ConfigBG.png");
			Texture.SetFont("Meiryo");
			Texture.SetTextSize(32);
			Texture.SetTextColor(255, 255, 255);
			OnlineUpdate.MenuTitleText = Texture.CreateFromText("オンライン アップデート");
			Texture.SetTextSize(24);
			OnlineUpdate.MainText = Texture.CreateFromText("Please wait...");
			OnlineUpdate.State = 0;
			OnlineUpdate.TotalCount = (OnlineUpdate.Count = 0);
			OnlineUpdate.FilePath = Path.GetTempPath() + "\\Lightness_Updater_" + GameCommon.Version.APPID + ".bin";
			OnlineUpdate.FileList = new string[1024];
			return ContentReturn.OK;
		}

		public static ContentReturn Main() {
			Core.Draw(OnlineUpdate.MenuTitleText, 20, 20);
			Core.Draw(OnlineUpdate.MainText, 30, 220);
			Core.Draw(OnlineUpdate.GreenL, 28, 268);
			Common.GSprite.DrawEx(OnlineUpdate.Green, 30, 270, 255, 0, 100, 900 * (int)OnlineUpdate.Pct / 1000, 25);
			if (OnlineUpdate.State == 0) {
				OnlineUpdate.MainText = Texture.CreateFromText(string.Format("認証中...", new object[0]));
				OnlineUpdate.D = new DNet(GameCommon.NewVerURL);
				OnlineUpdate.State = 1;
			}
			if (OnlineUpdate.State == 1) {
				OnlineUpdate.Pct = (long)OnlineUpdate.D.SFEXReadTotal * 1000L / OnlineUpdate.D.FileSize;
				OnlineUpdate.MainText = Texture.CreateFromText(string.Format("ダウンロードしています... {0}/{1}\u3000", OnlineUpdate.D.SFEXReadTotal, OnlineUpdate.D.FileSize, OnlineUpdate.Pct));
				if (OnlineUpdate.D.SaveFileEx(OnlineUpdate.FilePath) == ContentReturn.END) {
					OnlineUpdate.Pct = 1000L;
					OnlineUpdate.State = 2;
				}
			}
			if (OnlineUpdate.State == 2) {
				OnlineUpdate.MainText = Texture.CreateFromText(string.Format("確認中...", new object[0]));
				ReadOptions readOptions = new ReadOptions();
				readOptions.Encoding = Encoding.GetEncoding("shift_jis");
				try {
					OnlineUpdate.Z = ZipFile.Read(OnlineUpdate.FilePath, readOptions);
				} catch {
					Scene.Set("PDAdvertise");
					return ContentReturn.CHANGE;
				}
				OnlineUpdate.Z.Password = "DNGAS:J:A:A";
				OnlineUpdate.Z.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
				foreach (string current in OnlineUpdate.Z.EntryFileNames) {
					OnlineUpdate.FileList[OnlineUpdate.TotalCount] = current;
					OnlineUpdate.TotalCount++;
				}
				OnlineUpdate.State = 3;
				OnlineUpdate.Pct = 0L;
			}
			if (OnlineUpdate.State == 3) {
				try {
					OnlineUpdate.Pct = (long)(OnlineUpdate.Count * 1000 / OnlineUpdate.TotalCount);
				} catch {
				}
				OnlineUpdate.MainText = Texture.CreateFromText(string.Format("更新しています... {0}/{1}\u3000", OnlineUpdate.Count + 1, OnlineUpdate.TotalCount));
				ZipEntry zipEntry = OnlineUpdate.Z[OnlineUpdate.FileList[OnlineUpdate.Count]];
				zipEntry.Extract(".", ExtractExistingFileAction.OverwriteSilently);
				OnlineUpdate.Count++;
				if (OnlineUpdate.Count == OnlineUpdate.TotalCount) {
					OnlineUpdate.State = 4;
					OnlineUpdate.Pct = 1000L;
					OnlineUpdate.TotalCount = 60;
					OnlineUpdate.Count = 0;
				}
			}
			if (OnlineUpdate.State == 4) {
				OnlineUpdate.Pct = 0L;
				OnlineUpdate.MainText = Texture.CreateFromText(string.Format("完了しました。再起動します...\u3000", new object[0]));
				OnlineUpdate.Count++;
				if (OnlineUpdate.Count > OnlineUpdate.TotalCount) {
					StringBuilder ExePath = new StringBuilder(65535);
					GetModuleFileName(IntPtr.Zero, ExePath, ExePath.Capacity);
					new Process {
						StartInfo = {
							FileName = ExePath.ToString(),
							Arguments = ""
						}
					}.Start();
					return ContentReturn.END;
				}
			}
			Texture.SetTextSize(24);
			return ContentReturn.OK;
		}
	}
}
