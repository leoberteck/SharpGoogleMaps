using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using SharpGoogleMaps.Data;

namespace SharpGoogleMaps
{
    public static class GoogleMapsApiHelper
    {
        public static IEnumerable<Marker> MarkerList { get { return _MarkerList; } }
        internal static List<Marker> _MarkerList { get; set; }

        static GoogleMapsApiHelper()
        {
            _MarkerList = new List<Marker>();
        }
    }

    public static class WebViewerExtensions
    {
        private const string HTML_URI = "ms-appx:///SharpGoogleMaps/HTML/index.html";

        public static async Task InitMapsWindow(this WebView instance, string apiKey)
        {
            var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(HTML_URI));
            instance.ScriptNotify += WebView_ScriptNotify;
            string line = "";
            using (StreamReader objReader = new StreamReader(await htmlFile.OpenStreamForReadAsync()))
            {
                line = await objReader.ReadToEndAsync();
            }
            line = line.Replace("API_KEY", apiKey);
            instance.NavigateToString(line);
        }

        public static async Task AddPin(this WebView instance, string directions, bool centerMap = false)
        {
            await instance.InvokeScriptAsync("addPin", new string[] { directions, centerMap.ToString() });
        }

        public static async Task RemovePin(this WebView instance, Marker _marker)
        {
            await instance.InvokeScriptAsync("removePin", new string[] { _marker.place_id });
            GoogleMapsApiHelper._MarkerList.Remove(_marker);
        }

        public static async Task RemoveAllPins(this WebView instance)
        {
            foreach(var marker in GoogleMapsApiHelper._MarkerList)
            {
                await RemovePin(instance, marker);
            }
        }

        /// <summary>
        /// Callback para a execução de métodos javascript asíncronos. Toda script executado pelo javascript DEVE retonar da seguinte maneira: 
        /// * Um objeto único com o protótipo : 
        /// {
        ///     sender : "nome da função de gerou o retorno",
        ///     parameter : "objeto que corresponda a classe apropriada do namespace SharpGoogleMaps.Data"
        /// }
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if(e.Value != "FAIL")
            {
                var response = await DeserializeObjectAsync<ScriptResponse>(e.Value);
                switch (response.sender)
                {
                    case "addPin":
                        var newMarker = await DeserializeObjectAsync<Marker>(response.parameter);
                        GoogleMapsApiHelper._MarkerList.Add(newMarker);
                        break;
                }
            }
        }

        private static async Task<T> DeserializeObjectAsync<T>(string json)
        {
            return await Task.Run(() => { return JsonConvert.DeserializeObject<T>(json); });
        }

        private class ScriptResponse
        {
            public string sender { get; set; }
            public string parameter { get; set; }
        }
    }
}
