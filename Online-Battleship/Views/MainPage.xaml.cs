using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!string.IsNullOrEmpty(SessionService.Username))
            welcomeLabel.Text = $"Welcome, {SessionService.Username}!";
    }

    private async void butLogout_Clicked(object sender, EventArgs e)
    {
        await SessionService.Api.Logout(SessionService.UserId);
        await SessionService.Hub.Disconnect();
        SessionService.Clear();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void butFindMatch_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MatchPage");
    }

    private void butLeave_Clicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private async void butPlayers_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PlayersPage");
    }

    private async void butLeaderboard_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LeaderboardPage");
    }

    private async void butHistory_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MatchHistoryPage");
    }
}