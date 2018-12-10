namespace Contracts
{
    //Commands to queue
    public enum Command
    {
        Shout,          //Publish to every user
        Whisper,        //direct only to one user
        System,         //error messages from server
        Users           //Ask (cleint) / return (server) a list of all registered users
    }
}