namespace Online_Battleship.Views;

public partial class PlayersPage : ContentPage
{
	public PlayersPage()
	{
		InitializeComponent();
	}

    private async void butBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//MainPage");
    }
}