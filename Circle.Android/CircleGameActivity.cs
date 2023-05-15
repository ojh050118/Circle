#nullable disable

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using osu.Framework.Android;
using Debug = System.Diagnostics.Debug;

namespace Circle.Android
{
    [Activity(ConfigurationChanges = DEFAULT_CONFIG_CHANGES, Exported = true, LaunchMode = DEFAULT_LAUNCH_MODE, MainLauncher = true, ScreenOrientation = ScreenOrientation.FullUser)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault }, DataScheme = "content", DataPathPattern = ".*\\\\.circlez", DataHost = "*", DataMimeType = "*/*")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault }, DataSchemes = new[] { "circle" })]
    public class CircleGameActivity : AndroidGameActivity
    {
        protected override osu.Framework.Game CreateGame() => new CircleGameAndroid();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Debug.Assert(Window != null);

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}
