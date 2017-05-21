using System;
using SharpOSC;
using System.Drawing;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Forms;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using System.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;
using System.Messaging;
using System.Runtime.InteropServices;

namespace muse_osc_server
{
    class MainClass
    {
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Send a series of key presses to the Calculator application.
        private void button1_Click(object sender, EventArgs e)
        {
            // Make Calculator the foreground application and send it 
            // a set of calculations.
        }

        public static void Main(string[] args)
        {
            // Get a handle to the Calculator application. The window class
            // and window name were obtained using the Spy++ tool.
            IntPtr calculatorHandle = FindWindow(null, "Calculator");

            // Verify that Calculator is a running process.
            if (calculatorHandle == IntPtr.Zero)
            {
                MessageBox.Show("Calculator is not running.");
                return;
            }
            Button button1 = new Button();
            int i = 0;
            //string filePath = @"c:\temp\test.txt";
            // Callback function for received OSC messages. 
            // Prints EEG and Relative Alpha data only.
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                var addr = messageReceived.Address;
                if (addr == "/muse/elements/blink")
                {
                    foreach (var arg in messageReceived.Arguments)
                    {
                        if (Convert.ToInt32(arg) == 1)
                        {
                            Console.WriteLine("counter: " + i);
                            Console.WriteLine("1 Blink Detected");

                            if (i <= 6 && i >= 2)
                            {
                                Console.Write("Double Blink Detected");
                                SetForegroundWindow(calculatorHandle);
                                SendKeys.SendWait("111");
                                SendKeys.SendWait("*");
                                SendKeys.SendWait("11");
                                SendKeys.SendWait("=");
                            }
                            else
                            {
                                Console.Write("Double Blink Not Deteced");
                            }
                            i = 0;
                            Console.WriteLine("Stopwatch Reset");
                        }
                        else
                        {
                            i = i + 1;
                            Console.WriteLine("Not Blink: " + i);
                        }
                    }
                }
            };

            // Create an OSC server.
            var listener = new UDPListener(5000, callback);

            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
            listener.Close();
        }

    }
}