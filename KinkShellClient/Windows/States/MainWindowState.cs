using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Windows.States
{
    public class MainWindowState
    {
        public Plugin Plugin { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool HasError { get; set; }
        public string ErrorText { get; set; }
        public List<ShellNewUser> UsersToAdd { get; set; }
        public List<Guid> GuidsToDelete { get; set; }

        public string stringBuffer;
        public bool canSendCommands;
        public bool isRequestInFlight;

        public MainWindowState(Plugin plugin)
        {
            Plugin = plugin;

            SetDefauts();
        }

        public List<ShellNewUser> GetShellMembers(KinkShell kinkShell)
        {
            var shellMembers = new List<ShellNewUser>();

            kinkShell.Users.ForEach(u => shellMembers.Add(new ShellNewUser
            {
                UserID = u.AccountID,
                SendCommands = u.SendCommands
            }));

            shellMembers.RemoveAll(user => GuidsToDelete.Contains(user.UserID));
            shellMembers.AddRange(UsersToAdd);

            return shellMembers;
        }

        public void SetDefauts()
        {
            IsAuthenticated = false;
            UsersToAdd = new List<ShellNewUser>();
            GuidsToDelete = new List<Guid>();
            isRequestInFlight = false;

            ClearErrors();
            ResetBuffers();
        }

        public void ResetBuffers()
        {
            stringBuffer = "";
            canSendCommands = false;
        }

        public void OnError(string error)
        {
            HasError = true;
            ErrorText = error;
        }

        public void ClearErrors()
        {
            HasError = false;
            ErrorText = string.Empty;
        }
    }
}