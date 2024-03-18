using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient.Models
{
    public enum ConnectionStatus
    {
        CLOSED,
        CONNECTING,
        CONNECTED,
        FAILED
    }
}
