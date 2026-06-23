using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class MatchHistoryPage : ContentPage
{
    public MatchHistoryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHistory();
    }

    private async Task LoadHistory()
    {
        var history = await SessionService.Api.GetMatchHistory(SessionService.UserId);
        historyList.ItemsSource = history;
    }

    private async void butBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}