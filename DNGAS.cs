using Lightness.Core;
using Lightness.Framework;
using Lightness.Graphic;
using Lightness.Resources;
using System;
using System.Threading;
using Lightness.IO;
using Lightness.Media;

namespace LEContents {

	public struct CharData {
		public DNGAS.DangoID DangoType;
		public bool Moving;
		public bool Falling;
		public int Ywhen1Frame;
		public int X;
		public int Y;
		public int LimitX;
		public int MatrixX;
		public int MatrixY;
	}



	public static class DNGAS {
		public static int LoadingState = 0;

		public static Texture[] LoadingBG = new Texture[4];

		public static ContentReturn Initialize() {
			LoadingState = 0;
			Core.SetTitle(GameCommon.Version.Title);
			for (int i = 0; i < LoadingBG.Length; i++) {
				LoadingBG[i] = Texture.CreateFromFile("Loading" + i + ".png");
			}
			return ContentReturn.OK;
		}

		public enum DangoID {
			None,
			Pink,
			Yellow,
			Green,
			Blue,
			Black,
			Burn,
		}

		static Random RNG;

		static BGM BGM;
		static SE OutSE;
		static SE BulletS;
		static SE DNGGO;
		static SE DNGBurn;
		static SE DNGOK;

		static Texture GameBG;
		static Texture GameBG_Front;
		static Texture T;

		static Texture TScore;
		static int Score;

		static Texture[] DangoL = new Texture[Enum.GetNames(typeof(DangoID)).Length];
		static Texture[] DangoR = new Texture[Enum.GetNames(typeof(DangoID)).Length];

		public static bool Initialized = false;
		static int OX = 100;
		static int OY = 360;

		static DangoID CurrentDango;
		static DangoID NextDango;
		static DangoID NextDango2;
		static CharData MovingDango;

		static VirtualIOEx VIOEx = new VirtualIOEx();


		static bool CanThrow = true;
		static bool GameOver = false;
		static bool GameOverEx = false;

		static int FCounter = 0;
		static int WorkTime = 0;
		static int AddSpeed = 0;
		static Texture TTimer;

		static int AddScoreInSameFrame = 0;

		static CharData[][] DangoMatrix;


		public static ContentReturn AddScore(int ScoreToAdd) {
			AddScoreInSameFrame++;

			Score += (ScoreToAdd * AddScoreInSameFrame);

			Texture.SetTextSize(48);
			Texture.SetTextColor(255, 255, 255);
			Texture.SetFont("Meiryo");
			TScore = Texture.CreateFromText(string.Format("{0}", Score));


			if (Score > 500) { AddSpeed = 7; }
			if (Score > 1000) { AddSpeed = 6; }
			if (Score > 1500) { AddSpeed = 5; }
			if (Score > 2000) { AddSpeed = 4; }
			if (Score > 2500) { AddSpeed = 3; }
			if (Score > 3000) { AddSpeed = 2; }
			if (Score > 4000) { AddSpeed = 1; }


			if (Score > 1000) { AddSpeed = 9; }
			if (Score > 2000) { AddSpeed = 8; }
			if (Score > 3000) { AddSpeed = 7; }
			if (Score > 4000) { AddSpeed = 6; }
			if (Score > 5000) { AddSpeed = 5; }
			if (Score > 6000) { AddSpeed = 4; }
			if (Score > 8000) { AddSpeed = 3; }
			if (Score > 16000) { AddSpeed = 2; }
			if (Score > 32000) { AddSpeed = 1; }
			return ContentReturn.OK;
		}


		static DangoID GetNewDango() {
			RNG.Next();
			int ID = 0;
			while (true) {
				ID = RNG.Next(0, Enum.GetNames(typeof(DangoID)).Length);
//				if (ID == (int)DangoID.None) {
//					continue;
//				}
				break;
			}
			Console.WriteLine("NEW DANGO: {0}", Enum.ToObject(typeof(DangoID), ID));
			return (DangoID)ID;
		}

		static DangoID GetNextDango() {
			CurrentDango = NextDango;
			NextDango = NextDango2;

			while (true) {
				NextDango2 = GetNewDango();
				if (NextDango2 == DangoID.None) {
					continue;
				}
				if (NextDango == NextDango2 && NextDango == CurrentDango) {
					continue;
				}
				break;
			}


			return CurrentDango;
		}

		static bool AddDangoLine(int Line) {
			for(int i=0; i<Line; i++){

				//Check 1st Line
				bool AllBlank = true;
				for (int m = 0; m < DangoMatrix[0].Length; m++) {
					if (DangoMatrix[0][m].DangoType != DangoID.None) {
						AllBlank = false;
					}
				}
				if (!AllBlank) { GameOver = true; }



					for (int n = 0; n < DangoMatrix.Length - 1; n++) {

						for (int m = 0; m < DangoMatrix[n].Length; m++) {
							DangoMatrix[n][m] = DangoMatrix[n + 1][m];
						}
					}
					for (int m = 0; m < DangoMatrix[DangoMatrix.Length - 1].Length; m++) {
						DangoMatrix[DangoMatrix.Length - 1][m].DangoType = GetNewDango();
					}
				}
			DNGOK.Play();
			return false;
		}

