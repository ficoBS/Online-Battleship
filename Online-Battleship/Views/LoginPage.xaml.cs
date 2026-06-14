namespace Online_Battleship.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void butCreateAccount_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    private void butLogin_Clicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text;
        string password = passwordEntry.Text;
    }
}