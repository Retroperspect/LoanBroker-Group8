﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace GetCreditScore
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
                    channel.QueueDeclare(queue: "RequestWithCredit", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: "RequestWithCredit", basicProperties: basic, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RequestLoan",
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
                        CreditScoreService.CreditScoreServiceClient client = new CreditScoreService.CreditScoreServiceClient();
                        NoCredits.CreditScore = client.creditScore(NoCredits.ssn);
                        Console.WriteLine(" [x] Received {0}");
                        var message = Serializer.SerializeObjectToXmlType(NoCredits, NoCredits.GetType());
                        
                        sendEnriched(Encoding.UTF8.GetBytes(message), ea.BasicProperties);

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "RequestLoan",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
