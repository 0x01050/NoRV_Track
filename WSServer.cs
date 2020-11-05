using Accord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VCD_Demo
{
    class WSServer : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
        }

        protected override void OnClose(CloseEventArgs e)
        {
        }
    }
}
