using Lightness.Core;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.IO;
using Lightness.Media;
using Lightness.Resources;
using System;
using System.Diagnostics;

namespace LEContents {
	public static class Title {
		public static Texture[] LoadingBG = new Texture[4];

		public static int LoadingState = 0;

		public static bool Loading = true;

		public static Texture VersionText = null;

		public static Texture NetStatText = null;

		public static Texture ICStatIcon = null;

		public static Texture BG = null;

		public static Texture HelpTextS = null;

		public static Texture HelpTextC = null;

		public static Texture[] DNG = null;

		public static int DNGn = 0;

		public static int FramesCount = 0;

		public static BGM TitleBGM = new BGM();

		public static SE CancelSE = new SE();

		public static VirtualIOEx VIOEx = new VirtualIOEx();

		public static bool NowFadeOut = false;

		public static int Mode = 0;

		public static Menu MainMenu = null;


		public static ContentReturn Initialize() {
			MediaCommon.CloseAll();
			Title.NowFadeOut = false;
			Title.Mode = 0;
			Title.DNGn = 0;
			Title.DNG = new Texture[7];
			Title.LoadingState = 0;
			Title.Loading = true;
			for(int i = 0; i < Title.LoadingBG.Length; i++) {
				Title.LoadingBG[i] = Texture.CreateFromFile("Loading" + i + ".png");
			}
			Texture.SetFont("Consolas");
			Texture.SetTextSize(20);
			Texture.SetTextColor(255, 255, 255);
			Title.VersionText = Texture.CreateFromText(GameCommon.Version.Get() + " @C94");
			Texture.SetFont("Meiryo");
			Texture.SetTextSize(20);
			Texture.SetTextColor(255, 255, 255);
			Title.HelpTextS = Texture.CreateFromText("ステージを選択してください");
			Title.HelpTextC = Texture.CreateFromText("使用したい「だんご」を選択してください。");
			Title.MainMenu = new Menu("Meiryo", 26, 255, 255, 255, 255);
			Title.MainMenu.Add("だんごなげほうだい (1 PLAYER OFFLINE)");
			//			Title.MainMenu.Add("さくらのうちかた講座");
			//			Title.MainMenu.Add("オンラインランキング");
			Title.MainMenu.Add("設定");
			Title.MainMenu.Add("手動アップデート確認");
			Title.MainMenu.Add("終了");

			Title.FramesCount = 0;
			Effect.Reset();
			GameCommon.CheckNetworkStatus();
			if(GameCommon.NetworkStatus) {
				Title.NetStatText = Texture.CreateFromFile("DN_OK.png");
			} else {
				Title.NetStatText = Texture.CreateFromFile("DN_ERR.png");
			}
			Title.ICStatIcon = Texture.CreateFromFile("DNIC_ERR.png");
			return ContentReturn.OK;
		}

		public static ContentReturn Main() {
			if(Title.Loading) {
				GameCommon.DrawNetworkError();
				Core.Draw(Title.LoadingBG[Title.LoadingState % 4], 920, 560);
				switch(Title.LoadingState) {

					case 7:
						Title.CancelSE.LoadFile("DNGErr.wav");
						break;
					case 8:
						Title.TitleBGM.LoadFile("BGM_Title.wav");
						break;
					case 9:
						Title.BG = Texture.CreateFromFile("TitleBG.png");
						break;
					case 10:
						Title.MainMenu.SetPointer("DangoMenu.png");
						break;
					case 11:
						Title.MainMenu.SetSE("Menu.wav", "DNGOut.wav");
						break;

					case 18:
						Title.TitleBGM.Play();
						break;
					case 19:
						Title.Loading = false;
						break;
				}
				Title.LoadingState++;
				return ContentReturn.OK;
			}
			Core.Draw(Title.BG, 0, 0);
			Core.Draw(Title.VersionText, 10, 10);
			Core.Draw(Title.NetStatText, 10, 35);
			Core.Draw(Title.ICStatIcon, 79, 35);
			if(Title.Mode == 0) {
				ContentReturn contentReturn = Title.MainMenu.Exec(560, 370);
				if(contentReturn == ContentReturn.CHANGE) {
					Title.FramesCount = 0;
				}
				if(contentReturn == ContentReturn.END) {
					Effect.Reset();
					Title.NowFadeOut = true;
					switch(Title.MainMenu.Selected) {
						case 0:
							DNGAS.Initialized = false;
							Scene.Set("DNGAS");
							return ContentReturn.OK;
						/*
												case 1:
													Scene.Set("HowToPlay");
													break;
												case 2:
													Scene.Set("Ranking");
													break;
						 */
						case 1:
							Scene.Set("Config");
							break;
						case 2:
							Process.Start("http://project.xprj.net/game/DNGAS");
							Menu.Disabled = false;
							Environment.Exit(0);
							break;
						case 3:
							Scene.Set("GameEnd");
							break;
					}
				}
			}

			if(!Title.NowFadeOut) {
				Effect.Fadein();
				Title.FramesCount++;
				if(Title.FramesCount > 2770 && Title.Mode == 0) {
					MediaCommon.CloseAll();
					Scene.Set("PDAdvertise");
					return ContentReturn.CHANGE;
				}
			} else if(Effect.Fadeout() == ContentReturn.END) {
				MediaCommon.CloseAll();
				return ContentReturn.CHANGE;
			}
			GameCommon.DrawNetworkError();
			GameCommon.DrawCInfo();
			return ContentReturn.OK;
		}
	}
}
