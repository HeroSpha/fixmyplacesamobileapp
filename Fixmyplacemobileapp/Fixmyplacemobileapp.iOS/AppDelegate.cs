
using Foundation;
using UIKit;
using Prism;
using Prism.Ioc;
using ImageCircle.Forms.Plugin.iOS;
using Acr.UserDialogs;
using ZXing.Mobile;
using Com.OneSignal;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Fixmyplacemobileapp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
       
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Plugin.Iconize.Iconize
              .With(new Plugin.Iconize.Fonts.MaterialModule())
              .With(new Plugin.Iconize.Fonts.IoniconsModule())
              .With(new Plugin.Iconize.Fonts.FontAwesomeModule());
            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            OneSignal.Current.StartInit("88b69634-8970-4f5d-92d4-69f696d697fd")
                  .EndInit();
           
            FormsPlugin.Iconize.iOS.IconControls.Init();
            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
           
        }
    }

}
