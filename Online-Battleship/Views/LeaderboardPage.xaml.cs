using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class LeaderboardPage : ContentPage
{
    public LeaderboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadLeaderboard();
    }

    private async Task LoadLeaderboard()
    {
        var players = await SessionService.Api.GetLeaderboard();
        leaderboardList.ItemsSource = players;
    }

    private async void butBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}