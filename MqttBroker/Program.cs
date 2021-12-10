using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System.Text;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Connecting;
using MQTTnet.Server;
using Serilog;
namespace MqttBroker
{
    class Program
    {
        static MqttClient mqttClient = null;
        static IMqttServer mqttServer = null;
        static IMqttClientOptions options = null;
        static bool runState = false;
        static bool running = false;
        static string ServerUrl = "127.0.0.1";
        static int port = 8080;
        static string passowrd = "111111";
        static string UserId = "admin";
        static string Topic = "PortalPlayer(TWN) Co, Ltd";
        static bool Retained = false;
        static int QualityIOfServiceLevel = 0;
        public static void Start()
        {
            try
            {
                runState = true;
                Thread thread = new Thread(Work);
                thread.IsBackground = true;
                thread.Start();
            } catch (Exception ex)
            {
                Console.WriteLine("There is a problem @client side " + ex.ToString());
            }
        }

        public static void OnNewConnection( MqttConnectionValidatorContext context)
        {

            Log.Logger.Information(
                "New connection: ClientId = {clientId}, Endpoint = {endpoint}",
                context.ClientId,
                context.Endpoint);
        }
        static int MessageCounter = 0;

        public static void OnNewMessage( MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null ? null : context.ApplicationMessage?.Payload;
            MessageCounter++;
            Log.Logger.Information(
                "MessageId: {MessasgeCounter} - TimeStamp: {TimeStamp} -- Message: ClientId = {clientid}, Topic={topic}, Payload={payload}, Qos = {qos}, Rtain-Flag={retainFlag}",
                MessageCounter,
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain
                );
        }
        private static void Work()
        {
            running = true;
            Console.WriteLine("Work >> Begin");
            try
            {
                var factory = new MqttFactory();
                options = new MqttClientOptionsBuilder()
                    .WithTcpServer(ServerUrl, port)
                    .WithCredentials(UserId, passowrd)
                    .WithClientId("XMan")
                    .Build();
                MqttServerOptionsBuilder _opts = new MqttServerOptionsBuilder()
                    .WithDefaultEndpoint()
                    .WithDefaultEndpointPort(8080)
                    .WithConnectionValidator(OnNewConnection)
                    .WithApplicationMessageInterceptor(OnNewMessage);
                mqttServer = factory.CreateMqttServer();
                mqttServer.StartAsync(_opts.Build()).GetAwaiter().GetResult();
                Console.ReadLine();

#if false
                mqttClient = factory.CreateMqttClient() as MqttClient;
                options = new MqttClientOptionsBuilder()
                    .WithTcpServer(ServerUrl, port)
                    .WithCredentials(UserId, passowrd)
                    .WithClientId("XMan")
                    .Build();
                mqttClient.ConnectAsync(options);

                mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(new Func<MqttClientConnectedEventArgs, Task>(Connected));
                mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(new Func<MqttClientDisconnectedEventArgs, Task>(Disconnected));
                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(new Action<MqttApplicationMessageReceivedEventArgs>(MqttApplicationMessageReceived));
                while (runState)
                {

                    Thread.Sleep(100);

                }
#endif
            }
            catch ( Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Work >> End");
            running = false;
            runState = false;
        }
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Console.WriteLine("Hello World!");
            Work();
        }
    }
}
