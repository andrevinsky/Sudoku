using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku.Model
{
	public class Command
	{
		public int Row { get; set; }
		public int Col { get; set; }
		public int Value { get; set; }
		public int ReplacedValue { get; set; }
	}
	public class BoardController
	{
		public Board Board { get; private set; }
		public Stack<Command> History { get; private set; }
		public List<Command> SuggestedMoves { get; private set; }

		public BoardController(Board board)
		{
			Board = board;
			History = new Stack<Command>();
			SuggestedMoves = new List<Command>();
		}

		public void SetCommand(int row, int col, int value)
		{
			History.Push(new Command
			{
				Row = row,
				Col = col,
				Value = value,
				ReplacedValue = Board.Rows[row][col].Value
			});
			Board.Rows[row][col].Value = value;
		}
		public void Undo()
		{
			if (History.Count == 0)
			{
				Console.WriteLine("Cannot undo");
				return;
			}
			var command = History.Pop();
			if (command == null) return;

			Board.Rows[command.Row][command.Col].Value = command.ReplacedValue;
		}


		public bool DispatchCommand(string userInput)
		{
			if (string.Equals(userInput, "u", StringComparison.OrdinalIgnoreCase))
			{
				Undo();
				return true;
			}
			if (string.Equals(userInput, "x", StringComparison.OrdinalIgnoreCase))
				return false;

			var threeDigits = (from char digit in userInput.ToCharArray()
									 where Char.IsDigit(digit)
									 select int.Parse(digit.ToString())).ToArray();

			if ((threeDigits.Length == 1) && SuggestedMoves.Count > 0)
			{
				var move = SuggestedMoves[threeDigits[0] - 1];
				SetCommand(move.Row, move.Col, move.Value);
				return true;
			}
			if (threeDigits.Length == 3)
			{
				if (userInput.StartsWith("/"))
					RowColumnReference(threeDigits[0], threeDigits[1], threeDigits[2]);
				else
					FractalReference(threeDigits[0], threeDigits[1], threeDigits[2]);
				return true;
			}
			Console.WriteLine("Unknown command");
			return true;
		}

		private void RowColumnReference(int row, int col, int val)
		{
			SetCommand(row - 1, col - 1, val);
		}

		private void FractalReference(int numpadG, int numpadS, int val)
		{
			int?[] map = new int?[] { null, 6, 7, 8, 3, 4, 5, 0, 1, 2 };
			var step1 = map[numpadG];
			var step2 = map[numpadS];
			if (!step1.HasValue || !step2.HasValue)
			{
				Console.WriteLine("Unknown position");
				return;
			}
			int step13 = step1.Value / 3;
			int step1mod3 = step1.Value % 3;

			int step23 = step2.Value / 3;
			int step2mod3 = step2.Value % 3;

			int row = step13 * 3 + step23;
			int col = step1mod3 * 3 + step2mod3;

			SetCommand(row, col, val);
		}

		public bool Initialize(string[] StartingBoard)
		{
			var values = from string item in StartingBoard
							 from char digit in item.ToCharArray()
							 where Char.IsDigit(digit)
							 select int.Parse(digit.ToString());
			return Board.Initialize(values.ToArray());
		}



		public void SuggestAMove()
		{
			var freeCells = Board.FreeCells;
			SuggestedMoves.Clear();
			for (int i = 0; i < freeCells.Count; i++)
			{
				Cell trialCell = freeCells[i];
				bool[] results = new bool[9];
				for (int v = 1; v < 10; v++)
				{
					SetCommand(trialCell.Row, trialCell.Col, v);
					results[v - 1] = Board.IsValid;
					Undo();
				}
				if (results.Count(item => item) == 1)
				{
					SuggestedMoves.Add(new Command
					{
						Col = trialCell.Col,
						Row = trialCell.Row,
						Value = Array.IndexOf<bool>(results, true) + 1
					});
				}
			}
			if (SuggestedMoves.Count == 0)
			{
				Console.WriteLine("No suggested moves");
				return;
			}
			Console.WriteLine("Suggested moves are:");
			foreach (Command cmd in SuggestedMoves)
				Console.WriteLine("R{0}C{1}: {2}", cmd.Row + 1, cmd.Col + 1, cmd.Value);
		}
	}
}
