using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading;

namespace Loaner
{
    class RabbitMQManager
    {
        public string Host_name { get; set; }

        public RabbitMQManager(string hostname)
        {
            Host_name = hostname;
        }
        string s;

        public void WorkerSendMessage(string Que_name, byte[] body)
        {
            var factory = new ConnectionFactory() { HostName = Host_name, UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Declares a que
                channel.QueueDeclare(queue: Que_name, durable: true, exclusive: false, autoDelete: false, arguments: null);


                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "Class of LoanRequest.";
                properties.CorrelationId = Guid.NewGuid().ToString();

                //Publish Message
                channel.BasicPublish(exchange: "", routingKey: Que_name, basicProperties: properties, body: body);
                Console.WriteLine(" [x] Sent {0}");


            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

