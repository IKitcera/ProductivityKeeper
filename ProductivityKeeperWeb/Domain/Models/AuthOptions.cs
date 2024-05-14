namespace ProductivityKeeperWeb.Domain.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "http://localhost:65070"; // издатель токена
        public const string AUDIENCE = "http://localhost:65070"; // потребитель токена
        const string KEY = "MIIJQgIBADANBgkqhkiG9w0BAQEFAASCCSwwggkoAgEAAoICAQDRs5EzYQIb0GFs" +
            "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" +
"KezGXU4nicAY5oguH0sFFSEM4Ul8JA==";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 1 минута
        public static Microsoft.IdentityModel.Tokens.SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(KEY));
        }
    }
}
