using Online_Battleship.Models;
using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class ShipPlacementPage : ContentPage
{
    private const int BoardSize = 10;
    private Button[,] buttons = new Button[BoardSize, BoardSize];
    private Board board = new Board();
    private Ship selectedShip;
    private ShipOrientation orientation = ShipOrientation.Horizontal;
    private HashSet<ShipType> placedShips = new HashSet<ShipType>();

    public ShipPlacementPage()
    {
        InitializeComponent();
        BuildBoard();
        SessionService.Hub.OnOpponentReady += OnOpponentReady;
    }

    private void BuildBoard()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            placementBoard.RowDefinitions.Add(new RowDefinition { Height = 35 });
            placementBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = 35 });
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
                btn.Clicked += (s, e) => OnCellClicked(r, c);

                buttons[row, col] = btn;
                placementBoard.Add(btn, col, row);
            }
        }
    }

    private void ShipButton_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        ShipType type = btn.Text switch
        {
            "Carrier (5)" => ShipType.Carrier,
            "Battleship (4)" => ShipType.Battleship,
            "Cruiser (3)" => ShipType.Cruiser,
            "Submarine (3)" => ShipType.Submarine,
            "Destroyer (2)" => ShipType.Destroyer,
            _ => ShipType.Carrier
        };

        if (placedShips.Contains(type)) return;
        selectedShip = new Ship(type);
    }

    private void butRotate_Clicked(object sender, EventArgs e)
    {
        orientation = orientation == ShipOrientation.Horizontal
            ? ShipOrientation.Vertical
            : ShipOrientation.Horizontal;

        butRotate.Text = $"Rotate: {orientation}";
    }

    private async void OnCellClicked(int row, int col)
    {
        if (selectedShip == null) return;

        bool placed = board.PlaceShip(selectedShip, row, col, orientation);

        if (!placed)
        {
            await DisplayAlert("Invalid", "Cannot place ship here!", "OK");
            return;
        }

        foreach (var cell in selectedShip.Cells)
            buttons[cell.Row, cell.Col].BackgroundColor = Colors.Gray;

        placedShips.Add(selectedShip.Type);
        selectedShip = null;

        if (placedShips.Count == 5)
            butReady.IsEnabled = true;
    }

    private async void butReady_Clicked(object sender, EventArgs e)
    {
        butReady.IsEnabled = false;
        butReady.Text = "Waiting for opponent...";
        await SessionService.Hub.ShipsReady(SessionService.CurrentGameId);
    }

    private void OnOpponentReady()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync("//GamePage");
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SessionService.Hub.OnOpponentReady -= OnOpponentReady;
    }
}