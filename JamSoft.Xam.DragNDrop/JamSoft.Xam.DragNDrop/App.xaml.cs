using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using JamSoft.Xam.DragNDrop.Services;
using JamSoft.Xam.DragNDrop.Views;

namespace JamSoft.Xam.DragNDrop
{
    public partial class App : Application
    {
        public static int AppScreenWidth;

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
