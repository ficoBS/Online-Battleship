namespace Online_Battleship.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    private void clearEntrys()
    {
        usernameEntry.Text = "";
        emailEntry.Text = "";
        passwordEntry1.Text = "";
        passwordEntry2.Text = "";
        birthPicker.Date = DateTime.Today;
    }
    private async void butRegister_Clicked(object sender, EventArgs e)
    {
        clearEntrys();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void butBack_Clicked(object sender, EventArgs e)
    {
        clearEntrys();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}