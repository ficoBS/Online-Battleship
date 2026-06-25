using Online_Battleship.Models;
using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class GamePage : ContentPage
{
    private const int BoardSize = 10;
    private Button[,] playerButtons = new Button[BoardSize, BoardSize];
    private Button[,] enemyButtons = new Button[BoardSize, BoardSize];
    private bool myTurn = false;

    public GamePage()
    {
        InitializeComponent();
        BuildBoard(playerBoard, playerButtons, isEnemy: false);
        BuildBoard(enemyBoard, enemyButtons, isEnemy: true);
        ShowPlayerShips();

        SessionService.Hub.OnOpponentShot += OnOpponentShot;
        SessionService.Hub.OnShotResult += OnShotResult;
        SessionService.Hub.OnReceiveMessage += OnReceiveMessage;
        SessionService.Hub.OnGameEnded += OnGameEnded;
    }

    private void ShowPlayerShips()
    {
        if (SessionService.PlayerBoard == null) return;

        for (int row = 0; row < 10; row++)
            for (int col = 0; col < 10; col++)
                if (SessionService.PlayerBoard.Cells[row, col].State == Models.CellState.Ship)
                    playerButtons[row, col].BackgroundColor = Colors.Gray;
    }

    private void BuildBoard(Grid grid, Button[,] buttons, bool isEnemy)
    {
        for (int i = 0; i < BoardSize; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = 35 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 35 });
        }

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                var btn = new Button
                {
                    BackgroundColor = Color.FromArgb("#1a6fa8"),
                    CornerRadius = 3,
                    Margin = 1
                };

                int r = row, c = col;
                if (isEnemy)
                    btn.Clicked += (s, e) => OnEnemyCellClicked(r, c);

                buttons[row, col] = btn;
                grid.Add(btn, col, row);
            }
        }
    }



    private async void OnEnemyCellClicked(int row, int col)
    {
        if (!SessionService.IsMyTurn) return;

        SessionService.IsMyTurn = false;
        enemyButtons[row, col].IsEnabled = false;

        await SessionService.Hub.Shoot(SessionService.CurrentGameId, row, col);
    }

    private void OnOpponentShot(int row, int col)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = SessionService.PlayerBoard.ReceiveShot(row, col);
            var cell = playerButtons[row, col];

            if (result == Models.CellState.Hit)
            {
                cell.BackgroundColor = Colors.Red;
                AddLog($"Enemy hit at {(char)('A' + col)}{row + 1}!", Colors.Red);
                await SessionService.Hub.ShotResult(SessionService.CurrentGameId, row, col, "Hit");

                if (SessionService.PlayerBoard.AllShipsSunk)
                    await SessionService.Hub.GameOver(SessionService.CurrentGameId, SessionService.OpponentId, SessionService.UserId);
            }
            else
            {
                cell.BackgroundColor = Colors.White;
                AddLog($"Enemy missed at {(char)('A' + col)}{row + 1}", Colors.Gray);
                await SessionService.Hub.ShotResult(SessionService.CurrentGameId, row, col, "Miss");
            }

            SessionService.IsMyTurn = true;
        });
    }

    private void OnShotResult(int row, int col, string result)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (result == "Hit")
            {
                enemyButtons[row, col].BackgroundColor = Colors.Red;
                AddLog($"You hit at {(char)('A' + col)}{row + 1}!", Colors.Green);
            }
            else
            {
                enemyButtons[row, col].BackgroundColor = Colors.White;
                AddLog($"You missed at {(char)('A' + col)}{row + 1}", Colors.Gray);
            }
        });
    }

    private void OnReceiveMessage(string username, string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var label = new Label
            {
                TextColor = username == SessionService.Username ? Colors.LightBlue : Colors.White
            };
            label.FormattedText = new FormattedString();
            label.FormattedText.Spans.Add(new Span { Text = $"{username}: ", FontAttributes = FontAttributes.Bold, TextColor = label.TextColor });
            label.FormattedText.Spans.Add(new Span { Text = message, TextColor = label.TextColor });
            chatLog.Children.Add(label);
        });
    }

    private void OnGameEnded(int winnerId)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string result = winnerId == SessionService.UserId ? "You Win!" : "You Lose!";
            await DisplayAlert("Game Over", result, "OK");
            await Shell.Current.GoToAsync("//MainPage");
        });
    }
    private void AddLog(string message, Color color)
    {
        var label = new Label { Text = message, TextColor = color, FontSize = 12 };
        chatLog.Children.Add(label);
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100);
            await chatScroll.ScrollToAsync(label, ScrollToPosition.End, true);
        });
    }

    private async void butSend_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(chatEntry.Text)) return;
        await SessionService.Hub.SendMessage(SessionService.CurrentGameId, chatEntry.Text);
        chatEntry.Text = "";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SessionService.Hub.OnOpponentShot -= OnOpponentShot;
        SessionService.Hub.OnShotResult -= OnShotResult;
        SessionService.Hub.OnReceiveMessage -= OnReceiveMessage;
        SessionService.Hub.OnGameEnded -= OnGameEnded;
    }
}