		static int GetSuitableMatrixX(int MatrixY) {
			for (int n = 0; n < DangoMatrix.Length; n++ ) {
				if (DangoMatrix[n][MatrixY].DangoType != DangoID.None) {
					return n-1;
				}
			}
			return -1;
		}


		static void DeleteBlankLineDango() {
			for (int n = DangoMatrix.Length-1; 0 < n; n--) {
				bool AllBlank = true;
				for (int m = 0; m < DangoMatrix[n].Length; m++) {
					if(DangoMatrix[n][m].DangoType != DangoID.None){AllBlank = false; break;}
				}
				if (AllBlank) {
//					Console.WriteLine("ALL BLANK DETECTED: " + n);
					for (int i = 0; i < n; i++) {
						for (int j = 0; j < DangoMatrix[i].Length; j++) {
							DangoMatrix[i][j].DangoType = DangoID.None;
						}
					}
					return;
				}
			}
		}
		static void DeleteMatchedDango(int X, int Y) {

			bool Deleted = false;
			DangoID DangoIDToDelete = DangoMatrix[X][Y].DangoType;

			try {
				if (DangoIDToDelete == DangoMatrix[X][Y - 1].DangoType) {
					DangoMatrix[X][Y].DangoType = DangoID.None;
					DangoMatrix[X][Y - 1].DangoType = DangoID.None;
					AddScore(200);
					Deleted = true;
				}
			} catch { }

			try {
				if (DangoIDToDelete == DangoMatrix[X][Y + 1].DangoType) {
					DangoMatrix[X][Y].DangoType = DangoID.None;
					DangoMatrix[X][Y + 1].DangoType = DangoID.None;
					AddScore(200);
					Deleted = true;
				}
			} catch { }

			try {
				if (DangoIDToDelete == DangoMatrix[X + 1][Y].DangoType) {
					DangoMatrix[X][Y].DangoType = DangoID.None;
					DangoMatrix[X+1][Y].DangoType = DangoID.None;
					AddScore(100);
					Deleted = true;
				}
			} catch { }

			if (Deleted) {
				DNGBurn.Play();
			}
		}

