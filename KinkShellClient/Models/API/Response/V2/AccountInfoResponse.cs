using System;

namespace CatboyEngineering.KinkShellClient.Models.API.Response.V2;

public struct AccountInfoResponse
{
    public string AccountID { get; set; }
    public string DisplayName { get; set; }
    public string? CharacterName { get; set; }
    public string? CharacterServer { get; set; }
    public int MaxAllowedShells { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsVerified { get; set; }
}
