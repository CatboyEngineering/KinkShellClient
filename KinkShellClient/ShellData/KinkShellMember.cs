﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.ShellData
{
    public struct KinkShellMember
    {
        public Guid AccountID { get; set; }
        public string DisplayName { get; set; }
        public bool SendCommands { get; set; }
    }
}
