using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class PlayersPage : ContentPage
{
    public PlayersPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPlayers();
    }

    private async Task LoadPlayers()
    {
        var players = await SessionService.Api.GetPlayers();
        playersList.ItemsSource = players.Where(p => p.Id != SessionService.UserId).ToList();
    }

    private async void butBack_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnChallengeClicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        var btn = sender as Button;
        var player = btn?.BindingContext as PlayerDto;
        if (player == null) return;

        bool confirm = await DisplayAlert("Challenge", $"Challenge {player.Username}?", "Yes", "No");
        if (!confirm) return;

        await SessionService.Hub.ChallengePlayer(player.Id);
        await DisplayAlert("Challenge Sent", $"Challenge sent to {player.Username}", "OK");
    }
}