using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace Determiner
{
    class RabbitManager
    {
        



            public void sendEnriched(byte[] body, IBasicProperties basic)
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Declares a que
                    channel.QueueDeclare(queue: "BestResponse", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: "BestResponse", basicProperties: basic, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "AllResponses",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicQos(0, 1, false);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;

                        Responses response = (Responses)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(Responses));
                        UniversalResponseFinal bestresponse = new UniversalResponseFinal() { bank = "none", interestrate = 20, ssn = "uknown" };
                        foreach (var item in response.MasterList)
                        {
                            if (item.interestrate < bestresponse.interestrate)
                            {
                                bestresponse = item;
                                Console.WriteLine("Was better");
                            }
                            else Console.WriteLine("Was not better");
                        }
                        var message = Serializer.SerializeObjectToJsonType(bestresponse);
                        sendEnriched(Encoding.UTF8.GetBytes(message), ea.BasicProperties);

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "AllResponses",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
