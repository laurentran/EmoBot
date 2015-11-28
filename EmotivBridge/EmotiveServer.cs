using Emo.Common;
using Emo.Data.DTO;
using EmotivController.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EmotivBridgeClassic
{
    public class EmotivServer
    {

        // Singleton Pattern here.  We want to give out the same instance of the 
        // server object to any client that calls
        private readonly static Lazy<EmotivServer> _instance = new Lazy<EmotivServer>(
            () => new EmotivServer(GlobalHost.ConnectionManager.GetHubContext<EmotivBridgeHub>().Clients)
        );

        // This is only used if in the future we want multiple controllers.  Not critical for just
        // one controller.
        private readonly ConcurrentDictionary<string, EmotivController.Data.EmotivController> _controllers =
            new ConcurrentDictionary<string, EmotivController.Data.EmotivController>();

        // Singleton pattern.
        public static EmotivServer Instance
        {
            get { return _instance.Value; }
        }

        private EmotivServer(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            var cont = new EmotivController.Data.EmotivController();
            if (!_controllers.TryAdd(cont.Name, cont))
            {
                Debug.Assert(false);
            }
            else
            {
                Debug.WriteLine("Controller {0} added to collection.", cont.Name);
            }
        }

        // This is where the work is done.  We get our controller from the list
        // (there is only one for now) and we broadcast it's emotion to the clients
        public void InitializeController()
        {
            var kvp = _controllers.First();
            var cont = kvp.Value;

            Debug.Assert(cont != null);

            cont.EmotionChanged += (s, e) =>
            {
                BroadcastEmotion(s as IEmotiv);
            };

            // The controller sends events from the Emotiv
            cont.StartListening();
        }

        public IHubConnectionContext<dynamic> Clients { get; set; }

        // This will broadcast our message to the clients.
        private void BroadcastEmotion( IEmotiv emo )
        {
            Clients.All.UpdateEmotion(emo);
        }
    }
}