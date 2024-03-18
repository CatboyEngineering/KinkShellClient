namespace KinkShellClient.Models.API.WebSocket
{
    public enum ShellSocketMessageType
    {
        // Issues a command to other users in a Shell.
        COMMAND,
        // States that we want to connect to a Shell.
        CONNECT,
        // States that we want to voluntarily disconnect from a Shell.
        DISCONNECT,
        // Used to communicate system information to Shell users.
        INFO,
        // A future option to support sending messages to the Shell.
        TEXT
    }
}
