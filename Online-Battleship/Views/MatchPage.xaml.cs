using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class MatchPage : ContentPage
{
    public MatchPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        SessionService.Hub.OnWaitingForOpponent += OnWaitingForOpponent;
        await SessionService.Hub.JoinMatchmaking(SessionService.UserId);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SessionService.Hub.OnWaitingForOpponent -= OnWaitingForOpponent;
    }

    private void OnWaitingForOpponent()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            statusLabel.Text = "Looking for players...";
        });
    }

    private void OnMatchFound(string gameId, string player1, string player2)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            SessionService.CurrentGameId = gameId;
            await Shell.Current.GoToAsync("//ShipPlacementPage");
        });
    }

    private async void butCancel_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        await SessionService.Hub.LeaveMatchmaking();
        await Shell.Current.GoToAsync("//MainPage");
    }
}