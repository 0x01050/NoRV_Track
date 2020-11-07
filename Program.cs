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
        private static DateTime prevTime = DateTime.Now.AddSeconds(-100);
        public static bool send_finished = true;

        public static void Broadcast(byte[] bytes)
        {
            try
            {
                TimeSpan elapsed = DateTime.Now - prevTime;
                if ((!send_finished && elapsed.TotalSeconds > 5) || _wssv.WebSocketServices.SessionCount > 5)
                {
                    Application.Restart();
                }
                if (send_finished)
                {
                    send_finished = false;
                    prevTime = DateTime.Now;
                    if (_wssv != null && _wssv.IsListening)
                    {
                        _wssv.WebSocketServices.BroadcastAsync(bytes, new Action(() =>
                        {
                            send_finished = true;
                        }));
                    }
                }
                else
                {
                    Console.WriteLine("\n" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " : Previous Broadcast Not Finished\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " : Error happened on Broadcast");
                Console.WriteLine(e.Message);
                Console.WriteLine("---------------------------");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("###########################\n");
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _wssv = new WebSocketServer(IPAddress.Any, 4649);
            _wssv.Log.Level = LogLevel.Trace;
            _wssv.KeepClean = true;
            _wssv.AddWebSocketService<WSServer>("/");
            _wssv.Start();

            Application.Run(new TrackForm());

            _wssv.Stop();
        }
    }
}
