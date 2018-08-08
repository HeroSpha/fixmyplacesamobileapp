using System;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace Client.Views
{
    public partial class ClientLocationPage : ContentPage
    {
        
        public ClientLocationPage()
        {
            InitializeComponent();

           
           // map.UiSettings.MyLocationButtonEnabled = true;
            //var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
            //map.MoveToRegion(new MapSpan(map.VisibleRegion.Center, latlongdegrees, latlongdegrees));
           // map.MapClicked += Map_MapClicked;
            Move();
          
        }
        private async void Move()
        {
            
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(new TimeSpan(10000));
          //  map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(1)));
        }
       

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        //private  void Map_MapClicked(object sender, MapClickedEventArgs e)
        //{
        //    map.Pins.Clear();
        //    map.Pins.Add(new Pin { Rotation = 33.3f, Address = "", Tag="id_location", Label = "Selected location", Position = new Position(e.Point.Latitude, e.Point.Longitude) });
          
        //}
    }
}
