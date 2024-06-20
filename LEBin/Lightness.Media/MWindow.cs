using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lightness.Media {
	public class MWindow : Form {
		public bool Initialized;

		protected override void OnPaintBackground(PaintEventArgs pevent) {
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			//			MediaCommon.WMProc(ref m);
		}

		public MWindow() {
			base.ShowInTaskbar = false;
			base.FormBorderStyle = FormBorderStyle.None;
			base.ClientSize = new Size(0, 0);
			base.Size = new Size(0, 0);
			base.WindowState = FormWindowState.Minimized;
		}
	}
}
