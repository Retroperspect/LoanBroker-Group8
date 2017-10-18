using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace Loaner
{
    public class RabbitManager
    {
        



            public void sendEnriched(byte[] body)
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Declares a que
                    channel.QueueDeclare(queue: "RequestLoan", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.CorrelationId = Guid.NewGuid().ToString();


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: "RequestLoan", basicProperties: properties, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RequestAPI",
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

                        apirequest req = (apirequest)Serializer.DeserializeObjectFromJsonType(Encoding.UTF8.GetString(body), typeof(apirequest));
                        LoanRequest FormatedRequest = new LoanRequest() { ssn = req.ssn, CreditScore = 0, LoanAmmount = req.amount, LoanDuration = TimeSpan.FromDays(req.term).ToString() };

                        var message = Serializer.SerializeObjectToXmlType(FormatedRequest, FormatedRequest.GetType());
                        
                        sendEnriched(Encoding.UTF8.GetBytes(message));

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "RequestAPI",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
