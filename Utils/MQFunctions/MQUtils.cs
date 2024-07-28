using Apache.NMS.ActiveMQ;
using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace JMS_Consumer_TimerTigger_Function.Utils.MQFunctions
{
    public class MQUtils
    {

        private const string BROKER_URI = "activemq:tcp://localhost:61616?wireFormat.tightEncodingEnabled=true";
        private const string QUEUE_NAME = "genesis.queue";

        public static void SendMessage(string message)
        {
            try
            {
                // Create a connection factory
                IConnectionFactory factory = new ConnectionFactory(BROKER_URI);

                // Create a connection
                using IConnection connection = factory.CreateConnection();
                connection.Start();

                // Create a session
                using (ISession session = connection.CreateSession(AcknowledgementMode.ClientAcknowledge))
                {

                    // Create the destination (queue)
                    IDestination destination = session.GetQueue(QUEUE_NAME);

                    // Create a producer
                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        producer.DeliveryMode = MsgDeliveryMode.Persistent;

                        // Create a text message
                        ITextMessage textMessage = producer.CreateTextMessage(message);

                        // Send the message
                        producer.Send(textMessage);
                    }
                }

                Console.WriteLine($"Sent message: {message}");
            }
            catch (System.Exception)
            {
                throw;
            }
        }



        public static string ReceiveMessage()
        {
            try
            {
                // Create a connection factory
                IConnectionFactory factory = new ConnectionFactory(BROKER_URI);

                // Create a connection
                using (IConnection connection = factory.CreateConnection())
                {
                    connection.Start();

                    // Create a session
                    using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                    {
                        // Create the destination (queue)
                        IDestination destination = session.GetQueue(QUEUE_NAME);

                        // Create a consumer
                        using IMessageConsumer consumer = session.CreateConsumer(destination);

                        // Receive the message
                        ITextMessage message = consumer.Receive() as ITextMessage;
                        if (message != null)
                        {
                            Console.WriteLine($"Received message: {message.Text}");
                        }
                        else
                        {
                            Console.WriteLine("No message received.");
                        }
                        return $"Received message: \n{message.Text}";
                    }
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        public async static Task<List<ITextMessage>> ReceiveMessages()
        {
            try
            {
                // Create a connection factory
                IConnectionFactory factory = new ConnectionFactory(BROKER_URI);

                List<ITextMessage> textMessages = new List<ITextMessage>();

                // Create a connection
                using (IConnection connection = await factory.CreateConnectionAsync())
                {
                    connection.Start();

                    // Create a session
                    using (ISession session = await connection.CreateSessionAsync(AcknowledgementMode.AutoAcknowledge))
                    {
                        // Create the destination (queue)
                        IDestination destination = await session.GetQueueAsync(QUEUE_NAME);

                        // Create a consumer
                        using IMessageConsumer consumer = await session.CreateConsumerAsync(destination);


                        while (true)
                        {
                            IMessage message = consumer.Receive(TimeSpan.FromSeconds(10));

                            // System.Console.WriteLine(message);
                            if (message is ITextMessage textMessage)
                            {
                                Console.WriteLine("Received message: " + textMessage.Text);
                                textMessages.Add(textMessage);
                            }
                            else if (message == null)
                            {
                                // No more messages in the queue
                                break;

                            }
                            // else
                            // {
                            //     break;

                            // }
                            // connection.Close();
                        }

                        // Receive the message
                        // ITextMessage message = consumer.Receive() as ITextMessage;
                        // if (message != null)
                        // {
                        //     Console.WriteLine($"Received message: {message.Text}");
                        // }
                        // else
                        // {
                        //     Console.WriteLine("No message received.");
                        // }
                        return textMessages;
                    }
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}