using Lightness.Core;
using Lightness.DNetwork;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.Resources;
using System;

namespace LEContents {
	public static class GameCommon {

		public static VersionInfo Version = new VersionInfo("DNGAS:J:A:A:20190810001", "だんごなげほうだい");

		public static bool NetworkStatus = false;

		public static bool UpdateAvailable = false;

		public static bool UpdateFailed = false;

		public static string[] DNetMarker = null;

		public static string[] DNetCInfo = null;

		public static string[] DNetNewVer = null;

		public static string NewVerURL = "";

		public static Texture TDNetCInfo = Texture.CreateFromText(" ");

		public static Texture DNErrMsg = Texture.CreateFromText(" ");

		public static Texture DNNewVer = Texture.CreateFromText(" ");

		public static Texture DNUpFail = Texture.CreateFromText(" ");

		public static bool InitDNE = false;

		public static int DCIPos = 0;

		public static ContentReturn CheckNetworkStatus() {
			try {
				GameCommon.UpdateAvailable = false;
				DNet dNet = new DNet("http://DNGAS.network.dark-x.net/dNetwork.txt");
				GameCommon.DNetMarker = dNet.GetStrings();
				if(GameCommon.DNetMarker[0] == "d-Network") {


					/*
					if (!GameCommon.InitDNE) {
						GameCommon.DNErrMsg = Texture.CreateFromFile("DN_ErrMsg.png");
						GameCommon.DNNewVer = Texture.CreateFromFile("DN_NewVer.png");
						GameCommon.InitDNE = true;
					}
					*/
					GameCommon.NetworkStatus = false;



					GameCommon.DNetCInfo = new DNet("http://DNGAS.network.dark-x.net/Information/Circle.txt").GetStrings();
					if(GameCommon.DNetCInfo[0] == "dNetwork.Information.Circle") {
						Texture.SetFont("Meiryo");
						Texture.SetTextSize(20);
						Texture.SetTextColor(255, 255, 255);
						GameCommon.TDNetCInfo = Texture.CreateFromText(GameCommon.DNetCInfo[1]);
					}
					DNet dNet2 = new DNet("http://DNGAS.update.network.dark-x.net/" + GameCommon.Version.GetNet() + ".txt");
					if(dNet2.Status <= 350) {
						GameCommon.DNetNewVer = dNet2.GetStrings();
					} else {
						dNet2 = new DNet(string.Concat(new string[]
						{
							"http://DNGAS.update.network.dark-x.net/",
							GameCommon.Version.APPID,
							"_",
							GameCommon.Version.SKU,
							"_",
							GameCommon.Version.TYPE,
							"_",
							GameCommon.Version.REV,
							".txt"
						}));
						if(dNet2.Status <= 350) {
							GameCommon.DNetNewVer = dNet2.GetStrings();
						} else {
							dNet2 = new DNet("http://DNGAS.update.network.dark-x.net/" + GameCommon.Version.APPID + ".txt");
							if(dNet2.Status <= 350) {
								GameCommon.DNetNewVer = dNet2.GetStrings();
							} else {
								dNet2 = new DNet("http://DNGAS.update.network.dark-x.net/Version.txt");
								if(dNet2.Status > 350) {
									return ContentReturn.END;
								}
								GameCommon.DNetNewVer = dNet2.GetStrings();
							}
						}
					}
					GameCommon.NewVerURL = GameCommon.DNetNewVer[1];

					int CurDate = 0, NewDate = 0, CurNum = 0, NewNum = 1;
					try {
						CurDate = int.Parse(GameCommon.Version.DATE);
						CurNum = int.Parse(GameCommon.Version.CNT);
						VersionInfo NVI = new VersionInfo(GameCommon.DNetNewVer[0]);
						NewDate = int.Parse(NVI.DATE);
						NewNum = int.Parse(NVI.CNT);
					} catch { }

					if((CurDate < NewDate) || ((CurDate == NewDate) && CurNum < NewNum)) {
						Debug.Log('I', "DNetwork", "New Version is detected: {0}", new object[]
						{
							GameCommon.DNetNewVer[0]
						});
						GameCommon.UpdateAvailable = true;
					} else {
						Debug.Log('I', "DNetwork", "Recent Version is running: {0}", new object[]
						{
							GameCommon.DNetNewVer[0]
						});
						GameCommon.UpdateAvailable = false;
					}
				} else {
					GameCommon.UpdateAvailable = false;
					GameCommon.NetworkStatus = false;
				}
			} catch {
				Debug.Log('W', "DNetwork", "Failed", new object[0]);
			}
			if(!GameCommon.InitDNE) {
				GameCommon.DNErrMsg = Texture.CreateFromFile("DN_ErrMsg.png");
				GameCommon.DNNewVer = Texture.CreateFromFile("DN_NewVer.png");
				GameCommon.InitDNE = true;
			}

			return ContentReturn.OK;
		}

		public static ContentReturn DrawNetworkError() {
			if(!GameCommon.UpdateFailed) {
				Core.Draw(GameCommon.DNUpFail, 660, 100, 255);
			}
			if(!GameCommon.NetworkStatus) {
				//Core.Draw(GameCommon.DNErrMsg, 660, 10, 255);
				return ContentReturn.OK;
			}
			if(GameCommon.UpdateAvailable) {
				Core.Draw(GameCommon.DNNewVer, 660, 10, 255);
				return ContentReturn.OK;
			}
			return ContentReturn.OK;
		}

		public static ContentReturn DrawCInfo() {
			try {
				Core.Draw(Effect.Black, 0, 690);
				Core.Draw(GameCommon.TDNetCInfo, Common.WindowX - GameCommon.DCIPos, 690);
				GameCommon.DCIPos += 2;
				if(GameCommon.DCIPos >= Common.WindowX + GameCommon.TDNetCInfo.Width) {
					GameCommon.DCIPos = 0;
				}
			} catch {
			}
			return ContentReturn.OK;
		}
	}
}
