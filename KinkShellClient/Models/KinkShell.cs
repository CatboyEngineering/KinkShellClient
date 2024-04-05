using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models
{
    public struct KinkShell
    {
        public Guid ShellID { get; set; }
        public Guid OwnerID { get; set; }
        public string ShellName { get; set; }
        public List<KinkShellMember> Users { get; set; }
    }
}
