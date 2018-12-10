using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;


namespace MSMQTutorialClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageQueue messageQueue = null;

            string description = "This is a test queue.";

            //string message = "This is a test message.";

            string path = @".\Private$\IDG";

            Console.WriteLine("Please insert your Username");

            string userName = Console.ReadLine();

            try
            {
                if (MessageQueue.Exists(path))
                {
                    messageQueue = new MessageQueue(path);
                    messageQueue.Label = description;
                }
                else
                {
                    MessageQueue.Create(path);
                    messageQueue = new MessageQueue(path);
                    messageQueue.Label = description;
                }

                //Sending Username and save SenderId in Server to authenticate on MessageBoard
                //@Andi Sender ID sollte irgendeine SID vergeben, die mit dem Zertifikat (das
                // sollte selbst erstellt werden, bei ner Public Queue dann auch nur eben zur
                // Identifizierung) verglichen wird, wenn des passt dann kommts durch
                // Über die SID könnten wir dann verifizieren welcher User gerade schreibt, also 
                // vlt in ein Array<Pair<Name, ID>> oder sowas. Wir könnten wenn es doppelte gibt dann
                // nummerieren, damit keiner einfach den selben User Namen eingibt um Scheiße zu trieben.
                // Für ne echte Anwendung denke ich durchaus sinnvoll, wennst in ne Datenbank schreibst oder so.
                // Die Zeit sollte automatisch angehängt werden (sent/received)
                Message auth = new Message(userName)
                {
                    AttachSenderId = true,
                    UseAuthentication = true
                };
                messageQueue.Send(auth);
            

                //Begin message Boarding
                Console.WriteLine("Please type your message...");
                string input = Console.ReadLine();

                while (input != "bye bye Client") // @Andi while(true) << kill Client?
                { 
                    Console.WriteLine("Sending...");
                    Message msg = new Message(input)
                    {
                        AttachSenderId = true,
                        UseAuthentication = true
                    };                 
                    messageQueue.Send(msg);

                    Console.WriteLine("Please type your message...");
                    input = Console.ReadLine();
                }
            }

            catch
            {
                throw;
            }
            finally
            {
                //List<string> msg = ReadQueue(path);
                //foreach(string str in msg)
                //{
                //    Console.WriteLine(str);
                //}

                //messageQueue.Purge();
                
                messageQueue.Dispose();
            }
        }

    //    private static List<string> ReadQueue(string path)
    //    {
    //        List<string> lstMessages = new List<string>();

    //        using (MessageQueue messageQueue = new MessageQueue(path))
    //        {
    //            System.Messaging.Message[] messages = messageQueue.GetAllMessages(); //.Recieve()?

    //            foreach (System.Messaging.Message message in messages)
    //            {

    //                message.Formatter = new XmlMessageFormatter(

    //                new String[] { "System.String, mscorlib" });

    //                string msg = message.Body.ToString();

    //                lstMessages.Add(msg);


    //                Console.WriteLine(msg.SentTime);
   
    //            }
    //        }
    //        return lstMessages;
    //    }
}
}
