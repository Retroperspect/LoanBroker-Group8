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
        //list of currently handled loans
        public List<LoanRequest> listOfLoans { get; set; }

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

                //first recieve the first message
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var header = ea.BasicProperties.Headers;
                    DateTime past = DateTime.Now;
                    DateTime future = past.AddSeconds(10);
                    LoanRequest request = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));
                    request.orderNumber = Encoding.UTF8.GetString((byte[])header["Order"]);
                    //then handle that message by consuming it and begin looking for new ones
                    while (past < future)
                    {
                        if (!listOfLoans.Contains(request))
                        {
                            //add request for later use
                            listOfLoans.Add(request);
                            //consume message to make room for new messages
                            channel.BasicConsume(queue: "Normalizer-Send-Que", autoAck: false, consumer: consumer);
                        }
                        else
                        {

                        }
                        past = DateTime.Now;
                    }
                    try
                    {
                        LoanRequest bestLoan;
                        foreach (LoanRequest item in listOfLoans)
                        {
                            if (item)
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    ///// send another message to another channel 
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
