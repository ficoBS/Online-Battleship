namespace Online_Battleship.Views;

public partial class MatchPage : ContentPage
{
	public MatchPage()
	{
		InitializeComponent();
	}

    private async void butCancel_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//MainPage");
    }
}