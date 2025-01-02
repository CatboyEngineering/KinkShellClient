using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Windows.MainWindow
{
    public class MainWindowState
    {
        public Plugin Plugin { get; set; }
        public MainWindowScreen Screen { get; set; }
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
            UsersToAdd = new List<ShellNewUser>();
            GuidsToDelete = new List<Guid>();
            isRequestInFlight = false;

            SetDefaultScreen();
            ClearErrors();
            ResetBuffers();
        }

        public void SetDefaultScreen()
        {
            if (Plugin.Configuration.KinkShellServerLoginToken.IsNullOrEmpty())
            {
                if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerPassword.IsNullOrEmpty())
                {
                    Screen = MainWindowScreen.MIGRATE;
                }
                else
                {
                    Screen = MainWindowScreen.CREATE;
                }
            }
            else
            {
                Screen = MainWindowScreen.LOGIN;
            }
        }

        public void ResetBuffers()
        {
            stringBuffer = "";
            canSendCommands = true;
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