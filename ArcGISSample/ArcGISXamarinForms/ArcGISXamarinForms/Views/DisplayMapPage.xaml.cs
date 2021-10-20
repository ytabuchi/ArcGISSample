using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace ArcGISXamarinForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMapPage : ContentPage
    {
        public DisplayMapPage()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            ArcGISRuntimeEnvironment.ApiKey = Secrets.ARCGIS_AKIKEY;

            // Create new Map with basemap.
            var myMap = new Map(BasemapStyle.ArcGISStreets);
            // Assign the map to the MapView.
            MyMapView.Map = myMap;
            MyMapView.Map.InitialViewpoint = new Viewpoint(
                latitude: 35.62873846060738,
                longitude: 139.73917311807074,
                scale: 5000);
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            MyMapView.LocationDisplay.IsEnabled = false;
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            var locationPermissionStatus = await CheckAndRequestLocationPermission();

            if (locationPermissionStatus != PermissionStatus.Granted)
            {
                // Permission が許可されていない
                return;
            }

            MyMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            await MyMapView.LocationDisplay.DataSource.StartAsync();
            MyMapView.LocationDisplay.IsEnabled = true;
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
    }
}