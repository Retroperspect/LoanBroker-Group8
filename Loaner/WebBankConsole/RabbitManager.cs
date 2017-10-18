using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace WebBankConsole
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
                    channel.QueueDeclare(queue: "GoBankResponse", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: "GoBankResponse", basicProperties: basic, body: body);
                    Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


                }

            }
            public void receiveMessage()
            {
                var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "LoanRequestB2",
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
                        LoanRequest req = (LoanRequest)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(LoanRequest));

                        ServiceReference1.LoanRequest lq = new ServiceReference1.LoanRequest();
                        lq.ssnk__BackingField = req.ssn;
                        lq.LoanDurationk__BackingField = req.LoanDuration;
                        lq.LoanAmmountk__BackingField = req.LoanAmmount;
                        ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
                        ServiceReference1.UniversalResponse res = client.HandleRequest(lq);

                        UniversalResponse prettyres = new UniversalResponse() { interestrate = res.interestratek__BackingField, ssn = res.ssnk__BackingField };

                        var message = Serializer.SerializeObjectToXmlType(prettyres, prettyres.GetType());
                        sendEnriched(Encoding.UTF8.GetBytes(message), ea.BasicProperties);

                        ///// send anotehr message to another channel 
                        Console.WriteLine(" [x] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "LoanRequestB2",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        
    }
}
