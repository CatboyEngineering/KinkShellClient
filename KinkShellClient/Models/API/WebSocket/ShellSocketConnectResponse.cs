﻿using KinkShellClient.ShellData;
using System;
using System.Collections.Generic;

namespace KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketTextResponse
    {
        public Guid ShellID { get; set; }
        public KinkShellMember UserFrom { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageText { get; set; }
    }
}
