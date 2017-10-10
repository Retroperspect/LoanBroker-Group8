using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipientList_Router
{
    class RabbitManager
    {
        



            public void sendEnriched(byte[] body, Bank banktosend, IBasicProperties basic)
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Declares a que
                    channel.QueueDeclare(queue: banktosend.format, durable: true, exclusive: false, autoDelete: false, arguments: null);


                    var properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.CorrelationId = basic.CorrelationId;
                    properties.ContentType = "Class of LoanRequest.";
                    properties.Headers["in"] = banktosend.Input;
                    properties.Headers["reply"] = banktosend.Output;
                    properties.Headers["Requests"] = Encoding.UTF8.GetString((byte[])basic.Headers["Requests"]);
                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: banktosend.format, basicProperties: properties, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RequestWithBanks",
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
                        


                        LoanRequestWithBanks FullRequest = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));

                        foreach (var bank in FullRequest.ViableBanks)
                        {

                            var message = Serializer.SerializeObjectToXml(new LoanRequest() { ssn = FullRequest.ssn, CreditScore = FullRequest.CreditScore, LoanAmmount = FullRequest.LoanAmmount, LoanDuration = FullRequest.LoanDuration});
                            sendEnriched(Encoding.UTF8.GetBytes(message), bank, ea.BasicProperties);

                        }

                        
                        

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "RequestWithBanks",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
