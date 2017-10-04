using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
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
            var factory = new ConnectionFactory() { HostName = "datdb.cphbusiness.dk", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Group8-LoanBroker-Request",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(0, 1, false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var header = ea.BasicProperties.Headers;
                    var body = ea.Body;

                    //this check which bank that the Normalizer received and direct the message to the correct Deserializer
                    try
                    {
                        switch (Encoding.UTF8.GetString((byte[])header["bank"]))
                        {
                            case "xml":
                                
                                LoanRequest XmlRequest = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));
                                var messageXmlRequest = Serializer.SerializeObjectToXml(new LoanRequest() { ssn = XmlRequest.ssn, CreditScore = XmlRequest.CreditScore, LoanAmmount = XmlRequest.LoanAmmount, LoanDuration = XmlRequest.LoanDuration });
                                Console.WriteLine("received {0}", messageXmlRequest);
                                sendEnriched(Encoding.UTF8.GetBytes(messageXmlRequest), "LoanReturn");
                                break;
                            case "school-bank-xml":
                                LoanRequest SchoolBankXmlRequest = Serializer.DeserializeObjectFromXmlSchoolBank(Encoding.UTF8.GetString(body));
                                var messageSchoolBankXmlRequest = Serializer.SerializeObjectToXml( ,new LoanRequest() { ssn = SchoolBankXmlRequest.ssn, CreditScore = SchoolBankXmlRequest.CreditScore, LoanAmmount = SchoolBankXmlRequest.LoanAmmount, LoanDuration = SchoolBankXmlRequest.LoanDuration });
                                Console.WriteLine("received {0}", messageSchoolBankXmlRequest);
                                sendEnriched(Encoding.UTF8.GetBytes(messageSchoolBankXmlRequest), "LoanReturn");
                                break;
                            case "school-bank-json":
                                LoanRequest SchoolBankJsonRequest = Serializer.DeserializeObjectFromJsonSchoolBank(Encoding.UTF8.GetString(body));
                                var messageSchoolBankJsonRequest = Serializer.SerializeObjectToXml(new LoanRequest() { ssn = SchoolBankJsonRequest.ssn, CreditScore = SchoolBankJsonRequest.CreditScore, LoanAmmount = SchoolBankJsonRequest.LoanAmmount, LoanDuration = SchoolBankJsonRequest.LoanDuration });
                                Console.WriteLine("received {0}", messageSchoolBankJsonRequest);
                                sendEnriched(Encoding.UTF8.GetBytes(messageSchoolBankJsonRequest), "LoanReturn");
                                break;
                            case "student-bank-xml":
                                LoanRequest StudentBankXmlRequest = Serializer.DeserializeObjectFromXmlStudentBank(Encoding.UTF8.GetString(body));
                                var messageStudentBankXmlRequest = Serializer.SerializeObjectToXml(new LoanRequest() { ssn = StudentBankXmlRequest.ssn, CreditScore = StudentBankXmlRequest.CreditScore, LoanAmmount = StudentBankXmlRequest.LoanAmmount, LoanDuration = StudentBankXmlRequest.LoanDuration });
                                Console.WriteLine("received {0}", messageStudentBankXmlRequest);
                                sendEnriched(Encoding.UTF8.GetBytes(messageStudentBankXmlRequest), "LoanReturn");
                                break;
                            case "student-bank-json":
                                LoanRequest StudentBankJsonRequest = Serializer.DeserializeObjectFromJsonStudentBank(Encoding.UTF8.GetString(body));
                                var messageStudentBankJsonRequest = Serializer.SerializeObjectToXml(new LoanRequest() { ssn = StudentBankJsonRequest.ssn, CreditScore = StudentBankJsonRequest.CreditScore, LoanAmmount = StudentBankJsonRequest.LoanAmmount, LoanDuration = StudentBankJsonRequest.LoanDuration });
                                Console.WriteLine("received {0}", messageStudentBankJsonRequest);
                                sendEnriched(Encoding.UTF8.GetBytes(messageStudentBankJsonRequest), "LoanReturn");
                                break;
                            default:
                                Console.WriteLine("no bank was specified in the deserialise message");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ain't no Object in that channel with [bank] as a header error was {0}", e);
                        //throw;
                    }

                    ///// send anotehr message to another channel 
                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Group8-LoanBroker-Request",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
