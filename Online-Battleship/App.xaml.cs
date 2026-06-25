using Microsoft.Extensions.DependencyInjection;
using Online_Battleship.Services;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
#endif

namespace Online_Battleship
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SessionService.Hub.OnChallengeReceived += OnChallengeReceived;
            SessionService.Hub.OnChallengeRejected += OnChallengeRejected;
            SessionService.Hub.OnMatchFound += OnMatchFound;
            SessionService.Hub.OnBothPlayersReady += OnBothPlayersReady;
        }

        private void OnMatchFound(string gameId, string player1, string player2, string id1, string id2)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SessionService.CurrentGameId = gameId;
                if (player1 == SessionService.Username)
                {
                    SessionService.OpponentUsername = player2;
                    SessionService.OpponentId = int.Parse(id2);
                }
                else
                {
                    SessionService.OpponentUsername = player1;
                    SessionService.OpponentId = int.Parse(id1);
                }
                await Shell.Current.GoToAsync("//ShipPlacementPage");
            });
        }

        private void OnChallengeReceived(int challengerId, string username)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool accept = await Current.Windows[0].Page.DisplayAlert(
                    "Challenge", $"{username} challenged you!", "Accept", "Reject");
                if (accept)
                    await SessionService.Hub.AcceptChallenge(challengerId);
                else
                    await SessionService.Hub.RejectChallenge(challengerId);
            });
        }

        private void OnChallengeRejected()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Current.Windows[0].Page.DisplayAlert(
                    "Challenge", "Your challenge was rejected", "OK");
            });
        }
        private void OnBothPlayersReady(bool isFirst)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SessionService.IsMyTurn = isFirst;
                await Shell.Current.GoToAsync("//GamePage");
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

#if WINDOWS
            window.HandlerChanged += (s, e) =>
            {
                if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window nativeWindow)
                    return;

                var hwnd = WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            };
#endif

            return window;
        }
    }
}