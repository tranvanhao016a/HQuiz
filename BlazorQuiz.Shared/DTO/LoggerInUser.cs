using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorQuiz.Shared.DTO
{
    public record LoggedInUser(int Id, string Name, string Role, string Token)
    {
        public string ToJson() => JsonSerializer.Serialize(this);

        public Claim[] ToClaims() =>
             [
                 new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Role, Role),
                new Claim(nameof(Token), Token)
             ];
        public static LoggedInUser? LoadForm(string json) =>
            !string.IsNullOrWhiteSpace(json) ? 
           JsonSerializer.Deserialize<LoggedInUser>(json):null;
    }
   

    
}
