using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace ArcGISAndroid
{
    /// <summary>
    /// Map 表示、Geolocation 取得などのアクティビティです。
    /// </summary>
    /// <see href="https://docs.microsoft.com/ja-jp/xamarin/essentials/geolocation?tabs=android">Xamarin.Essentials Geolocation</see>
    /// <see href="https://docs.microsoft.com/ja-jp/xamarin/essentials/permissions?tabs=android">Xamarin.Essentials Permission</see>
    /// <see cref="https://github.com/Esri/arcgis-runtime-samples-dotnet/blob/main/src/Android/Xamarin.Android/Samples/Location/DisplayDeviceLocation/DisplayDeviceLocation.cs">Esri DeviceLocation Code</see>
    [Activity(Label = "DisplayMapActivity")]
    public class DisplayMapActivity : AppCompatActivity
    {
        // Hold a reference to the map view
        private MapView _myMapView;
        private Button _locationStartButton;
        private Button _locationStopButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Xamarin.Essentials の初期化
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // ArcGIS API Key 指定
            ArcGISRuntimeEnvironment.ApiKey = Secrets.ARCGIS_AKIKEY;

            // Create your application here
            CreateLayout();
            Initialize();
        }

        private void Initialize()
        {
            // Create new Map with basemap and an initial location
            _myMapView.Map = new Map(BasemapStyle.ArcGISStreets);
            _myMapView.Map.InitialViewpoint = new Viewpoint(
                latitude: 35.62873846060738,
                longitude: 139.73917311807074,
                scale: 5000);

            _myMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
        }

        private void CreateLayout()
        {
            /* コードで作成する場合
            // Create a new vertical layout for the app
            LinearLayout layout = new LinearLayout(this) { Orientation = Orientation.Vertical };

            // Add the map view to the layout
            _myMapView = new MapView(this);
            layout.AddView(_myMapView);

            // Show the layout in the app
            SetContentView(layout);
            */

            // /* レイアウトファイルを使う場合
            SetContentView(Resource.Layout.activity_display_map);

            _myMapView = FindViewById<MapView>(Resource.Id.myMapView);
            _locationStartButton = FindViewById<Button>(Resource.Id.locationStartButton);
            _locationStartButton.Click += _locationStartButton_Click;
            _locationStopButton = FindViewById<Button>(Resource.Id.locationStopButton);
            _locationStopButton.Click += _locationStopButton_Click;
        }

        private async void _locationStartButton_Click(object sender, EventArgs e)
        {
            _myMapView.LocationDisplay.IsEnabled = false;

            var locationPermissionStatus = await CheckAndRequestLocationPermission();

            if (locationPermissionStatus != PermissionStatus.Granted)
            {
                // Permission が許可されていない
                return;
            }
            
            await _myMapView.LocationDisplay.DataSource.StartAsync();
            _myMapView.LocationDisplay.IsEnabled = true;
        }

        private async void _locationStopButton_Click(object sender, EventArgs e)
        {
            _myMapView.LocationDisplay.IsEnabled = false;
            await _myMapView?.LocationDisplay?.DataSource?.StopAsync();
        }

        /// <summary>
        /// Xamarin.Essentials で Location の Permission を確認、取得するメソッド
        /// </summary>
        /// <returns></returns>
        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        /// <summary>
        /// Xamarin.Essentials でパーミッションリクエストの結果を受け取るメソッド
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}