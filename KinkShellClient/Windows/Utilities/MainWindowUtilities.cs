using System.Net;
using System.Threading.Tasks;

namespace KinkShellClient.Windows.Utilities
{
    public class MainWindowUtilities
    {
        public static async Task LogInAndRetrieve(Plugin plugin, MainWindow window)
        {
            // TODO authenticate with the server, get an auth token, store in memory only, GET a list of shells, draw them on screen.
            var result = await plugin.ConnectionHandler.Authenticate();

            if(result == HttpStatusCode.OK)
            {
                window.State.IsAuthenticated = true;
            }
            else
            {
                window.State.OnError("Incorrect login");
            }
        }

        public static async Task LogOut(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.LogOut();

            if (result == HttpStatusCode.OK)
            {
                window.State.IsAuthenticated = false;
            }
        }
    }
}
