using Online_Battleship.Services;

namespace Online_Battleship.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void butRegister_Clicked(object sender, EventArgs e)
    {
        string username = usernameEntry.Text?.Trim();
        string email = emailEntry.Text?.Trim();
        string password1 = passwordEntry1.Text;
        string password2 = passwordEntry2.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password1) || string.IsNullOrWhiteSpace(password2))
        {
            await DisplayAlert("Error", "Please fill in all fields", "OK");
            return;
        }

        if (password1 != password2)
        {
            await DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        butRegister.IsEnabled = false;
        butRegister.Text = "Registering...";

        var (success, message) = await SessionService.Api.Register(username, email, password1);

        if (!success)
        {
            await DisplayAlert("Error", message, "OK");
            butRegister.IsEnabled = true;
            butRegister.Text = "Register";
            return;
        }

        await DisplayAlert("Success", "Account created! Please login.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void butBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}