using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Project_Management.Handlers
{
    public class FirebaseUserAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger _logger;
        public FirebaseUserAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ILogger<FirebaseUserAuthenticationHandler> logger1) : base(options, logger, encoder, clock)
        {
            _logger = logger1;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) {
                _logger.LogWarning("Missing authorization header");
                return AuthenticateResult.Fail("Unauthorized");
            }
            
            string? authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader)) {
                _logger.LogWarning("Empty authorization header");
                return AuthenticateResult.Fail("Unauthorized");
            }

            string token = authorizationHeader.Trim();
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token)) return AuthenticateResult.Fail("Unauthorized");

            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                if (decodedToken is null)
                {
                    _logger.LogWarning("Null decoded token");
                    return AuthenticateResult.Fail("Unauthorized");
                }
                var claims = new List<Claim> {
                new Claim("Uid",decodedToken.Uid)};
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principle = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principle, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception err)
            {
                _logger.LogError($"{DateTime.Now} Authentication Failed: ", err);
                return AuthenticateResult.Fail(err.ToString());
            }

        }
    }
}
