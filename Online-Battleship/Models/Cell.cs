using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public enum CellState
    {
        Empty,
        Ship,
        Hit,
        Miss
    }

    public class Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public CellState State { get; set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            State = CellState.Empty;
        }
    }
}