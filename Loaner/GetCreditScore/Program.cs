using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading;



namespace GetCreditScore
{
    class Program
    {
        public string messagesa;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.receiveMessage();
            CreditScoreService.CreditScoreServiceClient client = new CreditScoreService.CreditScoreServiceClient();
            Console.WriteLine(p.messagesa);
            Console.ReadLine();

        }

        private void sendEnriched(byte[] body)
        {
            var factory = new ConnectionFactory() { HostName = "207.154.250.196", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Declares a que
                channel.QueueDeclare(queue: "CreditEnriched", durable: true, exclusive: false, autoDelete: false, arguments: null);


                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;


                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: "CreditEnriched", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        private void receiveMessage()
        {
            var factory = new ConnectionFactory() { HostName = "207.154.250.196" , UserName= "admin", Password= "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "RequestLoan",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    CreditScoreService.CreditScoreServiceClient client = new CreditScoreService.CreditScoreServiceClient();
                    int l = client.creditScore(message);
                    Console.WriteLine(l);
                    message = message +" - " +l;
                    sendEnriched(Encoding.UTF8.GetBytes(message));
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
