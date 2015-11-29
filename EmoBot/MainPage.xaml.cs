using Emo.Common;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EmoBot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void TestEmoConnection()
        {
            // Establish a connection to our Hub.  The Hub is created in EmotivBridgeClassic
            var hubConnection = new HubConnection("http://localhost:16566/");

            // Get a proxy to our hub instance.
            IHubProxy emotiveProxy = hubConnection.CreateHubProxy("EmotivBridgeHub");

            // Wire up the UpdateEmotion event.  When the server fires this event, it will call this code.
            emotiveProxy.On("UpdateEmotion",
                emo =>
                {
                    // Not the best way to parse this data.  JObject is from JSON.NET
                    // http://www.newtonsoft.com/json
                    var data = emo as JObject;
                    Debug.WriteLine(String.Format("My new emotion is {0} with magnitude {1} ",
                        Enum.Parse(typeof(EmotivEmotion),
                        data.SelectToken("Emotion").ToString()).ToString(),
                        data.SelectToken("Magnitude").ToString()).ToString()
                        );
                });

            // Start the controller and kick off the listening
            // Find InitializeController in EmotivServer
            await hubConnection.Start();
            await emotiveProxy.Invoke("InitializeController");
        }

        private void EmotivBridgeTestButton_Click(object sender, RoutedEventArgs e)
        {
            TestEmoConnection();
        }
    }
}
