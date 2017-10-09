using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator
{
    class RabbitManager
    {
        //Remake into sending a enriched body to the aggregator
        public void sendEnriched(byte[] body, string messagetoreturn)
        {
            var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Declares a que
                channel.QueueDeclare(queue: messagetoreturn, durable: true, exclusive: false, autoDelete: false, arguments: null);


                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object>();
                //properties.Headers["in"] = messagetoreturn.Input;
                //properties.Headers["reply"] = messagetoreturn.Output;
                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: messagetoreturn, basicProperties: properties, body: body);
                Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


            }

        }

        //Remake and add logic to figure out which bank send the message
        public void receiveMessage()
        {
            var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Normalizer-Send-Que",
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
                    DateTime past = DateTime.Now;
                    DateTime future = past.AddSeconds(10);

                    while (past < future)
                    {
                        //welp fix that spam
                        sendEnriched(body, "LoanOnStandby");

                        channel.BasicConsume(queue: "Normalizer-Send-Que", autoAck: false, consumer: consumer);
                    }
                    try
                    {
                        channel.QueueDeclare(queue: "Aggregator-Wait-Que",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                        consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (eodel, da) =>
                        {

                        };
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    LoanRequest XmlRequest = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));
                    //implement the correct format and implement logic for gathering and finding the best loan option
                    var messageXmlRequest = Serializer.SerializeObjectToXml(new LoanRequest() { ssn = XmlRequest.ssn, CreditScore = XmlRequest.CreditScore, LoanAmmount = XmlRequest.LoanAmmount, LoanDuration = XmlRequest.LoanDuration });
                    Console.WriteLine("received {0}", messageXmlRequest);
                    //
                    sendEnriched(Encoding.UTF8.GetBytes(messageXmlRequest), "LoanReturn");

                    ///// send anotehr message to another channel 
                    Console.WriteLine(" [x] Done");

                    //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Normalizer-Send-Que",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
