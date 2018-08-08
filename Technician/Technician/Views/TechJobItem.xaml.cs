using Xamarin.Forms;

namespace TechTechnician.Views
{
    public partial class TechJobItem : ContentPage
    {
        public TechJobItem()
        {
            InitializeComponent();
            /*ove().ConfigureAwait(true);*/
        }
        //private Task Move()
        //{
        //    return Task.Run(() =>
        //    {
        //        if (TechnicianModule.Latitude != null)
        //        {
        //            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position((double)TechnicianModule.Latitude, (double)TechnicianModule.Longitude), Distance.FromMiles(1)));
        //            map.Pins.Add(new Pin { Rotation = 33.3f, Address = "", Tag = "id_location", Label = "Selected location", Position = new Position((double)TechnicianModule.Latitude, (double)TechnicianModule.Longitude) });
        //        }
        //    });


        //}
    }
}
