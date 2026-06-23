using Online_Battleship.Models;

namespace Online_Battleship.Views;

public partial class GamePage : ContentPage
{
    private const int BoardSize = 10;
    private Button[,] playerButtons = new Button[BoardSize, BoardSize];
    private Button[,] enemyButtons = new Button[BoardSize, BoardSize];

    public GamePage()
    {
        InitializeComponent();
        BuildBoard(playerBoard, playerButtons, isEnemy: false);
        BuildBoard(enemyBoard, enemyButtons, isEnemy: true);
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

    private void OnEnemyCellClicked(int row, int col)
    {
        // shooting logic will go here
    }

    private void butSend_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(chatEntry.Text)) return;

        var label = new Label
        {
            Text = $"You: {chatEntry.Text}",
            TextColor = Colors.LightBlue
        };
        chatLog.Children.Add(label);
        chatEntry.Text = "";
    }
}