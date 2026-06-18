namespace Online_Battleship.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void clearEntrys()
    {
        emailEntry.Text = "";
        passwordEntry.Text = "";
    }

    private async void butCreateAccount_Clicked(object sender, EventArgs e)
    {
        clearEntrys();
        await Shell.Current.GoToAsync("//RegisterPage");
    }

    private async void butLogin_Clicked(object sender, EventArgs e)
    {
        clearEntrys();
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void butLeave_Clicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}