using Contracts;
using System;
using System.Collections.Generic;
using System.Messaging;

namespace MSMQTutorialServer
{
    internal class Program
    {
        //Path and description of the Public Queue which every Client writes to
        private static string path = @".\Private$\Public";
        private static string description = "This is the public Queue.";


        private static void Main(string[] args)
        {
            MessageQueue publicQueue = null;

            //List of registered users
            List<string> users = new List<string>();

            try
            {
                //initialize public queue
                if (MessageQueue.Exists(path))
                {
                    publicQueue = new MessageQueue(path);
                    publicQueue.Label = description;
                }
                else
                {
                    MessageQueue.Create(path);
                    publicQueue = new MessageQueue(path);
                    publicQueue.Label = description;
                }

                //set Formatter to get own message object when Receiving
                publicQueue.Formatter = new XmlMessageFormatter(new Type[]
                    {typeof(MessageObject)});

                while (true)
                {
                    //Receive message and cast to MessageObject class
                    //Receive removes the message from the queue 
                    Message msg = publicQueue.Receive();
                    MessageObject msgObject = (MessageObject)msg.Body;

                    //Server Log to Console
                    Console.WriteLine(msgObject.ToString());

                    //Add new users
                    if (!users.Contains(msgObject.Username))
                    {
                        users.Add(msgObject.Username);
                    }

                    switch (msgObject.Command)
                    {
                        //Send message to every user except sending user
                        case Command.Shout:
                            foreach (string user in users)
                            {
                                if(!(user == msgObject.Username))
                                Send(user, msgObject);
                            }
                            break;

                        //Send message to specific user if user is registered
                        case Command.Whisper:
                            if (!users.Contains(msgObject.ToUsername))
                            {
                                MessageObject newMsg = new MessageObject();
                                newMsg.Username = "System";
                                newMsg.Message = "No User with Name " + msgObject.ToUsername + " found.";
                                newMsg.Command = Command.System;
                                Send(msgObject.Username, newMsg);
                            }
                            else
                            {
                                Send(msgObject.ToUsername, msgObject);
                            }
                            break;

                            //Send all registered Users as a comma seperated string back to sending user
                        case Command.Users:
                            string userList = String.Join(",", users);
                            MessageObject userMsg = new MessageObject();
                            userMsg.Username= "System";
                            userMsg.Message = userList;
                            userMsg.Command = Command.Users;
                            Send(msgObject.Username, userMsg);
                            break;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                //disposes queue and let garbage collector free it
                //you could also use Purge() here to clear the contents of the queue
                //but this way, when the server is started again, it can't continue with
                //the messages it had stored.
                publicQueue?.Dispose();
            }
        }

        /// <summary>
        /// Sends a given MessageObject to given queue
        /// </summary>
        /// <param name="queueName">the name of the queue to send the message to</param>
        /// <param name="msg">the message object to be sent</param>
        private static void Send(string queueName, MessageObject msg)
        {
            string path = @".\Private$\" + queueName;

            MessageQueue queue = null;

            //initialize private queue
            if (MessageQueue.Exists(path))
            {
                queue = new MessageQueue(path);
                queue.Label = "Private Queue of " + queueName;
            }
            else
            {
                MessageQueue.Create(path);
                queue = new MessageQueue(path);
                queue.Label = "Private Queue of " + queueName;
            }

            //server log to Console
            Console.WriteLine("Sending...");
            Console.WriteLine(msg.ToString());

            queue.Send(msg);
        }
    }
}