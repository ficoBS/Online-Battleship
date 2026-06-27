using Online_Battleship.Models;
using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class ShipPlacementPage : ContentPage
{
#if ANDROID
    private int cellSize = 35;
#else
    private int cellSize = 30;
#endif
    private const int BoardSize = 10;
    private Button[,] buttons = new Button[BoardSize, BoardSize];
    private Board board = new Board();
    private Ship selectedShip;
    private ShipOrientation orientation = ShipOrientation.Horizontal;
    private HashSet<ShipType> placedShips = new HashSet<ShipType>();

    private Dictionary<ShipType, (Button btn, Border badge)> shipControls;
    private static readonly Dictionary<ShipType, Color> shipColors = new()
    {
        { ShipType.Carrier,    Color.FromArgb("#2E8B57") },
        { ShipType.Battleship, Color.FromArgb("#8B0000") },
        { ShipType.Cruiser,    Color.FromArgb("#FF8C00") },
        { ShipType.Submarine,  Color.FromArgb("#4B0082") },
        { ShipType.Destroyer,  Color.FromArgb("#008B8B") },
    };

    private readonly IOrientationService _orientationService;

    public ShipPlacementPage(IOrientationService orientationService)
    {
        InitializeComponent();
        _orientationService = orientationService;

        shipControls = new Dictionary<ShipType, (Button, Border)>
        {
            { ShipType.Carrier,    (butCarrier,    badgeCarrier)    },
            { ShipType.Battleship, (butBattleship, badgeBattleship) },
            { ShipType.Cruiser,    (butCruiser,    badgeCruiser)    },
            { ShipType.Submarine,  (butSubmarine,  badgeSubmarine)  },
            { ShipType.Destroyer,  (butDestroyer,  badgeDestroyer)  },
        };

        foreach (var (type, (btn, badge)) in shipControls)
        {
            var t = type;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => RemovePlacedShip(t);
            badge.GestureRecognizers.Add(tap);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _orientationService.SetLandscape();
        resetBoard();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _orientationService.SetPortrait();
    }

    private void resetBoard()
    {
        board = new Board();
        selectedShip = null;
        orientation = ShipOrientation.Horizontal;
        placedShips.Clear();
        butReady.IsEnabled = false;
        butReady.Text = "Ready";
        butRotate.Text = "Rotate: Horizontal";
        placementBoard.Clear();
        placementBoard.RowDefinitions.Clear();
        placementBoard.ColumnDefinitions.Clear();

        foreach (var (type, (btn, badge)) in shipControls)
        {
            btn.IsEnabled = true;
            btn.BackgroundColor = Color.FromArgb("#ADD8E6");
            btn.BorderColor = Colors.Transparent;
            btn.BorderWidth = 0;
            badge.IsVisible = false;
        }

        BuildBoard();
    }

    private void BuildBoard()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            placementBoard.RowDefinitions.Add(new RowDefinition { Height = cellSize });
            placementBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = cellSize });
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

    private void SetSelectedShip(ShipType? newType)
    {
        foreach (var (type, (btn, _)) in shipControls)
        {
            if (!placedShips.Contains(type))
            {
                btn.BackgroundColor = Color.FromArgb("#ADD8E6");
                btn.BorderColor = Colors.Transparent;
                btn.BorderWidth = 0;
            }
        }

        if (newType is null) return;

        var selected = shipControls[newType.Value].btn;
        selected.BackgroundColor = Color.FromArgb("#87CEEB");
        selected.BorderColor = Color.FromArgb("#FFD700");
        selected.BorderWidth = 3;
    }

    private async void ShipButton_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

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

        if (selectedShip?.Type == type)
        {
            selectedShip = null;
            SetSelectedShip(null);
            return;
        }

        selectedShip = new Ship(type);
        SetSelectedShip(type);
    }

    private async void butRotate_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        orientation = orientation == ShipOrientation.Horizontal
            ? ShipOrientation.Vertical
            : ShipOrientation.Horizontal;

        butRotate.Text = $"Rotate: {orientation}";
    }

    private async void OnCellClicked(int row, int col)
    {
        await SoundService.PlayClickAsync();

        if (selectedShip == null) return;

        bool placed = board.PlaceShip(selectedShip, row, col, orientation);

        if (!placed)
        {
            await DisplayAlert("Invalid", "Cannot place ship here!", "OK");
            return;
        }

        foreach (var cell in selectedShip.Cells)
            buttons[cell.Row, cell.Col].BackgroundColor = shipColors[selectedShip.Type];

        var (btn, badge) = shipControls[selectedShip.Type];
        btn.IsEnabled = false;
        btn.BackgroundColor = Color.FromArgb("#555555");
        btn.BorderColor = Colors.Transparent;
        btn.BorderWidth = 0;
        badge.IsVisible = true;

        placedShips.Add(selectedShip.Type);
        selectedShip = null;
        SetSelectedShip(null);

        if (placedShips.Count == 5)
            butReady.IsEnabled = true;
    }

    private async void RemovePlacedShip(ShipType type)
    {
        await SoundService.PlayClickAsync();

        if (!placedShips.Contains(type)) return;

        var ship = board.Ships.FirstOrDefault(s => s.Type == type);
        if (ship == null) return;

        foreach (var cell in ship.Cells)
            buttons[cell.Row, cell.Col].BackgroundColor = Color.FromArgb("#1a6fa8");

        board.RemoveShip(ship);

        var (btn, badge) = shipControls[type];
        btn.IsEnabled = true;
        btn.BackgroundColor = Color.FromArgb("#ADD8E6");
        btn.BorderColor = Colors.Transparent;
        btn.BorderWidth = 0;
        badge.IsVisible = false;

        placedShips.Remove(type);
        butReady.IsEnabled = false;

        selectedShip = new Ship(type);
        SetSelectedShip(type);
    }

    private async void butReady_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        butReady.IsEnabled = false;
        butReady.Text = "Waiting for opponent...";
        SessionService.PlayerBoard = board;
        await SessionService.Hub.ShipsReady(SessionService.CurrentGameId);
    }
}