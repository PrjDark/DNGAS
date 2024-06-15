using Lightness.Core;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Lightness.Media
{
	public static class MediaCommon
	{
		public delegate int dIOProc(IntPtr lpMMIOInfo, IntPtr uMessage, IntPtr lParam1, IntPtr lParam2);

		public static int SameFrameSECount = 0;

		public static int MAX_SOUNDS = 128;

		public static ContentStream[] MediaStream = new ContentStream[MediaCommon.MAX_SOUNDS];

		public static string[] IOFileNames = new string[MediaCommon.MAX_SOUNDS];

		public static string[] MCIDeviceIdToAlias = new string[MediaCommon.MAX_SOUNDS];

		public static bool[] MCIDeviceIdLoopFlag = new bool[MediaCommon.MAX_SOUNDS];

		public static MediaCommon.dIOProc IOProcess;

		public static int LastAssignId = -1;

		public static string LastAssignFile = "*";

		public static string[] BGMAlias = new string[MediaCommon.MAX_SOUNDS];

		public static bool Installed = false;

		public static LEWindow MainWindow = null;

		public static MWindow MediaWindow = null;

		public static string GetRandom()
		{
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			byte[] array = new byte[16];
			rNGCryptoServiceProvider.GetBytes(array);
			return BitConverter.ToString(array).ToLower().Replace("-", "");
		}

		public static uint mmioFOURCC(char c0, char c1, char c2, char c3)
		{
			return (uint)((uint)c3 << 24 | (uint)c2 << 16 | (uint)c1 << 8 | c0);
		}

		[DllImport("winmm.dll")]
		public static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

		[DllImport("winmm.dll")]
		public static extern int mmioInstallIOProc(uint fccIOProc, MediaCommon.dIOProc pIOProc, int dwFlags);

		[DllImport("winmm.dll")]
		public static extern int mciGetDeviceID(string Alias);

		public static int AssignMediaStreamId(string FileName)
		{
			for (int i = 0; i < MediaCommon.MediaStream.Length; i++)
			{
				if (MediaCommon.MediaStream[i] == null)
				{
					Debug.Log('I', "Media", "Assign: {0}", new object[]
					{
						i
					});
					return i;
				}
			}
			return 0;
		}

		public static void WMProc(ref Message m)
		{
			if (m.Msg == 953)
			{
				string text = MediaCommon.MCIDeviceIdToAlias[(int)m.LParam];
				if (MediaCommon.MCIDeviceIdLoopFlag[(int)m.LParam])
				{
					MediaCommon.mciSendString("seek " + text + " to start", null, 0, IntPtr.Zero);
					int num = MediaCommon.mciSendString("play " + text + " notify", null, 0, MediaCommon.MediaWindow.Handle);
					Debug.Log('I', "Media", "Loop: {0}, {1}, {2}", new object[]
					{
						text,
						num,
						m.LParam
					});
				}
			}
		}

		public static int IOProc(IntPtr lpMMIOInfo, IntPtr uMessage, IntPtr lParam1, IntPtr lParam2)
		{
			try
			{
				MMIOINFO mMIOINFO = default(MMIOINFO);
				mMIOINFO = (MMIOINFO)Marshal.PtrToStructure(lpMMIOInfo, typeof(MMIOINFO));
				int num = mMIOINFO.adwInfo;
				switch ((int)uMessage)
				{
				case 0:
				{
					byte[] array = new byte[(int)lParam2];
					MediaCommon.MediaStream[num].Seek((long)mMIOINFO.lDiskOffset, SeekOrigin.Begin);
					MediaCommon.MediaStream[num].Read(array, 0, array.Length);
					Marshal.Copy(array, 0, lParam1, (int)lParam2);
					mMIOINFO.lDiskOffset += (int)lParam2;
					Marshal.StructureToPtr(mMIOINFO, lpMMIOInfo, false);
					int result = (int)lParam2;
					return result;
				}
				case 2:
				{
					switch ((int)lParam2)
					{
					case 0:
						mMIOINFO.lDiskOffset = (int)lParam1;
						break;
					case 1:
						mMIOINFO.lDiskOffset += (int)lParam1;
						break;
					case 2:
						mMIOINFO.lDiskOffset = (int)(MediaCommon.MediaStream[num].Length - (long)((int)lParam1));
						break;
					}
					Marshal.StructureToPtr(mMIOINFO, lpMMIOInfo, false);
					int result = mMIOINFO.lDiskOffset;
					return result;
				}
				case 3:
				{
					string text = Marshal.PtrToStringAnsi(lParam1);
					text = text.Substring(0, text.LastIndexOf('+'));
					int result;
					if (MediaCommon.LastAssignFile == text)
					{
						mMIOINFO.adwInfo = MediaCommon.LastAssignId;
						Marshal.StructureToPtr(mMIOINFO, lpMMIOInfo, false);
						result = 0;
						return result;
					}
					num = MediaCommon.AssignMediaStreamId(text);
					if (MediaCommon.MediaStream[num] == null)
					{
						Debug.Log('I', "Media", "IO: Open: \"{0}\"", new object[]
						{
							text
						});
						try
						{
							MediaCommon.MediaStream[num] = new ContentStream(text);
							goto IL_213;
						}
						catch
						{
							Debug.Log('E', "Media", "IO: Not Exist: \"{0}\"", new object[]
							{
								text
							});
							result = -1;
							return result;
						}
					}
					Debug.Log('I', "Media", "IO: Already Opened: \"{0}\"", new object[]
					{
						text
					});
					IL_213:
					MediaCommon.IOFileNames[num] = text;
					mMIOINFO.adwInfo = num;
					Marshal.StructureToPtr(mMIOINFO, lpMMIOInfo, false);
					MediaCommon.LastAssignId = num;
					MediaCommon.LastAssignFile = text;
					result = 0;
					return result;
				}
				case 4:
				{
					Debug.Log('I', "Media", "IO: Close: {0}", new object[]
					{
						lParam1
					});
					MediaCommon.MediaStream[num].Close();
					MediaCommon.MediaStream[num].Dispose();
					MediaCommon.MediaStream[num] = null;
					MediaCommon.IOFileNames[num] = "";
					GC.Collect(2);
					int result = 0;
					return result;
				}
				}
			}
			catch
			{
			}
			Debug.Log('W', "Media", "IO: Unknown Command {0}, {1}, {2}, {3}", new object[]
			{
				lpMMIOInfo,
				uMessage,
				lParam1,
				lParam2
			});
			return -1;
		}

		public static void CloseAll()
		{
			for (int i = 0; i < MediaCommon.MCIDeviceIdToAlias.Length; i++)
			{
				MediaCommon.MCIDeviceIdLoopFlag[i] = false;
				if (MediaCommon.MCIDeviceIdToAlias[i] != null)
				{
					MediaCommon.mciSendString("close " + MediaCommon.MCIDeviceIdToAlias[i], null, 0, IntPtr.Zero);
				}
			}
		}

		public static int InstallIOProc()
		{
			if (MediaCommon.Installed)
			{
				return 0;
			}
			MediaCommon.IOProcess = new MediaCommon.dIOProc(MediaCommon.IOProc);
			GCHandle.Alloc(MediaCommon.IOProcess);
			int dwFlags = 268500992;
			int result = MediaCommon.mmioInstallIOProc(MediaCommon.mmioFOURCC('W', 'A', 'V', ' '), MediaCommon.IOProcess, dwFlags);
			MediaCommon.Installed = true;
			return result;
		}

		public static void Initialize(LEWindow MW)
		{
			MediaCommon.MainWindow = MW;
			MediaCommon.MediaWindow = new MWindow();
			MediaCommon.MediaWindow.Show();
			int num = MediaCommon.InstallIOProc();
			Debug.Log('I', "Sound", "Initialize: {0}", new object[]
			{
				num
			});
		}
	}
}
