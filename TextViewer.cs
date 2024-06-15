using Lightness.Core;
using Lightness.Graphic;
using Lightness.IO;
using Lightness.Resources;
using System;

namespace LEContents {
	public static class TextViewer {
		public static Texture Message;

		public static int PointerY;

		public static ContentReturn Initialize(string[] InfoMsgFile) {
			string text = "";
			TextViewer.PointerY = 0;
			for (int i = 2; i < InfoMsgFile.Length; i++) {
				text = text + InfoMsgFile[i] + "\n";
			}
			Texture.SetTextSize(16);
			TextViewer.Message = Texture.CreateFromText(text);
			return ContentReturn.OK;
		}

		public static ContentReturn Exec() {
			Common.GSprite.DrawEx(TextViewer.Message, 100, 100, 255, 0, TextViewer.PointerY, 1280, 560);
			if (VirtualIO.GetButton(0, VirtualIO.ButtonID.DOWN) != 0) {
				TextViewer.PointerY += 4;
			}
			if (VirtualIO.GetButton(0, VirtualIO.ButtonID.UP) != 0) {
				TextViewer.PointerY -= 4;
			}
			if (VirtualIO.GetAnalog(0, VirtualIO.AnalogID.LY) < 0) {
				TextViewer.PointerY += (VirtualIO.GetAnalog(0, VirtualIO.AnalogID.LY) + 128) / 8;
			}
			if (VirtualIO.GetAnalog(0, VirtualIO.AnalogID.LY) > 0) {
				TextViewer.PointerY += (VirtualIO.GetAnalog(0, VirtualIO.AnalogID.LY) - 128) / 8;
			}
			if (VirtualIO.GetPoint(VirtualIO.PointID.L) != 0) {
				TextViewer.PointerY -= (VirtualIO.RTPointY - VirtualIO.PointY) / 16;
			}
			if (TextViewer.PointerY < -350) {
				TextViewer.PointerY = -350;
			}
			if (TextViewer.PointerY > TextViewer.Message.Height - 150) {
				TextViewer.PointerY = TextViewer.Message.Height - 150;
			}
			return ContentReturn.OK;
		}
	}
}
