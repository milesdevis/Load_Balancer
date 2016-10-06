using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Eneter.Messaging.EndPoints.TypedMessages;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;
using Eneter.Messaging.MessagingSystems.TcpMessagingSystem;

namespace PiCalculator
{
    // Requested calculation range.
    public class Range
    {
        public double From;
        public double To;
    }

    class Program
    {
        static void Main(string[] args)
        {
            string aServiceAddress = GetMyAddress();
            if (aServiceAddress == "")
            {
                Console.WriteLine("The service could not start because all possible ports are occupied.");
                return;
            }

            // Create TCP messaging for receiving requests.
            IMessagingSystemFactory aMessaging = new TcpMessagingSystemFactory();
            IDuplexInputChannel anInputChannel = aMessaging.CreateDuplexInputChannel(aServiceAddress);

            // Create typed message receiver to receive requests.
            // It receives request messages of type Range and sends back
            // response messages of type double.
            IDuplexTypedMessagesFactory aReceiverFactory = new DuplexTypedMessagesFactory();
            IDuplexTypedMessageReceiver<double, Range> aReceiver
                = aReceiverFactory.CreateDuplexTypedMessageReceiver<double, Range>();

            // Subscribre to messages.
            aReceiver.MessageReceived += OnMessageReceived;

            // Attach the input channel and start listening.
            aReceiver.AttachDuplexInputChannel(anInputChannel);

            Console.WriteLine("Root Square Calculator listening to " + aServiceAddress +
                " is running.\r\n Press ENTER to stop.");
            Console.ReadLine();

            // Detach the input channel and stop listening.
            aReceiver.DetachDuplexInputChannel();
        }

        private static void OnMessageReceived(object sender, TypedRequestReceivedEventArgs<Range> e)
        {
            Console.WriteLine("Calculate From: {0} To: {1}", e.RequestMessage.From, e.RequestMessage.To);

            // Calculate requested range.
            double aResult = 0.0;
            double aDx = 0.000000001;
            for (double x = e.RequestMessage.From; x < e.RequestMessage.To; x += aDx)
            {
                aResult += 2 * Math.Sqrt(1 - x * x) * aDx;
            }

            // Response back the result.
            IDuplexTypedMessageReceiver<double, Range> aReceiver
                = (IDuplexTypedMessageReceiver<double, Range>)sender;
            aReceiver.SendResponseMessage(e.ResponseReceiverId, aResult);
        }

        // Helper method to get the address.
        // Note: Since our example services are running on the same machine,
        //       we must ensure they do not try to listen to the same IP address.
        private static string GetMyAddress()
        {
            List<int> aPossiblePorts = new List<int>(new int[]{ 8071, 8072, 8073 });

            // When we execute this service three time we want to get three instances
            // listening to different port.
            IPGlobalProperties anIpGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] aTcpListeners = anIpGlobalProperties.GetActiveTcpListeners();

            // Remove from the possible ports those which are used.
            foreach (IPEndPoint aListener in aTcpListeners)
            {
                aPossiblePorts.Remove(aListener.Port);
            }

            // Get the first available port.
            if (aPossiblePorts.Count > 0)
            {
                return "tcp://127.0.0.1:" + aPossiblePorts[0] + "/";
            }

            // All three instances are used.
            return "";
        }

    }
}