		public static ContentReturn Main() {
			AddScoreInSameFrame = 0;

			if (!Initialized) {
				RNG = new Random();

				Core.Draw(LoadingBG[LoadingState % 4], 920, 560);
				LoadingState++;
				if (LoadingState < 1) { return ContentReturn.OK; }


				DangoMatrix = new CharData[15][];
				for (int n = 0; n < DangoMatrix.Length; n++) {
					DangoMatrix[n] = new CharData[8];
					for (int m = 0; m < DangoMatrix[n].Length; m++) {
						DangoMatrix[n][m] = new CharData();
					}
				}

				CanThrow = true;
				GameOver = false;
				GameOverEx = false;

				Score = 0;
				FCounter = 60;
				WorkTime = 0;
				AddSpeed = 8;
				AddSpeed = 10;

				MovingDango = new CharData();

				BGM = new BGM();
				BGM.LoadFile("MMS.wav");
				BGM.Play();

				OutSE = new SE();
				OutSE.LoadFile("DNGOut.wav");

				BulletS = new SE();
				BulletS.LoadFile("BulletS.wav");

				DNGGO = new SE();
				DNGGO.LoadFile("DNGGO.wav");

				DNGBurn = new SE();
				DNGBurn.LoadFile("DNGBurn.wav");

				DNGOK = new SE();
				DNGOK.LoadFile("DNGOK.wav");

				GameBG = Texture.CreateFromFile("GameBG.png");
				GameBG_Front = Texture.CreateFromFile("GameBG_Block.png");
				T = Texture.CreateFromFile("TargetLine.png");
				Initialized = true;

				DangoL[(int)DangoID.None] = Texture.CreateFromFile("DangoNone.png");
				DangoL[(int)DangoID.Pink] = Texture.CreateFromFile("DangoPink.png");
				DangoL[(int)DangoID.Yellow] = Texture.CreateFromFile("DangoYellow.png");
				DangoL[(int)DangoID.Green] = Texture.CreateFromFile("DangoGreen.png");
				DangoL[(int)DangoID.Blue] = Texture.CreateFromFile("DangoWater.png");
				DangoL[(int)DangoID.Black] = Texture.CreateFromFile("DangoBlack.png");
				DangoL[(int)DangoID.Burn] = Texture.CreateFromFile("DangoBurn.png");

				for (int i = 0; i < 3; i++) {
					GetNextDango();
				}
				AddDangoLine(8);

				AddScore(0);

				return ContentReturn.OK;
			}

			int RTX = VirtualIO.RTPointX; 
			int RTY = VirtualIO.RTPointY;
			if (RTY < 180) { RTY = 180; }
			if (RTY > 540) { RTY = 540; }

			/*
//			if (RTX < (OX + 240)) { RTX = OX + 240; }
			double RD = Math.Atan2(RTY - OY, RTX - OX);
			if (RD < -0.785398f) { RD = -0.783598f; }
			if (RD > 0.785398f) { RD = 0.783598f; }
			//Console.WriteLine(RD);
			*/


			//BG
			Core.Draw(GameBG, 0, 0, 255);

			//MAIN
			if (CanThrow) {
				//Common.GSprite.DrawRadian(T, OX, OY, 255, (float)RD, 0, 15);
				Core.Draw(T, OX, RTY-16, 255);
			}
			//Core.Draw(DangoL[(int)CurrentDango], OX - 32, OY - 32, 255);
			Core.Draw(DangoL[(int)CurrentDango], OX - 32, RTY - 32, 255);



			//DANGOs
			for(int n=0; n<DangoMatrix.Length; n++){
				int Wave = 0;
//				if ((n % 2) == 0) {
//					Wave = 24;
//				}
				Wave = 12;
					for (int m = 0; m < DangoMatrix[n].Length; m++) {
					//Common.GSprite.DrawEx(DangoL[(int)DangoMatrix[n][m].DangoType], n, m, 255, 64, 64, 64, 64);
					Core.Draw(DangoL[(int)DangoMatrix[n][m].DangoType], (n * 58) + 400, (m * 48) + 150 + Wave);
				}
			}

			if (CanThrow && VIOEx.GetButtonOnce(0, VirtualIO.ButtonID.MENU) != 0) {
				AddDangoLine(1);
			}


			if(CanThrow && !GameOver && VIOEx.GetPointOnce(VirtualIO.PointID.L) != 0){
				CanThrow = false;

				MovingDango = new CharData();
				MovingDango.DangoType = CurrentDango;
				MovingDango.Moving = true;
				MovingDango.X = OX - 32;
				MovingDango.Y = RTY - 32;
				//MovingDango.Y = OY - 32;

				/*
				double TMPX = RTX - OX;
				double TMPY = RTY - OY;
				double TEST = (TMPY / TMPX) * 16;
				Console.WriteLine("Y: {0} - {1} / {2} - {3} = {4}", RTY, OY, RTX, OX, TEST);
				MovingDango.Ywhen1Frame = (int) TEST;
				*/

				MovingDango.MatrixY = (RTY -24  - 150) / 48;
				if (MovingDango.MatrixY > DangoMatrix[0].Length-1) { MovingDango.MatrixY = DangoMatrix[0].Length-1; }

				MovingDango.MatrixX = GetSuitableMatrixX(MovingDango.MatrixY);

				MovingDango.LimitX = (MovingDango.MatrixX * 58) + 400;
				if (MovingDango.MatrixX < 0) {
					MovingDango.MatrixX = 0;
					GameOver = true;
				}

				BulletS.Play();
				GetNextDango();
			}


			if (MovingDango.Moving) {
				MovingDango.X += 24;
				/*
				MovingDango.Y += MovingDango.Ywhen1Frame;
				if (MovingDango.Y < 160) {
					MovingDango.Ywhen1Frame *= -1;
				}
				if (MovingDango.Y > 510) {
					MovingDango.Ywhen1Frame *= -1;
				}
				*/
				MovingDango.Y += 0;
				Core.Draw(DangoL[(int)MovingDango.DangoType], MovingDango.X, MovingDango.Y);

				if (MovingDango.LimitX < MovingDango.X) {
					MovingDango.Moving = false;
					DangoMatrix[MovingDango.MatrixX][MovingDango.MatrixY] = MovingDango;
					DeleteMatchedDango(MovingDango.MatrixX, MovingDango.MatrixY);
					DeleteBlankLineDango();

					CanThrow = true;
				}

			}

			if (FCounter < 0) {
				FCounter = 60;
				WorkTime++;
				Texture.SetTextSize(48);
				Texture.SetTextColor(255, 255, 255);
				Texture.SetFont("Meiryo"); 
				TTimer = Texture.CreateFromText("" + (AddSpeed - WorkTime));
				if (WorkTime >= AddSpeed) {
					WorkTime = 0;
					AddDangoLine(1);
				}
			}
			FCounter--;

			//FRONT
			Core.Draw(GameBG_Front, 0, 0, 255);
			Core.Draw(DangoL[(int)NextDango], 75, 620, 255);
			Core.Draw(DangoL[(int)NextDango2], 75 + 10 + 64, 620, 255);
			Core.Draw(TScore, 1000, 50, 255);
			Core.Draw(TTimer, 800, 50, 255);


			if (GameOver) {
				if (!GameOverEx) {
					Effect.Reset();
					DNGGO.Play();
					GameOverEx = true;
				} else {
					if (Effect.Fadeout() == ContentReturn.END) {
						Scene.Set("Title");
						return ContentReturn.CHANGE;
					}
				}
			}


			return ContentReturn.OK;
		}
	}
}
