using System;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;
using Eneter.Messaging.MessagingSystems.TcpMessagingSystem;
using Eneter.Messaging.Nodes.LoadBalancer;

namespace LoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create TCP messaging for the communication with the client
            // and with services performing requests.
            IMessagingSystemFactory aMessaging = new TcpMessagingSystemFactory();

            // Create load balancer.
          
            ILoadBalancerFactory aLoadBalancerFactory = new RoundRobinBalancerFactory(aMessaging);
            ILoadBalancer aLoadBalancer = aLoadBalancerFactory.CreateLoadBalancer();

            // Addresses of available services.
            string[] anAvailableServices = {
                       "tcp://127.0.0.1:8071/", "tcp://127.0.0.1:8072/", "tcp://127.0.0.1:8073/" };

            // Add IP addresses of services to the load balancer.
            foreach (string anIpAddress in anAvailableServices)
            {
                aLoadBalancer.AddDuplexOutputChannel(anIpAddress);
            }

            // Create input channel that will listen to requests from clients.
            IDuplexInputChannel anInputChannel = aMessaging.CreateDuplexInputChannel("tcp://127.0.0.1:8060/");

            // Attach the input channel to the load balancer and start listening.
            aLoadBalancer.AttachDuplexInputChannel(anInputChannel);

            Console.WriteLine("Load Balancer is running.\r\nPress ENTER to stop.");
            Console.ReadLine();

            // Stop lisening.
            aLoadBalancer.DetachDuplexInputChannel();
        }
    }
}
