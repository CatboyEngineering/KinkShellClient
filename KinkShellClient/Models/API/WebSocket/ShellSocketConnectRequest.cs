using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketConnectRequest
    {
        public Guid ShellID { get; set; }
    }
}
