using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SharpGoogleMaps;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpGoogleMapsDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string API_KEY = "AIzaSyBl45lrDuN8U4Uosa1nJL6AxAQRb1ifVhQ";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await webMaps.InitMapsWindow(API_KEY);
            await Task.Delay(4000);
            await webMaps.AddPin("Rua Manoel Lourenço Baptista, 21", true);
            await Task.Delay(10000);
            await webMaps.RemovePin(GoogleMapsApiHelper.MarkerList.First());
        }
    }
}
