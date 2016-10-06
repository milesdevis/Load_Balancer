# Load_Balancer
A Load balancer implementation using C# to calculate `π` 

To run the example bellow you must execute applications in the following order:
- PiCalculator up to 3 instances.
- LoadBalancer
- Client

## Client
The client is a simple application containing a button sending the request to the service to calculate `π`  number. 
The calculation is split into many small intervals. Therefore, the client sends about 400 messages each containing a small interval for the calculation. Then it collects all responses and summarizes them together to get the final result, the PI number.

## Load Balancer 
The load balancer is a simple console application acting as a real service. In this example it has a farm of three services. 
(To keep things simple, all three services run on the same computer. They listens to different ports.) When a user presses the button in the client application, the load balancer receives about 400 request messages and distributes them to its three services from the farm.
To decide which service is picked from the farm, the load balancer uses Round-Robin algorithm. Therefore, all services are chosen evenly. (The Round-Robin algorithm is suitable for load balancing if processing of requests takes approximately same time.) 

## Pi Calculator

The calculator is a simple service (console application). It receives requests to perform calculations for specific intervals. 
When the calculation is done, it sends back the calculation result.

