namespace Contracts
{
    //Message Object which is sent from clients and servers
    public class MessageObject
    {
        //Who sent the message
        public string Username { get; set; }

        //What is the message about
        public Command Command { get; set; }

        //To whom should the message be directed (only Whisper)
        public string ToUsername { get; set; }

        //The actual message
        public string Message { get; set; }

        //ToString Method for Server Log
        public override string ToString()
        {
            string ret = "";
            ret += "Username : " + Username + "\n";
            ret += "Command : " + Command.ToString() + "\n";
            if (!string.IsNullOrEmpty(ToUsername)) ret += "To :" + ToUsername + "\n";
            ret += "Message : " + Message + "\n";

            return ret;
        }
    }
}