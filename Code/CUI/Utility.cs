using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace CUI {
	public abstract class Utility {
		#region Public Static Properties
		public static readonly string SelectPrefix = "|-> ";
		#endregion

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

		#region DisplayOptions
		public static void DisplayOptions(List<string> optieLijst, bool metEinde = false) {
			Console.ResetColor();
			for (int i = 0; i < optieLijst.Count; i++) {
				if (i == FitnessApp.SelectedIndex) {
					if (FitnessApp.GaVerderOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Green;
					} else if (FitnessApp.StopOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Red;
					} else
						Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
					Console.WriteLine($"{SelectPrefix}{optieLijst[i]}");
					Console.ResetColor();
				} else {
					if (FitnessApp.GaVerderOpties.Contains(optieLijst[i]))
						Console.ForegroundColor = ConsoleColor.Green;
					else if (FitnessApp.StopOpties.Contains(optieLijst[i])) {
						Console.ForegroundColor = ConsoleColor.Red;
					} else
						Console.ResetColor();
					Console.WriteLine($"    {optieLijst[i]}");
				}
			}
			Console.ResetColor();
		}
		#endregion

		#region OptieLijstConroller()
		public static int OptieLijstConroller(List<string> optieLijst, string prompt = "", bool metEinde = false) {
			Console.CursorVisible = false;
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
			Console.CursorVisible = true;
			return FitnessApp.SelectedIndex;
		}
		#endregion

		#region SchrijfUnderline()
		public static string SchrijfUnderline(string @string) {
			var handle = GetStdHandle(STD_OUTPUT_HANDLE);
			uint mode;
			GetConsoleMode(handle, out mode);
			mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			SetConsoleMode(handle, mode);
			return $"\x1B[4m{@string}\x1B[24m";
		}
		#endregion

		#region Logger
		public static class Logger {
			public static void Error(Exception error, bool newLine = true, bool metAchtergrond = true, bool clearConsole = false, bool metKeyPress = true) {
				Console.ResetColor();
				if (clearConsole) Console.Clear();
				if (metAchtergrond) {
					Console.BackgroundColor = FitnessApp.DefaultErrorBackgroundPrintLineColor;
					Console.ForegroundColor = FitnessApp.DefaultErrorForeGrountPrintLineColor;
				} else
					Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
				if (newLine)
					Console.WriteLine($"{error.Message}\n");
				else
					Console.Write($"{error.Message}");
				Console.ResetColor();
				if (metKeyPress)
					ColorInput.ReadKnop();
			}

			public static void Error(string message, bool newLine = true, bool metAchtergrond = true, bool clearConsole = false, bool metKeyPress = true) {
				if (newLine)
					Error(new Exception(message), true, metAchtergrond, clearConsole: clearConsole, metKeyPress: metKeyPress);
				else
					Error(new Exception(message), false, metAchtergrond, clearConsole: clearConsole, metKeyPress: metKeyPress);
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
				if (newLine)
					Info(new Exception(message), true, metAchtergrond, color: color);
				else
					Info(new Exception(message), false, metAchtergrond, color: color);
			}
		}
		#endregion

		#region Colorinput
		public static class ColorInput {
			public static string ReadInput(ConsoleColor color = ConsoleColor.White, string prompt = "", bool metAchtergrond = false) {
				Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
				Logger.Info(prompt, false, metAchtergrond: metAchtergrond);
				if (color == ConsoleColor.White)
					Console.ForegroundColor = FitnessApp.DefaultInfoBackgroundPrintLineColor;
				else
					Console.ForegroundColor = color;
				string input = Console.ReadLine();
				Console.ResetColor();
				return input;
			}

			public static void ReadKnop(ConsoleColor color = ConsoleColor.White, string prompt = "Klik op een knop om verder te gaan.") {
				Console.CursorVisible = false;
				Console.ForegroundColor = FitnessApp.DefaultReadLineColor;
				Logger.Info(prompt, false, false);
				Console.ForegroundColor = color;
				Console.ReadKey(false);
				if (color != ConsoleColor.White)
					Console.ResetColor();
				Console.CursorVisible = true;
			}
		}
		#endregion

		#region Table
		public class Table {
			private const string TopLeftJoint = "┌";
			private const string TopRightJoint = "┐";
			private const string BottomLeftJoint = "└";
			private const string BottomRightJoint = "┘";
			private const string TopJoint = "┬";
			private const string BottomJoint = "┴";
			private const string LeftJoint = "├";
			private const string MiddleJoint = "┼";
			private const string RightJoint = "┤";
			private const char HorizontalLine = '─';
			private const string VerticalLine = "│";

			private string[] _headers;
			private List<string[]> _rows = new List<string[]>();

			public int Padding { get; set; } = 1;
			public bool HeaderTextAlignRight { get; set; }
			public bool RowTextAlignRight { get; set; }

			public void SetHeaders(params string[] headers) {
				_headers = headers;
			}

			public void AddRow(params string[] row) {
				_rows.Add(row);
			}

			public void Clear() {
				_rows.Clear();
				_headers = new string[0];
			}

			private int[] GetMaxCellWidths(List<string[]> table) {
				var maximumColumns = 0;
				foreach (var row in table) {
					if (row.Length > maximumColumns)
						maximumColumns = row.Length;
				}

				var maximumCellWidths = new int[maximumColumns];
				for (int i = 0; i < maximumCellWidths.Count(); i++)
					maximumCellWidths[i] = 0;

				var paddingCount = 0;
				if (Padding > 0) {
					//Padding is left and right
					paddingCount = Padding * 2;
				}

				foreach (var row in table) {
					for (int i = 0; i < row.Length; i++) {
						var maxWidth = row[i].Length + paddingCount;

						if (maxWidth > maximumCellWidths[i])
							maximumCellWidths[i] = maxWidth;
					}
				}

				return maximumCellWidths;
			}

			private StringBuilder CreateTopLine(int[] maximumCellWidths, int rowColumnCount, StringBuilder formattedTable) {
				for (int i = 0; i < rowColumnCount; i++) {
					if (i == 0 && i == rowColumnCount - 1)
						formattedTable.AppendLine(string.Format("{0}{1}{2}", TopLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
					else if (i == 0)
						formattedTable.Append(string.Format("{0}{1}", TopLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
					else if (i == rowColumnCount - 1)
						formattedTable.AppendLine(string.Format("{0}{1}{2}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
					else
						formattedTable.Append(string.Format("{0}{1}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
				}

				return formattedTable;
			}

			private StringBuilder CreateBottomLine(int[] maximumCellWidths, int rowColumnCount, StringBuilder formattedTable) {
				for (int i = 0; i < rowColumnCount; i++) {
					if (i == 0 && i == rowColumnCount - 1)
						formattedTable.AppendLine(string.Format("{0}{1}{2}", BottomLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
					else if (i == 0)
						formattedTable.Append(string.Format("{0}{1}", BottomLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
					else if (i == rowColumnCount - 1)
						formattedTable.AppendLine(string.Format("{0}{1}{2}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
					else
						formattedTable.Append(string.Format("{0}{1}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
				}

				return formattedTable;
			}

			private StringBuilder CreateValueLine(int[] maximumCellWidths, string[] row, bool alignRight, StringBuilder formattedTable) {
				int cellIndex = 0;
				int lastCellIndex = row.Length - 1;

				var paddingString = string.Empty;
				if (Padding > 0)
					paddingString = string.Concat(Enumerable.Repeat(' ', Padding));

				foreach (var column in row) {
					var restWidth = maximumCellWidths[cellIndex];
					if (Padding > 0)
						restWidth -= Padding * 2;

					var cellValue = alignRight ? column.PadLeft(restWidth, ' ') : column.PadRight(restWidth, ' ');

					if (cellIndex == 0 && cellIndex == lastCellIndex)
						formattedTable.AppendLine(string.Format("{0}{1}{2}{3}{4}", VerticalLine, paddingString, cellValue, paddingString, VerticalLine));
					else if (cellIndex == 0)
						formattedTable.Append(string.Format("{0}{1}{2}{3}", VerticalLine, paddingString, cellValue, paddingString));
					else if (cellIndex == lastCellIndex)
						formattedTable.AppendLine(string.Format("{0}{1}{2}{3}{4}", VerticalLine, paddingString, cellValue, paddingString, VerticalLine));
					else
						formattedTable.Append(string.Format("{0}{1}{2}{3}", VerticalLine, paddingString, cellValue, paddingString));

					cellIndex++;
				}

				return formattedTable;
			}

			private StringBuilder CreateSeperatorLine(int[] maximumCellWidths, int previousRowColumnCount, int rowColumnCount, StringBuilder formattedTable) {
				var maximumCells = Math.Max(previousRowColumnCount, rowColumnCount);

				for (int i = 0; i < maximumCells; i++) {
					if (i == 0 && i == maximumCells - 1) {
						formattedTable.AppendLine(string.Format("{0}{1}{2}", LeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), RightJoint));
					} else if (i == 0) {
						formattedTable.Append(string.Format("{0}{1}", LeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
					} else if (i == maximumCells - 1) {
						if (i > previousRowColumnCount)
							formattedTable.AppendLine(string.Format("{0}{1}{2}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
						else if (i > rowColumnCount)
							formattedTable.AppendLine(string.Format("{0}{1}{2}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
						else if (i > previousRowColumnCount - 1)
							formattedTable.AppendLine(string.Format("{0}{1}{2}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
						else if (i > rowColumnCount - 1)
							formattedTable.AppendLine(string.Format("{0}{1}{2}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
						else
							formattedTable.AppendLine(string.Format("{0}{1}{2}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), RightJoint));
					} else {
						if (i > previousRowColumnCount)
							formattedTable.Append(string.Format("{0}{1}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
						else if (i > rowColumnCount)
							formattedTable.Append(string.Format("{0}{1}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
						else
							formattedTable.Append(string.Format("{0}{1}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
					}
				}

				return formattedTable;
			}

			public override string ToString() {
				var table = new List<string[]>();

				var firstRowIsHeader = false;
				if (_headers?.Any() == true) {
					table.Add(_headers);
					firstRowIsHeader = true;
				}

				if (_rows?.Any() == true)
					table.AddRange(_rows);

				if (!table.Any())
					return string.Empty;

				var formattedTable = new StringBuilder();

				var previousRow = table.FirstOrDefault();
				var nextRow = table.FirstOrDefault();

				int[] maximumCellWidths = GetMaxCellWidths(table);

				formattedTable = CreateTopLine(maximumCellWidths, nextRow.Count(), formattedTable);

				int rowIndex = 0;
				int lastRowIndex = table.Count - 1;

				for (int i = 0; i < table.Count; i++) {
					var row = table[i];

					var align = RowTextAlignRight;
					if (i == 0 && firstRowIsHeader)
						align = HeaderTextAlignRight;

					formattedTable = CreateValueLine(maximumCellWidths, row, align, formattedTable);

					previousRow = row;

					if (rowIndex != lastRowIndex) {
						nextRow = table[rowIndex + 1];
						formattedTable = CreateSeperatorLine(maximumCellWidths, previousRow.Count(), nextRow.Count(), formattedTable);
					}

					rowIndex++;
				}

				formattedTable = CreateBottomLine(maximumCellWidths, previousRow.Count(), formattedTable);

				return formattedTable.ToString();
			}
		}
		#endregion
	}
}