namespace Contracts
{
    //Message Object which is sent from clients and servers
    public class MessageObject
    {
        //Who sent the message
        public string Username;

        //What is the message about
        public Command Command;

        //To whom should the message be directed (only Whisper)
        public string ToUsername;

        //The actual message
        public string Message;

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