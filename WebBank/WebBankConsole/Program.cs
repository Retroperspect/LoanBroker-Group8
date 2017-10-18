using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WebBankConsole
{
    class Program
    {
        public static WebBankService.Service1Client client;
        static void Main(string[] args)
        {
            client = new WebBankService.Service1Client();
            receiveMessage();
            /// This is the translator, no?. This Console Application should listen to a RabbitMQ channel somewhere more specificly "LoanRequestB2".
            /// As from the GetBanks Service: format = "XML", Input = "LoanRequestB2", Output = "Group8-LoanBroker-Request" }) you can change it if you want.
            /// It should translate the message to the banks prefered format and then send to the bank by one of the 4 ways of system integration.
            /// in this case just contact a SOAP service to either get the UniversalResponse within the code or have the webservice send to a rabbitMQ channel.
        }

        public static void sendEnriched(byte[] body, IBasicProperties basic, string replyque)
        {
            var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Declares a que
                channel.QueueDeclare(queue: replyque, durable: true, exclusive: false, autoDelete: false, arguments: null);


                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.CorrelationId = basic.CorrelationId;
                properties.ContentType = "Bank response with interest rate.";


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: replyque, basicProperties: properties, body: body);


            }

        }
        public static void receiveMessage()
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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;

                    WebBankService.LoanRequest request = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));

                    WebBankService.UniversalResponse response = client.HandleRequest(request);
                    //implement logic to add interest rate to loan request
                    

                    //send message
                    var message = Serializer.SerializeObjectToUniversal(response);
                    sendEnriched(Encoding.UTF8.GetBytes(message), ea.BasicProperties, Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["reply"]));

                    ///// send another message to another channel 

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "LoanRequestB2",
                                     autoAck: false,
                                     consumer: consumer);
            }
        }
    }
}
