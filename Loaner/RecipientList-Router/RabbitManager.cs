using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

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


                    if (banktosend.Bname != "None")
                    {
                    basic.Headers["in"] = banktosend.Input;
                    basic.Headers["reply"] = banktosend.Output;
                    basic.Headers["bname"] = banktosend.Bname;
                    }
                    

                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: banktosend.format, basicProperties: basic, body: body);
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
                        var header = ea.BasicProperties.Headers;

                        //// If header requets is 0 = send no response to final result.


                        LoanRequestWithBanks FullRequest =  (LoanRequestWithBanks)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(LoanRequestWithBanks));

                        if (FullRequest.ViableBanks.Count == 0)
                        {
                            var message = Encoding.UTF8.GetBytes("CreditScore is to low, no banks will take the loan request. Your CreditScore is: "+FullRequest.CreditScore);
                            var properties = channel.CreateBasicProperties();
                            properties.Persistent = true;
                            properties.CorrelationId = ea.BasicProperties.CorrelationId;
                            sendEnriched(message, new Bank() { format = "BestResponse", Bname = "None", Input = "None", Output = "None" }, properties);
                        }
                        foreach (var bank in FullRequest.ViableBanks)
                        {

                            var message = Serializer.SerializeObjectToXmlType(new LoanRequest() { ssn = FullRequest.ssn, CreditScore = FullRequest.CreditScore, LoanAmmount = FullRequest.LoanAmmount, LoanDuration = FullRequest.LoanDuration }, typeof(LoanRequest));
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
