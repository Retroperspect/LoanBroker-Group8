using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebBank
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public void HandleRequest()
        {
            receiveMessage();
        }

        public void sendEnriched(byte[] body, IBasicProperties basic, string replyque)
        {
            var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Declares a que
                channel.QueueDeclare(queue: replyque, durable: true, exclusive: false, autoDelete: false, arguments: null);


                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object>();
                properties.Persistent = true;
                properties.CorrelationId = basic.CorrelationId;
                properties.ContentType = "Bank response with interest rate.";


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: "RequestFromBank", basicProperties: properties, body: body);


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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;


                    LoanRequest request = Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body));

                    //implement logic to add interest rate to loan request
                    UniversalResponse urMessage = URConversion(request, 2);

                    //send message
                    var message = Serializer.SerializeObjectToUniversal(urMessage);
                    sendEnriched(Encoding.UTF8.GetBytes(message), ea.BasicProperties, Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["reply"]));

                    ///// send another message to another channel 

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "LoanRequestB2",
                                     autoAck: false,
                                     consumer: consumer);
            }
        }
        public UniversalResponse URConversion(LoanRequest request, decimal interestRate)
        {
            UniversalResponse response = new UniversalResponse();
            response.ssn = request.ssn;
            response.interestrate = interestRate;
            return response;
        }
    }
}
