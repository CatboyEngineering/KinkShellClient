using KinkShellClient.ShellData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient.Windows.Utilities
{
    public class MainWindowState
    {
        public Plugin Plugin { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool HasError { get; set; }
        public string ErrorText { get; set; }
        public List<Guid> GuidsToAdd { get; set; }
        public List<Guid> GuidsToDelete { get; set; }

        public byte[] stringByteBuffer;

        public MainWindowState(Plugin plugin)
        {
            Plugin = plugin;

            SetDefauts();
        }

        public List<Guid> GetShellMembers(KinkShell kinkShell)
        {
            var shellMembers = new List<Guid>();

            shellMembers.AddRange(kinkShell.Users.Select(u => u.AccountID));
            shellMembers.RemoveAll(GuidsToDelete.Contains);
            shellMembers.AddRange(GuidsToAdd);

            return shellMembers;
        }

        public void SetDefauts()
        {
            IsAuthenticated = false;
            GuidsToAdd = new List<Guid>();
            GuidsToDelete = new List<Guid>();

            ClearErrors();
            ResetStringBuffer();
        }

        public void ResetStringBuffer()
        {
            stringByteBuffer = new byte[40];
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