﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace GetBanks
{
    class RabbitManager
    {
        



            public void sendEnriched(byte[] body, IBasicProperties basic, int BanksAmount)
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Declares a que
                    channel.QueueDeclare(queue: "RequestWithBanks", durable: true, exclusive: false, autoDelete: false, arguments: null);


                    
                    basic.Headers = new Dictionary<string, object>();
                    basic.Headers["Requests"] = BanksAmount.ToString();
                    

                    //Publish Message
                    channel.BasicPublish(exchange: "", routingKey: "RequestWithBanks", basicProperties: basic, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RequestWithCredit",
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


                        LoanRequest NoCredits = (LoanRequest)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(LoanRequest));
                        ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
                        ServiceReference1.Banks[] _ViableBanks = client.GetBanks(NoCredits.CreditScore);
                        LoanRequestWithBanks LoanWithBanks = new LoanRequestWithBanks() { ssn = NoCredits.ssn, LoanDuration = NoCredits.LoanDuration, CreditScore = NoCredits.CreditScore, LoanAmmount = NoCredits.LoanAmmount, ViableBanks = new List<Bank>() };

                        foreach (var item in _ViableBanks)
                        {
                            LoanWithBanks.ViableBanks.Add(new Bank() { format = item.format, Input = item.Input, Output=item.Output, Bname = item.Bname });
                        }
                        

                        
                        Console.WriteLine(" [x] Received {0}");


                        var message = Serializer.SerializeObjectToXmlType(LoanWithBanks, LoanWithBanks.GetType());
                        sendEnriched(Encoding.UTF8.GetBytes(message), ea.BasicProperties, _ViableBanks.Count());

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "RequestWithCredit",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
