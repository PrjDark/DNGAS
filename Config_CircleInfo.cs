using Lightness.Core;
using Lightness.DNetwork;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.IO;
using Lightness.Media;
using Lightness.Resources;
using System;
using System.Threading;

namespace LEContents {
	public static class Config_CircleInfo {
		public static Texture MenuTitleText = null;

		public static Texture HelpText = null;

		public static Texture BG = null;

		public static SE CancelSE = new SE();

		public static SE DecideSE = new SE();

		public static VirtualIOEx VIOEx = new VirtualIOEx();

		public static int Counter = 0;

		public static ContentReturn Initialize() {
			Config_CircleInfo.CancelSE.LoadFile("DNGErr.wav");
			Config_CircleInfo.DecideSE.LoadFile("DNGOut.wav");
			Config_CircleInfo.BG = Texture.CreateFromFile("ConfigBG.png");
			Texture.SetFont("Meiryo");
			Texture.SetTextSize(32);
			Texture.SetTextColor(255, 255, 255);
			Config_CircleInfo.MenuTitleText = Texture.CreateFromText("サークルからのお知らせ");
			Texture.SetTextSize(16);
			Config_CircleInfo.HelpText = Texture.CreateFromText("決定・キャンセルボタンで前の画面に戻ります。 / ドラッグするとスクロールします。");
			TextViewer.Initialize(new DNet("http://DNGAS.network.dark-x.net/Information/Circle.txt").GetStrings());
			GameCommon.CheckNetworkStatus();
			return ContentReturn.OK;
		}

		public static ContentReturn Main() {
			Core.Draw(Config_CircleInfo.BG, 0, 0);
			Core.Draw(Config_CircleInfo.MenuTitleText, 20, 20);
			TextViewer.Exec();
			if(Config_CircleInfo.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.OK) != 0 || Config_CircleInfo.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.CANCEL) != 0 || Config_CircleInfo.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.START) != 0 || Config_CircleInfo.VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.MENU) != 0 || Config_CircleInfo.VIOEx.GetPointOnce(VirtualIO.PointID.R) != 0) {
				Config_CircleInfo.DecideSE.Play();
				Scene.Set("Config");
				Thread.Sleep(250);
				return ContentReturn.CHANGE;
			}
			Config_CircleInfo.Counter++;
			if(Config_CircleInfo.Counter > 30) {
				Core.Draw(Config_CircleInfo.HelpText, 160, 510);
			}
			if(Config_CircleInfo.Counter > 60) {
				Config_CircleInfo.Counter = 0;
			}
			return ContentReturn.OK;
		}
	}
}
