namespace Online_Battleship.Services
{
    public class SoundService
    {
        private const double volume = 0.1;
        public static async Task PlayClickAsync()
        {
            var player = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(
                await FileSystem.OpenAppPackageFileAsync("click.mp3")
            );
            player.Volume = 0.06;
            player.Play();
        }
        public static async Task PlayHitAsync()
        {
            var player = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(
                await FileSystem.OpenAppPackageFileAsync("explode.mp3")
            );
            player.Volume = volume;
            player.Play();
        }
        public static async Task PlayMissAsync()
        {
            var player = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(
                await FileSystem.OpenAppPackageFileAsync("waterdrop.mp3")
            );
            player.Volume = volume;
            player.Play();
        }
    }
}