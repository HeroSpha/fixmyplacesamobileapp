
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Acr.UserDialogs;
using FormsPlugin.Iconize.Droid;
using ZXing.Mobile;
using Plugin.Permissions;
using Prism;
using Prism.Ioc;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.CurrentActivity;

namespace Fixmyplacemobileapp.Droid
{
    [Activity(Label = "Kagiso House", Theme = "@style/MyTheme.Base", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait,  MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    //[Android(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
      
        protected override void OnCreate(Bundle bundle)
        {
            Plugin.Iconize.Iconize
             .With(new Plugin.Iconize.Fonts.IoniconsModule())
             .With(new Plugin.Iconize.Fonts.FontAwesomeModule())
             .With(new Plugin.Iconize.Fonts.MaterialModule());
            IconControls.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);
            TabLayoutResource = Resource.Layout.tabs;
            ToolbarResource = Resource.Layout.toolbar;
            UserDialogs.Init(this);
            CrossCurrentActivity.Current.Init(this, bundle);

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MyTheme_Base);
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
          //  CachedImageRenderer.Init(true);
           ImageCircleRenderer.Init();

            MobileBarcodeScanner.Initialize(Application);

            LoadApplication(new App(new AndroidInitializer()));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
      
    }


    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}

