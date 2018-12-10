using Contracts;
using System;
using System.Messaging;
using System.Windows;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logic _logic;
        private string _username;

        private static string _path = @".\Private$\";

        public MainWindow()
        {
            InitializeComponent();
            tbUsername.Text = "default";
            btSend.IsEnabled = false;
            tbInput.IsEnabled = false;
        }

        private void Init()
        {
            // add supported commands
            cbMode.Items.Add(Command.Shout);
            cbMode.Items.Add(Command.Whisper);
            cbMode.Items.Add(Command.Users);

            // set default command
            cbMode.SelectedIndex = 0;

            // initialize logic
            _logic = new Logic(tbUsername.Text);

            // start listening on the private queue
            StartListening(_path + _username);
            
        }

        /// <summary>
        /// Creates a private queue using the given Path and
        /// sets a listener to receive events
        /// </summary>
        /// <param name="queuePath">path on which the queue will be openend</param>
        private void StartListening(string queuePath)
        {
            // Create an instance of MessageQueue. 
            MessageQueue privateQueue = new MessageQueue(queuePath);

            // Set its formatter.
            privateQueue.Formatter = new XmlMessageFormatter(new Type[]
                {typeof(MessageObject)});

            // Add an event handler for the ReceiveCompleted event.
            privateQueue.ReceiveCompleted +=
                new ReceiveCompletedEventHandler(MsgReceiveCompleted);

            // Begin the asynchronous receive operation.
            privateQueue.BeginReceive();
        }
        
        /// <summary>
        /// Writes a message to the chatboard that was sent or received
        /// </summary>
        /// <param name="msg">The MessageObject that was sent or received</param>
        /// <param name="send">Indicates whether it was a send or receive operation</param>
        private void AddMessage(MessageObject msg, bool send = false)
        {
            string line = "";

            if (msg.Command == Command.System)
            {
                line = "<ERROR> " + msg.Message;
            }
            else
            {
                string usertag = "";

                if (send)
                {
                    if (msg.Command == Command.Whisper && !string.IsNullOrEmpty(msg.ToUsername))
                    {
                        usertag = " to " + msg.ToUsername;
                    }
                }
                else
                {
                    if (msg.Command == Command.Shout || msg.Command == Command.Whisper)
                    {
                        usertag = " from " + msg.Username;
                    }
                }

                // add command and message to the chat board
                line = "<" + msg.Command + usertag + "> " + msg.Message;
            }
            line += "\n";
            tbChatboard.Text += line;
        }

        // Set the users combobox based on a comma seperated string
        private void SetUsers(string usernames)
        {
            cbUser.Items.Clear();
            cbUser.Items.Add("");
            string[] users = usernames.Split(',');
            foreach (var user in users)
            {
                cbUser.Items.Add(user);
            }
            cbUser.SelectedIndex = 0;
        }

        private void btUsername_Click(object sender, RoutedEventArgs e)
        {
            _username = tbUsername.Text;
            //set enable property of GUI fields to prevent unwanted input
            tbUsername.IsEnabled = false;
            btUsername.IsEnabled = false;
            tbInput.IsEnabled = true;
            btSend.IsEnabled = true;
            Init();
        }

        private void btSend_Click(object sender, RoutedEventArgs e)
        {
            // create Message object using current input
            MessageObject mo = new MessageObject
            {
                Command = (Command)cbMode.SelectedItem,
                Message = tbInput.Text,
                ToUsername = cbUser.SelectedItem?.ToString(),
                Username = tbUsername.Text
            };

            // add the created message to the chat board
            AddMessage(mo, true);

            // send the message in the corresponding queue
            _logic.SendMessage(mo);

            // refocus the input textbox
            tbInput.Focus();

            // clear input
            tbInput.Text = "";

            // scroll chat board to bottom
            scChatboard.ScrollToBottom();
        }

        /// <summary>
        /// Eventhandler for Receive Events 
        /// </summary>
        /// <param name="source">MessageQueue which has received a Message</param>
        /// <param name="asyncResult">Message that has been received</param>
        public void MsgReceiveCompleted(object source, ReceiveCompletedEventArgs asyncResult)
        {
            try
            {
                // Connect to the queue.
                MessageQueue mq = (MessageQueue)source;

                // End the asynchronous receive operation.
                Message m = mq.EndReceive(asyncResult.AsyncResult);

                MessageObject mo = m.Body as MessageObject;

                if (mo.Command == Command.Users)
                {
                    Dispatcher.Invoke(() => SetUsers(mo.Message));
                }
                else
                {
                    Dispatcher.Invoke(() => AddMessage(mo));
                }

                // Restart the asynchronous receive operation.
                mq.BeginReceive();
            }
            catch (MessageQueueException mqe)
            {
                // Handle sources of MessageQueueException.
                MessageObject mo = new MessageObject
                {
                    Command = Command.System,
                    Message = mqe.Message
                };
                Dispatcher.Invoke(() => AddMessage(mo));
            }

            return;
        }
    }
}