
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Text;

namespace EventInfo
{
    public class Program
    {

        


        public static async Task Handle_Received_Application_Message()
        {
           

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("mqttumrezi").Build();

                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    //read json attributes from message
                    var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    if(e.ApplicationMessage.Topic == "event")
                    {
                        Console.WriteLine("\nReceived event message.\n");
                        Console.WriteLine(message);
                        //deserialize json
                        var Event = JsonConvert.DeserializeObject<EventDTO>(message);
                        EventInfoHistory.Instance.AddEvent(Event);
                        Console.WriteLine("\nEvent added to history.\n\n");
                    }
                    else if(e.ApplicationMessage.Topic == "analytics")
                    {
                        Console.WriteLine("\nReceived analytics message.\n");
                        Console.WriteLine(message);
                        //deserialize json
                        AnalyticsStore.Instance.AddAnalytics(JsonConvert.DeserializeObject<AnalyticsDTO>(message));
                        Console.WriteLine("\nAnalytics added to store.\n\n");
                    }
                   

                    return Task.CompletedTask;
                };

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);


                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic("event");
                            
                        })
                    .WithTopicFilter(
                        f =>
                        {
                            
                            f.WithTopic("analytics");
                        })
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Console.WriteLine("MQTT client subscribed to topic.");


                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }
        static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            var mqttFactory = new MqttFactory();
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

                mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            }
        }
        public static void Main(string[] args)
        {

            Handle_Received_Application_Message();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
