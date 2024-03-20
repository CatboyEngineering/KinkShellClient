using KinkShellClient.ShellData;
using System.Collections.Generic;

namespace KinkShellClient.Models.API.Response
{
    public struct ShellListResponse
    {
        public List<KinkShell> Shells { get; set; }
    }
}
