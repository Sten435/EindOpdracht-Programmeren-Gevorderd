using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CUI {

	public abstract class Utility {

		#region Public Static Properties

		public static readonly string SelectPrefix = "|-> ";

		#endregion Public Static Properties

		#region Private Fields

		private static readonly string _predixSpace = "    ";

		#endregion Private Fields

		#region Om Underlines te kunnen tekenen in console

		private const int STD_OUTPUT_HANDLE = -11;
		private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll")]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		#endregion Om Underlines te kunnen tekenen in console

		#region DisplayOptions

		public static void DisplayOptions(List<string> optieLijst, bool metEinde = false, bool metPijl = true) {
			Console.ResetColor();

			for (int i = 0; i < optieLijst.Count; i++)
				if (i == FitnessApp.SelectedIndex) {
					string text;
					text = $"{SelectPrefix}{optieLijst[i]}";

					if (!metPijl) text = $"{optieLijst[i]}";

					if (FitnessApp.GaVerderOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Green;
						text = $"{SelectPrefix}{optieLijst[i]}";
					} else if (FitnessApp.StopOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Red;
						text = $"{SelectPrefix}{optieLijst[i]}";
					} else if (FitnessApp.DisabledOptie.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Red;
						text = $"{SelectPrefix}{optieLijst[i]}";
					} else {
						Console.ForegroundColor = FitnessApp.DefaultReadLineColor;

						if (!metPijl) {
							Console.BackgroundColor = FitnessApp.DefaultInfoBackgroundPrintLineColor;
							Console.ForegroundColor = ConsoleColor.Black;
						}
					}

					Console.WriteLine(text);
					Console.ResetColor();
				} else {
					string text;
					text = optieLijst[i];

					if (FitnessApp.GaVerderOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Green;
						if (!metPijl) text = @$"{_predixSpace}{optieLijst[i]}";
					} else if (FitnessApp.StopOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Red;
						if (!metPijl) text = @$"{_predixSpace}{optieLijst[i]}";
					} else Console.ResetColor();

					if (!metPijl) Console.WriteLine(text);
					else Console.WriteLine(@$"{_predixSpace}{optieLijst[i]}");
				}

			Console.ResetColor();
		}

		#endregion DisplayOptions

		#region OptieLijstConroller()

		public static int OptieLijstConroller(List<string> optieLijst, string prompt = "", bool metEinde = false, bool metPijl = true) {
			ConsoleKey consoleKey;

			Console.CursorVisible = false;
			Console.Clear();

			do {
				Logger.Info(prompt);
				DisplayOptions(optieLijst, metEinde: metEinde, metPijl: metPijl);

				consoleKey = Console.ReadKey().Key;

				if (consoleKey == ConsoleKey.UpArrow && FitnessApp.SelectedIndex > 0) FitnessApp.SelectedIndex--;
				else if (consoleKey == ConsoleKey.DownArrow && FitnessApp.SelectedIndex < optieLijst.Count - 1) FitnessApp.SelectedIndex++;

				Console.Clear();
			} while (consoleKey != ConsoleKey.Enter);

			Console.CursorVisible = true;
			return FitnessApp.SelectedIndex;
		}

		#endregion OptieLijstConroller()

		#region SchrijfUnderline()

		public static string SchrijfUnderline(string @string) {
			IntPtr handle;

			handle = GetStdHandle(STD_OUTPUT_HANDLE);
			GetConsoleMode(handle, out uint mode);

			mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			SetConsoleMode(handle, mode);

			return $"\x1B[4m{@string}\x1B[24m";
		}

		#endregion SchrijfUnderline()

		#region Logger

		public static class Logger {

			public static void Error(Exception error, bool newLine = true, bool metAchtergrond = true, bool clearConsole = false, bool metKeyPress = true) {
				Console.ResetColor();

				if (clearConsole) Console.Clear();

				if (metAchtergrond) {
					Console.BackgroundColor = FitnessApp.DefaultErrorBackgroundPrintLineColor;
					Console.ForegroundColor = FitnessApp.DefaultErrorForeGrountPrintLineColor;
				} else Console.ForegroundColor = FitnessApp.DefaultReadLineColor;

				if (newLine) Console.WriteLine($"{error.Message}\n");
				else Console.Write($"{error.Message}");

				Console.ResetColor();

				if (metKeyPress) AskUser.ReadKnop();
			}

			public static void Error(string message, bool newLine = true, bool metAchtergrond = true, bool clearConsole = false, bool metKeyPress = true) {
				if (newLine) Error(new Exception(message), true, metAchtergrond, clearConsole: clearConsole, metKeyPress: metKeyPress);
				else Error(new Exception(message), false, metAchtergrond, clearConsole: clearConsole, metKeyPress: metKeyPress);
			}

			public static void Info(Exception error, bool newLine = true, bool metAchtergrond = true, ConsoleColor color = ConsoleColor.Black) {
				Console.ResetColor();
				if (metAchtergrond) {
					Console.BackgroundColor = FitnessApp.DefaultInfoBackgroundPrintLineColor;
					Console.ForegroundColor = FitnessApp.DefaultInfoForeGrountPrintLineColor;
				} else
					Console.ForegroundColor = FitnessApp.DefaultReadLineColor;

				if (color != ConsoleColor.Black)
					Console.ForegroundColor = color;

				if (newLine)
					Console.WriteLine($"{error.Message}\n");
				else
					Console.Write($"{error.Message}");
				Console.ResetColor();
			}

			public static void Info(string message, bool newLine = true, bool metAchtergrond = true, ConsoleColor color = ConsoleColor.Black) {
				if (newLine) Info(new Exception(message), true, metAchtergrond, color: color);
				else Info(new Exception(message), false, metAchtergrond, color: color);
			}
		}

		#endregion Logger

		#region AskUser

		public static class AskUser {

			public static string ReadInput(ConsoleColor color = ConsoleColor.White, string prompt = "", bool metAchtergrond = false, ConsoleColor promptColor = ConsoleColor.White) {
				string input;

				Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
				Logger.Info(prompt, false, metAchtergrond: metAchtergrond, color: promptColor);

				if (color == ConsoleColor.White) Console.ForegroundColor = FitnessApp.DefaultInfoBackgroundPrintLineColor;
				else Console.ForegroundColor = color;

				Logger.Info(" ", false, metAchtergrond: false);
				input = Console.ReadLine();

				Console.ResetColor();
				return input;
			}

			public static ConsoleKey ReadKnop(ConsoleColor color = ConsoleColor.White, string prompt = "Klik op een knop om verder te gaan.", ConsoleColor promptColor = ConsoleColor.White) {
				ConsoleKey key;

				Console.CursorVisible = false;
				Console.ForegroundColor = FitnessApp.DefaultReadLineColor;

				Logger.Info(prompt, false, false, color: promptColor);
				Console.ForegroundColor = color;

				key = Console.ReadKey(false).Key;
				if (color != ConsoleColor.White) Console.ResetColor();
				Console.CursorVisible = true;

				return key;
			}
		}

		#endregion AskUser
	}
}