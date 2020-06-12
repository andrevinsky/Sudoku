using System;

namespace Sudoku.Model
{
	public class Cell : IComparable<Cell>
	{
		private int value;
		public int Value
		{
			get { return this.value; }
			set
			{
				var newValue = value;
				var oldValue = this.value;
				this.value = newValue;
				if (oldValue != newValue)
					if (OnValueChanged != null)
						OnValueChanged(oldValue, newValue);
			}
		}

		public int Row { get; set; }
		public int Col { get; set; }

		public Cell()
		{
			Value = 0;
		}

		public Cell(int value)
		{
			if ((value >= 10) || (value <0))
				throw new IndexOutOfRangeException("Cell value");

			Value = value;
		}

		public override string ToString()
		{
			return (Value != 0) ? Value.ToString() : "_";
		}

		public event Action<int, int> OnValueChanged;

		public int CompareTo(Cell other)
		{
			return this.Value - other.Value;
		}
	}

}
