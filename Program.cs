using System;
using System.Net;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VCD_Demo
{
    static class Program
    {
        private static WebSocketServer _wssv;
        public static void Broadcast(byte[] bytes)
        {
            _wssv.WebSocketServices.Broadcast(bytes);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _wssv = new WebSocketServer(IPAddress.Any ,4649);
            _wssv.Log.Level = LogLevel.Trace;
            _wssv.AddWebSocketService<WSServer>("/");
            _wssv.Start();

            Application.Run(new TrackForm());

            _wssv.Stop();
        }
    }
}
