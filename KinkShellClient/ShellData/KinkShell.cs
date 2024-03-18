using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient.ShellData
{
    public struct KinkShell
    {
        public Guid ShellID { get; set; }
        public Guid OwnerID { get; set; }
        public string ShellName { get; set; }
        public List<Guid> Users { get; set; }
    }
}
