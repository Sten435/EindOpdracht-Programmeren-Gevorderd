using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CUI {
	public static class Utility {
		public static readonly string SelectPrefix = "|-> ";
		#region Om Underlines te kunnen tekenen in console
		private const int STD_OUTPUT_HANDLE = -11;
		private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);
		[DllImport("kernel32.dll")]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
		#endregion

		public static void DisplayOptions(List<string> optieLijst, bool metEinde = false) {
			for (int i = 0; i < optieLijst.Count; i++) {
				if (i == FitnessApp.SelectedIndex) {
					if (i == optieLijst.Count - 1 && metEinde && optieLijst[i] == FitnessApp.GaVerderOptie) {
						Console.ForegroundColor = ConsoleColor.Green;
					} else
						Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
					Console.WriteLine(@$"{SelectPrefix}{optieLijst[i]}");
					Console.ResetColor();
				} else {
					if (i == optieLijst.Count - 1 && metEinde && optieLijst[i] == FitnessApp.GaVerderOptie)
						Console.ForegroundColor = ConsoleColor.Green;
					else
						Console.ResetColor();
					Console.WriteLine(@$"    {optieLijst[i]}");
				}
			}
		}
		public static int OptieLijstConroller(List<string> optieLijst, string prompt = "", bool metEinde = false) {
			Console.Clear();
			ConsoleKey consoleKey;
			do {
				Logger.Info(prompt);
				DisplayOptions(optieLijst, metEinde: metEinde);
				consoleKey = Console.ReadKey().Key;
				if (consoleKey == ConsoleKey.UpArrow) {
					if (FitnessApp.SelectedIndex > 0)
						FitnessApp.SelectedIndex--;
				} else if (consoleKey == ConsoleKey.DownArrow) {
					if (FitnessApp.SelectedIndex < optieLijst.Count - 1)
						FitnessApp.SelectedIndex++;
				}
				Console.Clear();
			} while (consoleKey != ConsoleKey.Enter);
			return FitnessApp.SelectedIndex;
		}
		public static string SchrijfUnderline(string @string) {
			var handle = GetStdHandle(STD_OUTPUT_HANDLE);
			uint mode;
			GetConsoleMode(handle, out mode);
			mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			SetConsoleMode(handle, mode);
			return $"\x1B[4m{@string}\x1B[24m";
		}

		public static class Logger {
			public static void Error(Exception error, bool newLine = true, bool metKleur = true) {
				if (metKleur) {
					Console.BackgroundColor = ConsoleColor.DarkRed;
					Console.ForegroundColor = ConsoleColor.White;
				}
				if (newLine)
					Console.WriteLine($"{error.Message}\n");
				else
					Console.Write($"{error.Message}");
				Console.ResetColor();
			}

			public static void Error(string message, bool newLine = true, bool metKleur = true) {
				if (newLine)
					Error(new Exception(message), true, metKleur);
				else
					Error(new Exception(message), false, metKleur);
			}

			public static void Info(Exception error, bool newLine = true, bool metKleur = true) {
				if (metKleur) {
					Console.BackgroundColor = ConsoleColor.DarkYellow;
					Console.ForegroundColor = ConsoleColor.Black;
				}
				if (newLine)
					Console.WriteLine($"{error.Message}\n");
				else
					Console.Write($"{error.Message}");
				Console.ResetColor();
			}

			public static void Info(string message, bool newLine = true, bool metKleur = true) {
				if (newLine)
					Info(new Exception(message), true, metKleur);
				else
					Info(new Exception(message), false, metKleur);
			}
		}
		public static class ColorInput {
			public static string ReadInput(ConsoleColor color = ConsoleColor.White, string prompt = "") {
				Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
				Logger.Info(prompt, false, false);
				Console.ForegroundColor = color;
				string input = Console.ReadLine();
				if (color != ConsoleColor.White)
					Console.ResetColor();
				return input;
			}
		}
	}
}