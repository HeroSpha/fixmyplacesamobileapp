using System;
using Prism;
using Prism.Ioc;

namespace Fixmyplacemobileapp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Fixmyplacemobileapp.App(new UwpInitializer()));
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }

}
