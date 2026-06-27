using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void clearEntry()
    {
        emailEntry.Text = "";
        passwordEntry.Text = "";
    }

    private async void butLogin_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        string email = emailEntry.Text?.Trim();
        string password = passwordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please fill in all fields", "OK");
            return;
        }

        butLogin.IsEnabled = false;
        butLogin.Text = "Logging in...";

        var (success, message, userId, username) = await SessionService.Api.Login(email, password);

        if (!success)
        {
            string errorMsg = string.IsNullOrEmpty(message) ? "Account is already logged in" : message;
            await DisplayAlert("Error", message, "OK");
            butLogin.IsEnabled = true;
            butLogin.Text = "Login";
            return;
        }

        SessionService.UserId = userId;
        SessionService.Username = username;

        await SessionService.Hub.Connect(userId);

        await Shell.Current.GoToAsync("//MainPage");

        butLogin.IsEnabled = true;
        butLogin.Text = "Login";
        clearEntry();
    }

    private async void butCreateAccount_Clicked(object sender, EventArgs e)
    {
        await SoundService.PlayClickAsync();

        await Shell.Current.GoToAsync("//RegisterPage");
        clearEntry();
    }

    private void butLeave_Clicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}