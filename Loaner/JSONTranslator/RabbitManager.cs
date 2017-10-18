using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace JSONTranslator
{
    class RabbitManager
    {
        



            public void sendEnriched(byte[] body, string quename, string replyque, IBasicProperties basic)
            {
            ConnectionFactory factory;
            if (quename == "cphbusiness.bankJSON")
            {
                factory = new ConnectionFactory() { HostName = "datdb.cphbusiness.dk", UserName = "guest", Password = "guest" };
            }
            else { factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" }; }
            using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Declares a que
                    var Que = channel.QueueDeclare(queue: replyque, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    

                    /////
                    
                    basic.ReplyTo = Que.QueueName;

                IMapMessageBuilder b = new MapMessageBuilder(channel);

                    //Publish Message
                    channel.BasicPublish(exchange: quename, routingKey: "", basicProperties: basic, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "JSON",
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



                        var mes = (LoanRequest)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(LoanRequest));

                        
                        //////TRANSLATION COMENCE!!!
                        TimeSpan duration = TimeSpan.Parse(mes.LoanDuration);
                        double res = double.Parse(duration.Days.ToString()) / 30.436875;
                        int ress = (int)res;
                        string TranslatedDuration = ress.ToString();
                        TranslatedRequest Request = new TranslatedRequest() { ssn = mes.ssn.Replace("-", ""), creditScore = mes.CreditScore, loanAmount = (decimal)mes.LoanAmmount, loanDuration = TranslatedDuration };

                        
                        var message = Encoding.UTF8.GetBytes(Serializer.SerializeObjectToJsonType(Request));

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
                        sendEnriched(message, input, output, ea.BasicProperties);

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "JSON",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
