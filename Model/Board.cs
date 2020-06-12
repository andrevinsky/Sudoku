using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Ext;

namespace Sudoku.Model
{
	public class Board : IEnumerable<Nine>
	{
		public List<Nine> Rows { get; set; }
		public List<Nine> Cols { get; set; }
		public List<Nine> Groups { get; set; }
		public List<Nine> ExtraConstraints { get; set; }
		public List<Nine> AllContraints { get; set; }
		public bool IsX { get; set; }

		public List<Cell> FreeCells
		{
			get
			{
				return (from Nine nine in Rows
						  from Cell cell in nine
						  where cell.Value == 0
						  select cell).ToList();
			}
		}

		private bool isValidExtraCheckNeeded = false;
		private bool isValid;
		public bool IsValid {
			get
			{
				if (isValidExtraCheckNeeded)
				{
					bool isValidLocal = ReEvaluate();
					isValid = isValidLocal;
					if (isValidLocal)
						isValidExtraCheckNeeded = false;
				}
				return isValid;
			}
			set {
				isValid = value;
				if (!isValid) 
					isValidExtraCheckNeeded = true;
			}
		}

		private bool ReEvaluate()
		{
			ConflictingNines.Clear();
			foreach (Nine nine in AllContraints)
			{
				if (!nine.IsValid())
					ConflictingNines.Add(nine);
			}
			return ConflictingNines.Count == 0;
		}

		public List<Nine> ConflictingNines { get; set; }

		public Board(bool isX)
		{
			Rows = new List<Nine>(9);
			Cols = new List<Nine>(9);
			Groups = new List<Nine>(9);
			IsX = isX;
			if (isX)
			{
				ExtraConstraints = new List<Nine>(2);
				AllContraints = new List<Nine>(29);				 
			}
			else
			{
				ExtraConstraints = new List<Nine>(0);
				AllContraints = new List<Nine>(27);
			}
			ConflictingNines = new List<Nine>();
			IsValid = true;
			InnerInitialize();
		}

		private void InnerInitialize()
		{
			for (int r = 0; r < 9; r++)
			{
				Nine row = new Nine
				{
					Attribute = string.Format("Row: {0}", r + 1)
				};
				for (int c = 0; c < 9; c++)
				{
					row.Add(new Cell
					{
						Col = c,
						Row = r
					});
				}
				Rows.Add(row);
			}

			for (int r = 0; r < 9; r++)
			{
				Nine col = new Nine
				{
					Attribute = string.Format("Column: {0}", r + 1)
				};
				(from Nine nine in Rows
				 select nine[r]).Each(cell => col.Add(cell));
				Cols.Add(col);
			}

			for (int r = 0; r < 9; r++)
			{
				Nine group = new Nine
				{
					Attribute = string.Format("Group: {0}", r + 1)
				};
				for (int c = 0; c < 9; c++)
				{
					int rmod3 = r % 3;
					int cmod3 = c % 3;
					int r3 = r / 3;
					int c3 = c / 3;
					
					int gIdx = r3 * 3 + c3;
					int cInd = rmod3 * 3 + cmod3;

					group.Add(Rows[r3 * 3 +c3][rmod3 * 3 + cmod3]);
				}
				Groups.Add(group);
			}

			if (IsX)
			{
				Nine slashF = new Nine
				{
					Attribute = "Slash Forward"
				};
				Nine slashB = new Nine
				{
					Attribute = "Backslash"
				};
				for (int i = 0; i < 9; i++)
				{
					slashB.Add(Rows[i][i]);
					slashF.Add(Rows[8 - i][i]);
				}
				ExtraConstraints.Add(slashB);
				ExtraConstraints.Add(slashF);
			}

			AllContraints.AddRange(Rows.Union(Cols).Union(Groups).Union(ExtraConstraints));

			(from Nine nine in AllContraints
					  select nine).Each(nine => {
						  nine.OnValidationError += new Action<Nine>(nine_OnValidationError);
					  });

		}

		void nine_OnValidationError(Nine obj)
		{
			IsValid = false;
			ConflictingNines.Add(obj);
		}

		public bool Initialize(int[] sourceArray)
		{
			if (sourceArray.Length != 9 * 9)
				throw new IndexOutOfRangeException("sourceArray");

			for (int i = 0; i < sourceArray.Length; i++)
			{
				int r = i / 9;
				int c = i % 9;
				Rows[r][c].Value = sourceArray[i];
				if (!IsValid)
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, Rows.Each<Nine, string>(nine => nine.ToString()).ToArray());
		}

		#region IEnumerable<Nine> Members

		public IEnumerator<Nine> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion


	}
}
