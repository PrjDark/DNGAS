using System;
using Lightness.Core;
using Lightness.Framework;
using Lightness.IO;
using Lightness.Media;
using Lightness.Resources;

namespace LEContents {
	public class PDAdvertise {
		private static bool SetAdvertise;

		public static int AdvertiseID = 99999;

		public static ContentReturn Initialize() {
			MediaCommon.CloseAll();
			PDAdvertise.SetAdvertise = false;
			PDAdvertise.AdvertiseID = 0;
			GameCommon.CheckNetworkStatus();
			return ContentReturn.OK;
		}

		public static ContentReturn Main() {

			if(VirtualIO.GetButton(0, VirtualIO.ButtonID.START) != 0){ //Full skip Advertise when START pressed
						Scene.Set("Title");
						return ContentReturn.CHANGE;
			}

			if (!PDAdvertise.SetAdvertise) {
				switch (PDAdvertise.AdvertiseID) {
					case 0:
						Advertise.Set("WarnMsg_DNGAS.png", 15, 30);
						break;
					case 1:
						Advertise.Set("AdvertiseJP.png", 15, 20);
						break;
/*
						case 2:
						Advertise.Set("TeamDangoLogo.png", 15, 20);
						break;
					case 3:
						Advertise.Set("DNLogo.png", 15, 20);
						break;
*/
					default:
						Scene.Set("Title");
						return ContentReturn.CHANGE;
				}
				PDAdvertise.AdvertiseID++;
				PDAdvertise.SetAdvertise = true;
			}
			if (Advertise.Exec() == ContentReturn.END) {
				PDAdvertise.SetAdvertise = false;
			}
			GameCommon.DrawNetworkError();
			return ContentReturn.OK;
		}
	}
}
