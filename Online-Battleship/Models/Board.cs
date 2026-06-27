using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public class Board
    {
        public const int Size = 10;
        public Cell[,] Cells { get; set; }
        public List<Ship> Ships { get; set; }

        public Board()
        {
            Cells = new Cell[Size, Size];
            Ships = new List<Ship>();

            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                    Cells[row, col] = new Cell(row, col);
        }

        public bool PlaceShip(Ship ship, int startRow, int startCol, ShipOrientation orientation)
        {
            ship.Orientation = orientation;

            if (!IsValidPlacement(ship, startRow, startCol))
                return false;

            for (int i = 0; i < ship.Size; i++)
            {
                int row = orientation == ShipOrientation.Horizontal ? startRow : startRow + i;
                int col = orientation == ShipOrientation.Horizontal ? startCol + i : startCol;

                Cells[row, col].State = CellState.Ship;
                ship.Cells.Add(Cells[row, col]);
            }

            Ships.Add(ship);
            return true;
        }

        public bool IsValidPlacement(Ship ship, int startRow, int startCol)
        {
            for (int i = 0; i < ship.Size; i++)
            {
                int row = ship.Orientation == ShipOrientation.Horizontal ? startRow : startRow + i;
                int col = ship.Orientation == ShipOrientation.Horizontal ? startCol + i : startCol;

                if (row >= Size || col >= Size) return false;
                if (Cells[row, col].State == CellState.Ship) return false;
            }
            return true;
        }

        public CellState ReceiveShot(int row, int col)
        {
            Cell cell = Cells[row, col];

            if (cell.State == CellState.Ship)
            {
                cell.State = CellState.Hit;
                Ship ship = Ships.First(s => s.Cells.Contains(cell));
                ship.HitCount++;
                return CellState.Hit;
            }
            else
            {
                cell.State = CellState.Miss;
                return CellState.Miss;
            }
        }

        public void RemoveShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                cell.State = CellState.Empty;
            }
            Ships.Remove(ship);
        }

        public bool AllShipsSunk => Ships.All(s => s.IsSunk);
    }
}