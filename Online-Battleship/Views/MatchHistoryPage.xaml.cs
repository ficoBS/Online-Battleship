namespace Online_Battleship.Views;

public partial class MatchHistoryPage : ContentPage
{
	public MatchHistoryPage()
	{
		InitializeComponent();
	}

    private async void butBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//MainPage");
    }
}