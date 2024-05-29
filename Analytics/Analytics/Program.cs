using System;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Analytics
{
    internal class Program
    {
       
        static Dictionary<int,float> avgTemp = new Dictionary<int,float>();
        static Dictionary<int,float> avgHum = new Dictionary<int,float>();
        static Dictionary<int,float> count = new Dictionary<int,float>();
       
        public static async Task Handle_Received_Application_Message()
        {
            /*
             * This sample subscribes to a topic and processes the received message.
             */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost").Build();

                // Setup message handling before connecting so that queued messages
                // are also handled properly. When there is no event handler attached all
                // received messages get lost.
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    //read json attributes from message
                    var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    //deserialize json
                    var vrednostSenzora = JsonConvert.DeserializeObject<MerenjeDTO>(message);
                    Console.WriteLine("Received application message.");
                    Console.WriteLine(message);

                    if(!avgTemp.ContainsKey(vrednostSenzora.Device))
                    {
                        avgTemp.Add(vrednostSenzora.Device, vrednostSenzora.Temperature);
                        avgHum.Add(vrednostSenzora.Device, vrednostSenzora.Humidity);
                        count.Add(vrednostSenzora.Device, 1);
                    }
                    else
                    {
                        avgTemp[vrednostSenzora.Device] += vrednostSenzora.Temperature;
                        avgHum[vrednostSenzora.Device] += vrednostSenzora.Humidity;
                        count[vrednostSenzora.Device] += 1;
                    }

                    //send analytics on every 10th message
                    if (count[vrednostSenzora.Device] % 10 == 0)
                    {
                        

                        AnalyticsDTO analyticsDTO = new AnalyticsDTO
                        {
                            Date = DateTime.Now,
                            Time = DateTime.Now.TimeOfDay,
                            Device = vrednostSenzora.Device,
                            Battery = vrednostSenzora.Battery,
                            AverageHumidity = avgHum[vrednostSenzora.Device] / count[vrednostSenzora.Device] ,
                            AverageTemperature = avgTemp[vrednostSenzora.Device] / count[vrednostSenzora.Device] 
                        };

                        var mqttApplicationMessage = new MqttApplicationMessageBuilder()
                            .WithTopic("analytics")
                            .WithPayload(JsonConvert.SerializeObject(analyticsDTO))
                            .Build();

                        mqttClient.PublishAsync(mqttApplicationMessage, CancellationToken.None);
                        Console.WriteLine("Analytics sent.");

                    }

                    


                    String poruka = "";
                   
                    if (vrednostSenzora.Temperature < 20)
                        poruka = "Temperatura je ispod 20 stepeni/n";
                    if (vrednostSenzora.Humidity < 30)
                        poruka += "Vlaznost vazduha je ispod 30%./n";
                    if(vrednostSenzora.Battery < 1)
                        poruka += "Baterija je prazna.";

                    if(poruka != "")
                    { 
                        EventDTO eventDTO = new EventDTO
                        {

                            Date = vrednostSenzora.Date,
                            Time = vrednostSenzora.Time,
                            Device = vrednostSenzora.Device,
                            Battery = vrednostSenzora.Battery,
                            Humidity = vrednostSenzora.Humidity,
                            Temperature = vrednostSenzora.Temperature,
                            Message = poruka
                        };
                        var mqttApplicationMessage = new MqttApplicationMessageBuilder()
                            .WithTopic("event")
                            .WithPayload(JsonConvert.SerializeObject(eventDTO))
                            .Build();


                        mqttClient.PublishAsync(mqttApplicationMessage, CancellationToken.None);
                        Console.WriteLine("Event sent.");
                    }

                    return Task.CompletedTask;
                };

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic("merenje");
                        })
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Console.WriteLine("MQTT client subscribed to topic merenje.");

                Console.WriteLine("Press enter to exit.");

                Console.ReadLine();

                var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

                await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            }
        }

        static async Task SendAnalitycs()
        {

        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Analytics servise");
            await Handle_Received_Application_Message();
            
          
        }

       
    }
}
