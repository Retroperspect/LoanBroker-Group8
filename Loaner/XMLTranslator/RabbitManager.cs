using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLTranslator
{
    class RabbitManager
    {
        



            public void sendEnriched(byte[] body, string quename, string replyque)
            {
                var factory = new ConnectionFactory() { HostName = "datdb.cphbusiness.dk", UserName = "guest", Password = "guest" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Declares a que
                    var Que = channel.QueueDeclare(queue: "Group8-LoanBroker-Request", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    

                    /////
                    var correlationId = Guid.NewGuid().ToString();
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.ReplyTo = Que.QueueName;
                    properties.CorrelationId = correlationId;
                    
                    IMapMessageBuilder b = new MapMessageBuilder(channel);

                    //Publish Message
                    channel.BasicPublish(exchange: quename, routingKey: "", basicProperties: properties, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "XML",
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
                        var header = ea.BasicProperties.Headers;



                        // 


                        var mes = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));


                        //////TRANSLATION COMENCE!!!
                        TimeSpan duration = TimeSpan.Parse(mes.LoanDuration);
                        DateTime UnixZero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        DateTime Duration = UnixZero.Add(duration);
                        string TranslatedDuration = Duration.ToString("yyyy-MM-dd H:mm:ss CET");
                        TranslatedRequest Request = new TranslatedRequest() { ssn = mes.ssn.Replace("-", ""), creditScore = mes.creditScore, LoanAmmount = (float)mes.LoanAmmount, LoanDuration = TranslatedDuration };

                        
                        var message = Encoding.UTF8.GetBytes( Serializer.SerializeObjectToXml(Request) );

                        string input;
                        string output;
                        try
                        {

                            input = Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["in"]);
                            output = Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["reply"]);
                        }
                        catch (Exception e)
                        {
                            input = "InvalidBank";
                            output = "InvalidBank";
                        }

                        /// send translated message with destination and reply destination
                        sendEnriched(message, input, output);

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "XML",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
