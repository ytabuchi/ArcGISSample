using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Esri.ArcGISRuntime;

namespace ArcGISAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var gotoMapActivityButton = FindViewById<Button>(Resource.Id.goto_display_map_activity_button);
            gotoMapActivityButton.Click += GotoMapActivityButton_Click;
        }

        private void GotoMapActivityButton_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(DisplayMapActivity));
            StartActivity(intent);
        }
    }
}