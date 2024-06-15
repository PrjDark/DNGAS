using Lightness.Core;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.IO;
using Lightness.Media;
using Lightness.Resources;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace LEContents {
	public static class Config_PlayerName {
		public static bool ReturnToResult = false;

		public static Texture MenuTitleText = null;

		public static Texture VerText = null;

		public static Texture BG = null;

		public static Texture TPlayerName = null;

		public static Texture MainText = null;

		public static Texture HelpText = null;

		public static string PlayerName = "";

		public static string PlayerNameOld = "*";

		public static int BlinkCount = 0;

		public static SE DecideSE = new SE();

		public static SE CancelSE = new SE();

		public static SE SelectSE = new SE();

		public static VirtualIOEx VIOEx = new VirtualIOEx();

		public static ContentStream UserDataFile = null;

		public static StreamReader UserData = null;

		public static ContentReturn Initialize() {
			Config_PlayerName.BlinkCount = 0;
			Config_PlayerName.PlayerNameOld = "*";
			Effect.Reset();
			Config_PlayerName.UserDataFile = new ContentStream("UserData.lec", true);
			Config_PlayerName.UserData = new StreamReader(Config_PlayerName.UserDataFile, Encoding.UTF8, true);
			Config_PlayerName.PlayerName = Config_PlayerName.UserData.ReadLine();
			if (Config_PlayerName.PlayerName == null) {
				Config_PlayerName.PlayerName = "";
			}
			Config_PlayerName.DecideSE.LoadFile("DNGOut.wav");
			Config_PlayerName.CancelSE.LoadFile("DNGErr.wav");
			Config_PlayerName.SelectSE.LoadFile("Select.wav");
			Texture.SetFont("Meiryo");
			Texture.SetTextSize(32);
			Texture.SetTextColor(255, 255, 255);
			Config_PlayerName.MenuTitleText = Texture.CreateFromText("プレイヤー名の登録");
			Texture.SetTextSize(24);
			Config_PlayerName.MainText = Texture.CreateFromText("ランキングで使用する公開プレイヤー名を入力してください。");
			Texture.SetTextSize(20);
			Config_PlayerName.HelpText = Texture.CreateFromText("START / [Enter] : 決定 - SELECT / [ESC]: 戻る\n[Delete]: 全文字削除");
			Texture.SetTextSize(14);
			Config_PlayerName.VerText = Texture.CreateFromText(GameCommon.Version.Get());
			Config_PlayerName.BG = Texture.CreateFromFile("ConfigBG.png");
			Config_PlayerName.TPlayerName = Texture.CreateFromText(Config_PlayerName.PlayerName + " ");
			Control.LastKey = "";
			return ContentReturn.OK;
		}

		public static ContentReturn Main() {
			Core.Draw(Config_PlayerName.BG, 0, 0);
			Core.Draw(Config_PlayerName.MenuTitleText, 20, 20);
			Core.Draw(Config_PlayerName.VerText, 20, 80);
			Core.Draw(Config_PlayerName.MainText, 60, 140);
			if (Config_PlayerName.BlinkCount < 30) {
				Core.Draw(Config_PlayerName.HelpText, 260, 440);
			}
			if (Config_PlayerName.BlinkCount > 60) {
				Config_PlayerName.BlinkCount = 0;
			}
			if (Config_PlayerName.PlayerName != Config_PlayerName.PlayerNameOld) {
				Texture.SetFont("Consolas");
				Texture.SetTextColor(255, 255, 255);
				Config_PlayerName.PlayerNameOld = Config_PlayerName.PlayerName;
				if (Config_PlayerName.PlayerName != "") {
					Texture.SetTextSize(96);
					Config_PlayerName.TPlayerName = Texture.CreateFromText(Config_PlayerName.PlayerName);
				} else {
					Texture.SetTextSize(42);
					Config_PlayerName.TPlayerName = Texture.CreateFromText("(キーボードで入力してください)\u3000\u3000");
				}
			}
			Core.Draw(Config_PlayerName.TPlayerName, 520 - Config_PlayerName.TPlayerName.Width / 2, 230);
			if (Control.LastKey.Length >= 3) {
				if (Control.LastKey == "Back" && Config_PlayerName.PlayerName.Length != 0) {
					Config_PlayerName.PlayerName = Config_PlayerName.PlayerName.Substring(0, Config_PlayerName.PlayerName.Length - 1);
					Config_PlayerName.SelectSE.Play();
				}
				if (Control.LastKey == "Delete") {
					Config_PlayerName.PlayerName = "";
					Config_PlayerName.SelectSE.Play();
				}
				if (Control.LastKey == "OemBackslash") {
					Control.LastKey = "_";
				}
				if (Control.LastKey == "OemMinus") {
					Control.LastKey = "-";
				}
				if (Control.LastKey == "OemPeriod") {
					Control.LastKey = ".";
				}
				if (Control.LastKey == "Decimal") {
					Control.LastKey = ".";
				}
				if (Control.LastKey == "Subtract") {
					Control.LastKey = "-";
				}
			}
			if (Control.LastKey.Length == 2 && Control.LastKey.Substring(0, 1) == "D") {
				Control.LastKey = Control.LastKey.Substring(1);
			}
			if (Control.LastKey.Length == 7 && Control.LastKey.Substring(0, 6) == "NumPad") {
				Control.LastKey = Control.LastKey.Substring(6);
			}
			if (Control.LastKey.Length == 1) {
				Config_PlayerName.SelectSE.Play();
				Config_PlayerName.PlayerName += Control.LastKey;
			}
			if (Config_PlayerName.PlayerName.Length > 8) {
				Config_PlayerName.PlayerName = Config_PlayerName.PlayerName.Substring(0, 8);
				Config_PlayerName.CancelSE.Play();
			}
			if (Config_PlayerName.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.START) == 0) {
				if (Config_PlayerName.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.MENU) != 0) {
					Config_PlayerName.CancelSE.Play();
					if (!Config_PlayerName.ReturnToResult) {
						Scene.Set("Config");
						Config_PlayerName.UserData.Close();
						Thread.Sleep(250);
						return ContentReturn.CHANGE;
					}
				}
				Control.LastKey = "";
				Config_PlayerName.BlinkCount++;
				return ContentReturn.OK;
			}
			if (Config_PlayerName.PlayerName.Length == 0) {
				Config_PlayerName.CancelSE.Play();
				Control.LastKey = "";
				return ContentReturn.OK;
			}
			Config_PlayerName.DecideSE.Play();
			Scene.Set("Config");
			if (Config_PlayerName.ReturnToResult) {
				Scene.Set("Result");
				Config_PlayerName.ReturnToResult = false;
			}
			try {
				Config_PlayerName.UserDataFile.Close();
				Config_PlayerName.UserDataFile = new ContentStream();
				byte[] bytes = Encoding.UTF8.GetBytes(Config_PlayerName.PlayerName);
				Config_PlayerName.UserDataFile.Write(bytes, 0, bytes.Length);
				Config_PlayerName.UserDataFile.SaveFile("UserData.lec");
			} catch {
			}
			Thread.Sleep(250);
			return ContentReturn.CHANGE;
		}
	}
}
