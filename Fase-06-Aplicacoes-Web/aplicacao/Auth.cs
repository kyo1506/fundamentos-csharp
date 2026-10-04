using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Fase06.GestaoAcademica.WebApi;

public record LoginRequest(string Usuario, string Senha);

public record LoginResponse(string Token, string Tipo, int ExpiraEmSegundos);

public static class JwtHelper
{
    public const string ChaveSecretaPadrao = "ChaveSuperSecretaComMaisDeTrintaEDoisCaracteres123!";
    public const string Emissor = "GestaoAcademicaApi";
    public const string Audiencia = "GestaoAcademicaClients";

    public static string GerarToken(string usuario, string role, string chaveSecreta = ChaveSecretaPadrao)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] chaveBytes = Encoding.UTF8.GetBytes(chaveSecreta);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, usuario),
                new Claim(ClaimTypes.Role, role)
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = Emissor,
            Audience = Audiencia,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(chaveBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
