using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregator
{
    class RabbitManager
    {
        List<string> ActiveAggregations = new List<string>();
        List<string> DeadAggregations = new List<string>();
        List<Responses> Aggregations = new List<Responses>();

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
                properties.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: messagetoreturn, basicProperties: properties, body: body);
                Console.WriteLine(" [x] Sent {0}", Encoding.UTF8.GetString(body));


            }

        }

        //Remake and add logic to figure out which bank send the message
        public void receiveMessage()
        {
            var factory = new ConnectionFactory() { HostName = "138.197.186.82", UserName = "admin", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Aggregator",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(0, 1, false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);

                //first recieve the first message
                consumer.Received += (model, ea) =>
                {
                    Console.WriteLine("Message Consumed!");
                    var body = ea.Body;
                    var header = ea.BasicProperties.Headers;

                    int ResponseAmount = int.Parse(Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["Requests"]));
                    string ID = ea.BasicProperties.CorrelationId;

                    //////// if the response is late it will just send the response directly to late-responses channel.
                    if (DeadAggregations.Contains(ID))
                    {
                        Console.WriteLine("ID was found in deadAggre");
                        var message = body;
                        sendEnriched(message, "Late-Responses");
                    }

                    ///// If the response is a response of a request which has allready seen another response : it will then =
                    else if (ActiveAggregations.Contains(ID))
                    {
                        Console.WriteLine("ID was found in ActiveAggre");
                        //// Find the Index of the aggregationlist
                        var index = Aggregations.FindIndex(R => R.Aggregation_ID == ID);
                        ///// Add the response to the list of responses in the aggregation.
                        Aggregations[index].MasterList.Add(Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body)));

                        //// It will also check if all responses has been received, if so. Send message and 
                        if (Aggregations[index].CheckIfFull())
                        {
                            sendEnriched(Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXml(Aggregations[index])), "AllResponses");
                            Console.WriteLine("Sending because FULL!");
                            //// Removes aggregation from aggregator and Active aggregationslist and adds to DeadAggregations list (((( Should not be neccesary, since all messages has been received )))
                            Aggregations.RemoveAt(index);
                            DeadAggregations.Add(ID);
                            ActiveAggregations.RemoveAt(ActiveAggregations.FindIndex(R => R.Contains(ID)));

                            //// *****NOT NECCESARY***** Just for good meassure, it will put the ID on dead aggregations in case something goes wrong, and the message was duplicated somewhere in the system.
                            new Thread(delegate () {
                                string deadid = ID;
                                var t2 = Task.Run(async delegate
                                {
                                    
                                    await Task.Delay(60 * 3 * 1000);
                                    DeadAggregations.RemoveAt(DeadAggregations.FindIndex(R => R.Contains(deadid)));
                                });
                            }).Start();
                            
                        }
                    }
                    else //// This means, that the response is the first response which is a part of a request that the system has seen. 
                    {
                        Console.WriteLine("ID was not found!");

                        if (ResponseAmount == 1) ///// If the request only expects 1 response, since only 1 bank was viable to the request - it should just send the response immedietly ... yes :)
                        { ///// Also means that it will not be added to the aggregation but just passed straight to responses.
                            Responses QuickResp = new Responses() { Aggregation_ID = ID, ExpectedResponses = ResponseAmount, MasterList = new List<UniversalResponse>() { Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body)) } };
                            sendEnriched(Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXml(QuickResp)), "AllResponses");
                            Console.WriteLine("Only 1 response expected! sending!");
                        }
                        else //// Otherwise just go ahead with setting up an aggregation of responses.
                        {
                            //// Adds correlation ID to the a list to controll easily.
                            ActiveAggregations.Add(ID);
                            //// Adds Response to a new (Responses) class, and adds ID and expectedResponses to controll when it can be sent.
                            Aggregations.Add(new Responses() { Aggregation_ID = ID, ExpectedResponses = ResponseAmount, MasterList = new List<UniversalResponse>() { Serializer.DeserializeObjectFromXml(Encoding.UTF8.GetString(body)) } });

                            //// Starts new thread that will count down from 30, when time is up: Send all responses regardsless of missing responses (The banks were to slow in delivering
                            
                               Console.WriteLine("Saved ID and starting counter to send and move aggregation to dead.");
                               
                            new Thread(delegate () {
                                string id = ID; //// Saves ID, since ID will change when it consumes new message.
                                var agindex = Aggregations.FindIndex(R => R.Aggregation_ID == id);  //// Finds Index where the ID is the same as the saven correlation ID. BEFORE starting delay
                                var t1 = Task.Run(async delegate
                                {
                                    await Task.Delay(30 * 1000);

                                    
                                    sendEnriched(Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXml(Aggregations[agindex])), "AllResponses"); //// send the responses from the ID that was saven.
                                    Console.WriteLine("sending Because 30 seconds have passed!");
                                    //// Removes aggregation and ID from the active stuff and put ID into deadAggregations so that the system can ignore further responses if the banks resposne was late.
                                    Aggregations.RemoveAt(agindex);
                                    DeadAggregations.Add(id);
                                    ActiveAggregations.RemoveAt(ActiveAggregations.FindIndex(R => R.Contains(id)));

                                    Console.WriteLine("Aggregation cmplete, starting countdown for remove ID from DeadAggregation");
                                    string deadid = id;   //// saves id which needs to be removed from DeadAggregations when 3 minutes has passed ((((( This only works, if we know the banks wont respond when 3 minuts has passed )))) *** Just estimate or something
                                    var t2 = Task.Run(async delegate
                                    {
                                        await Task.Delay(60 * 3 * 1000);
                                        DeadAggregations.RemoveAt(DeadAggregations.FindIndex(R => R.Contains(deadid))); ///// Finaly removes ID from DeadAggregations, so if per chance the ID will be generated again from another request, it'll still work.
                                    });
                                });

                            }).Start();
                            
                           
                        }
                    }
                    

                    
                    
                    ///// send another message to another channel 
                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Aggregator",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
