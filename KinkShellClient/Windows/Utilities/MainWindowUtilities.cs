using System.Net;
using System.Threading.Tasks;

namespace KinkShellClient.Windows.Utilities
{
    public class MainWindowUtilities
    {
        public static async Task LogInAndRetrieve(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.Authenticate();

            if(result == HttpStatusCode.OK)
            {
                window.State.IsAuthenticated = true;
                await GetUserShells(plugin, window);
            }
            else
            {
                window.State.OnError("Incorrect login");
            }
        }

        public static async Task LogOut(Plugin plugin, MainWindow window)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.LogOut();

            if (result == HttpStatusCode.OK)
            {
                window.State.IsAuthenticated = false;
            }
        }

        public static async Task GetUserShells(Plugin plugin, MainWindow window)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.GetKinkShells();

            if (result != HttpStatusCode.OK)
            {
                window.State.OnError("Error retrieving Kinkshell list");
            }
        }

        public static async Task CreateShell(Plugin plugin, MainWindow window, string name)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.CreateShell(name);

            if (result != HttpStatusCode.Created)
            {
                if(result == HttpStatusCode.PaymentRequired)
                {
                    window.State.OnError("You cannot create any more shells!");
                }
                else
                {
                    window.State.OnError("Error creating a new shell");
                }
            }
        }
    }
}
