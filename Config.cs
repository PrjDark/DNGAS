using Lightness.Core;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.IO;
using Lightness.Media;
using Lightness.Resources;
using System;
using System.Diagnostics;

namespace LEContents {
	public static class Config {
		public static Texture MenuTitleText = null;

		public static Texture VerText = null;

		public static Texture MainText = null;

		public static Texture BG = null;

		public static bool ShowDetail = false;

		public static bool HelpInit = false;

		public static string HelpImage = "";

		public static VirtualIOEx VIOEx = new VirtualIOEx();

		public static SE CancelSE = new SE();

		public static Menu MainMenu = null;

		public static bool NowFadeOut = false;

		public static int Counter = 0;

		public static ContentReturn Initialize() {
			MediaCommon.CloseAll();
			GameCommon.CheckNetworkStatus();
			Config.BG = Texture.CreateFromFile("ConfigBG.png");
			Config.CancelSE.LoadFile("DNGErr.wav");
			Texture.SetFont("Meiryo");
			Texture.SetTextSize(32);
			Texture.SetTextColor(255, 255, 255);
			Config.MenuTitleText = Texture.CreateFromText("設定");
			Texture.SetTextSize(14);
			Config.VerText = Texture.CreateFromText(GameCommon.Version.Get());
			Config.MainMenu = new Menu("Meiryo", 26, 255, 255, 255, 255);
			Config.MainMenu.SetPointer("DangoMenu.png");
			Config.MainMenu.SetSE("Menu.wav", "DNGOut.wav");
			Config.MainMenu.Add("プレイヤー名の変更");
			if(GameCommon.NetworkStatus) {
				Config.MainMenu.Add("アップデート履歴");
				Config.MainMenu.Add("サークルからのお知らせ");
			}
			Config.MainMenu.Add("サークルサイトへ");
			Config.MainMenu.Add("サークル Twitter: @PrjDark へ");
			Config.MainMenu.Add("もどる");
			Config.ShowDetail = false;
			Config.HelpInit = false;
			Config.HelpImage = "";
			Config.NowFadeOut = false;
			Effect.Reset();
			return ContentReturn.OK;
		}

		public static ContentReturn Main() {
			if(Config.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.CANCEL) != 0) {
				Scene.Set("Title");
				Effect.Reset();
				Config.ShowDetail = false;
				Config.NowFadeOut = true;
				Config.CancelSE.Play();
			}
			Core.Draw(Config.BG, 0, 0);
			Core.Draw(Config.MenuTitleText, 20, 20);
			Core.Draw(Config.VerText, 20, 120);
			ContentReturn contentReturn = Config.MainMenu.Exec(320, 110);
			if(contentReturn == ContentReturn.END) {
				Config.ShowDetail = true;
				if(Config.MainMenu.Selected == 0) {
					Scene.Set("Config_PlayerName");
					return ContentReturn.CHANGE;
				}
				if(GameCommon.NetworkStatus) {
					if(Config.MainMenu.Selected == 1) {
						Scene.Set("Config_UpdateHistory");
						return ContentReturn.CHANGE;
					}
					if(Config.MainMenu.Selected == 2) {
						Scene.Set("Config_CircleInfo");
						return ContentReturn.CHANGE;
					}
				}
				if(Config.MainMenu.Selected == Config.MainMenu.MenuList.Count - 3) {
					Process.Start("http://c.dark-x.net/");
					Menu.Disabled = false;
				}
				if(Config.MainMenu.Selected == Config.MainMenu.MenuList.Count - 2) {
					Process.Start("http://twitter.com/PrjDark");
					Menu.Disabled = false;
				}
				if(Config.MainMenu.Selected == Config.MainMenu.MenuList.Count - 1) {
					Scene.Set("Title");
					Config.NowFadeOut = true;
					Effect.Reset();
				}
			}
			if(!Config.NowFadeOut) {
				Effect.Fadein();
			} else if(Effect.Fadeout() == ContentReturn.END) {
				return ContentReturn.CHANGE;
			}
			GameCommon.DrawCInfo();
			GameCommon.DrawNetworkError();
			return ContentReturn.OK;
		}
	}
}
