using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace EmotivBridgeClassic
{
    public class EmotivBridgeHub : Hub
    {
        private readonly EmotivServer _server;

        public EmotivBridgeHub() : this( EmotivServer.Instance )
        {

        }

        public EmotivBridgeHub( EmotivServer server )
        {
            _server = server;
        }
     
        // This will kick off our events   
        public void InitializeController()
        {
            _server.InitializeController();
        }
    }
}