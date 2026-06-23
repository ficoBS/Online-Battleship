using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public enum ShipType
    {
        Carrier,      // 5
        Battleship,   // 4
        Cruiser,      // 3
        Submarine,    // 3
        Destroyer     // 2
    }

    public enum ShipOrientation
    {
        Horizontal,
        Vertical
    }

    public class Ship
    {
        public ShipType Type { get; set; }
        public int Size { get; set; }
        public int HitCount { get; set; }
        public ShipOrientation Orientation { get; set; }
        public List<Cell> Cells { get; set; }

        public bool IsSunk => HitCount >= Size;

        public Ship(ShipType type)
        {
            Type = type;
            HitCount = 0;
            Orientation = ShipOrientation.Horizontal;
            Cells = new List<Cell>();

            Size = type switch
            {
                ShipType.Carrier => 5,
                ShipType.Battleship => 4,
                ShipType.Cruiser => 3,
                ShipType.Submarine => 3,
                ShipType.Destroyer => 2,
                _ => 0
            };
        }
    }
}