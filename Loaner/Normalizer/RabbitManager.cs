﻿using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace Normalizer
{
    class RabbitManager
    {
        private string[] connection;
        private string responseque;

        public RabbitManager(string[] con, string response)
        {
            connection = con;
            responseque = response;
        }
        

        //Remake into sending a enriched body to the aggregator
        public void sendEnriched(byte[] body, string messagetoreturn, IBasicProperties basic)
        {
            var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Declares a que
                channel.QueueDeclare(queue: messagetoreturn, durable: true, exclusive: false, autoDelete: false, arguments: null);





                

                channel.BasicPublish(exchange: "", routingKey: messagetoreturn, basicProperties: basic, body: body);
                Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


            }

        }

        //Remake and add logic to figure out which bank send the message
        public void receiveMessage()
        {
            var factory = new ConnectionFactory() { HostName = connection[0], UserName = connection[1], Password = connection[2] };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: responseque,
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


                    byte[] messages = Encoding.UTF8.GetBytes("WrongSHIT!");


                    try
                    {
                        XMLCPHBankClass cph = (XMLCPHBankClass)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(XMLCPHBankClass));
                        UniversalResponse UR = new UniversalResponse() { ssn = cph.ssn, interestrate = cph.interestRate };

                        messages = Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXmlType(UR, typeof(UniversalResponse)));
                    }catch (Exception e) { Console.WriteLine("Unsuccesfull Normalization with XMLCPH error: " + e.ToString()); }
                    try
                    {
                        JSONResponse json = (JSONResponse)Serializer.DeserializeObjectFromJsonType(Encoding.UTF8.GetString(body), typeof(JSONResponse));
                        UniversalResponse UR = new UniversalResponse() { ssn = json.ssn, interestrate = json.interestRate };

                        messages = Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXmlType(UR, typeof(UniversalResponse)));
                    } catch (Exception e) { Console.WriteLine("Unsuccesfull Normalization with JSONCPH error: " + e.ToString()); }
                    try
                    {
                        GoBankResponse go = (GoBankResponse)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(GoBankResponse));
                        UniversalResponse UR = new UniversalResponse() { ssn = go.ssn, interestrate = go.interestRate };

                        messages = Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXmlType(UR, typeof(UniversalResponse)));
                    }
                    catch (Exception e) { Console.WriteLine("Unsuccesfull Normalization with GOBANK error: " + e.ToString()); }
                    try 
                    {
                        UniversalResponse universal = (UniversalResponse)Serializer.DeserializeObjectFromXmlType(Encoding.UTF8.GetString(body), typeof(UniversalResponse));

                        messages = Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXmlType(universal, typeof(UniversalResponse)));
                    }
                    catch (Exception e) { Console.WriteLine("Unsuccesfull Normalization with C#Bank error: " + e.ToString()); }


                    ///
                    /// ADD BANK HERE!!! 
                    /// TRY CATCH IT AND MAKE ANOTHER PART IN THE SERIALIZER, ALSO ADD NEW CLASS FOR THE DATA STRUCTS!
                    ///


                    sendEnriched(messages, "Aggregator", ea.BasicProperties);

                    ///// send anotehr message to another channel 
                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: responseque,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
