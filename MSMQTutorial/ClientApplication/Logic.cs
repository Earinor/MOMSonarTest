using Contracts;
using System.Messaging;

namespace ClientApplication
{
    public class Logic
    {
        // constant value of every started queue path
        private static string _path = @".\Private$\";
        private static string _publicQueuePath = "Public";
        private static string description = "This is the public Queue.";
        private static string pubDesc = "This is my personal Queue.";
        private string _username;
        

        private MessageQueue _publicQueue;
        private MessageQueue _privateQueue;

        public Logic(string username)
        {
            _username = username;

            // initialize public queue 

            string publicPath = _path + _publicQueuePath;
            
            if (MessageQueue.Exists(publicPath))
            {
                _publicQueue = new MessageQueue(publicPath);
                _publicQueue.Label = description;
            }
            else
            {
                MessageQueue.Create(publicPath);
                _publicQueue = new MessageQueue(publicPath);
                _publicQueue.Label = description;
            }

            // initialize private queue

            string privatePath = _path + username;

            if (MessageQueue.Exists(privatePath))
            {
                _privateQueue = new MessageQueue(privatePath);
                _privateQueue.Label = pubDesc;
            }
            else
            {
                MessageQueue.Create(privatePath);
                _privateQueue = new MessageQueue(privatePath);
                _privateQueue.Label = pubDesc;
            }
        }

        public void SendMessage(MessageObject msg)
        {
            _publicQueue.Send(msg);
        }

    }
}