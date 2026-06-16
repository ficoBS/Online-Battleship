namespace Online_Battleship.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void butLogout_Clicked(object sender, EventArgs e)
    {
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
}