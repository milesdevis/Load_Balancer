using System;
using System.Windows.Forms;
using Eneter.Messaging.EndPoints.TypedMessages;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;
using Eneter.Messaging.MessagingSystems.TcpMessagingSystem;

namespace Client
{
    public partial class Form1 : Form
    {
        // Requested calculation range.
        public class Range
        {
            public double From;
            public double To;
        }

        public Form1()
        {
            InitializeComponent();
            OpenConnection();
        }

        public void OpenConnection()
        {
            // Create TCP messaging for the communication.
            // Note: Requests are sent to the balancer that will forward them
            //       to available services.
            IMessagingSystemFactory myMessaging = new TcpMessagingSystemFactory();
            IDuplexOutputChannel anOutputChannel = myMessaging.CreateDuplexOutputChannel("tcp://127.0.0.1:8060/");

            // Create sender to send requests.
            IDuplexTypedMessagesFactory aSenderFactory = new DuplexTypedMessagesFactory();
            mySender = aSenderFactory.CreateDuplexTypedMessageSender<double, Range>();

            // Subscribe to receive response messages.
            mySender.ResponseReceived += OnResponseReceived;

            // Attach the output channel and be able to send messages and receive responses.
            mySender.AttachDuplexOutputChannel(anOutputChannel);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Detach the output channel and stop listening for responses.
            mySender.DetachDuplexOutputChannel();
        }

        private void CalculatePiBtn_Click(object sender, EventArgs e)
        {
            myCalculatedPi = 0.0;

            // Split calculation of PI to 400 ranges and sends them for the calculation.
            for (double i = -1.0; i <= 1.0; i += 0.005)
            {
                Range anInterval = new Range() { From = i, To = i + 0.005 };
                mySender.SendRequestMessage(anInterval);
            }
        }

        private void OnResponseReceived(object sender, TypedResponseReceivedEventArgs<double> e)
        {
            // Receive responses (calculations for ranges) and calculate PI.
            myCalculatedPi += e.ResponseMessage;

            // Display the number.
            // Note: The UI control can be used only from the UI thread.
            InvokeInUIThread(() => ResultTextBox.Text = myCalculatedPi.ToString());
        }

        // Helper method to invoke some functionality in UI thread.
        private void InvokeInUIThread(Action uiMethod)
        {
            // If we are not in the UI thread then we must synchronize via the invoke mechanism.
            if (InvokeRequired)
            {
                Invoke(uiMethod);
            }
            else
            {
                uiMethod();
            }
        }

        private IDuplexTypedMessageSender<double, Range> mySender;
        private double myCalculatedPi;
    }
}
