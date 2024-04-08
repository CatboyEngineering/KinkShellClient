namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public enum ShellSocketMessageType
    {
        // Issues a command to other users in a Shell.
        COMMAND,
        // States that we want to connect to a Shell.
        CONNECT,
        // Used to communicate system information to Shell users.
        INFO,
        // Used to communicate a user's toy status.
        STATUS,
        // A future option to support sending messages to the Shell.
        TEXT
    }
}
