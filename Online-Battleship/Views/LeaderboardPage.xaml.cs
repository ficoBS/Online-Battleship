namespace Online_Battleship.Views;

public partial class LeaderboardPage : ContentPage
{
	public LeaderboardPage()
	{
		InitializeComponent();
	}

    private async void butBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//MainPage");
    }
}