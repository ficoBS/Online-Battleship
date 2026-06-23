using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public enum GameState
    {
        WaitingForPlayers,
        PlacingShips,
        InProgress,
        Finished
    }

    public class Game
    {
        public string GameId { get; set; }
        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public Board Board1 { get; set; }
        public Board Board2 { get; set; }
        public User CurrentTurn { get; set; }
        public GameState State { get; set; }
        public User Winner { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public Game(User player1, User player2)
        {
            GameId = Guid.NewGuid().ToString();
            Player1 = player1;
            Player2 = player2;
            Board1 = new Board();
            Board2 = new Board();
            CurrentTurn = player1;
            State = GameState.WaitingForPlayers;
            StartedAt = DateTime.Now;
        }

        public CellState ProcessShot(User shooter, int row, int col)
        {
            Board targetBoard = shooter == Player1 ? Board2 : Board1;
            CellState result = targetBoard.ReceiveShot(row, col);

            if (targetBoard.AllShipsSunk)
            {
                State = GameState.Finished;
                Winner = shooter;
                EndedAt = DateTime.Now;
            }
            else
            {
                CurrentTurn = CurrentTurn == Player1 ? Player2 : Player1;
            }

            return result;
        }

        public bool IsPlayerTurn(User player) => CurrentTurn == player;
    }
}