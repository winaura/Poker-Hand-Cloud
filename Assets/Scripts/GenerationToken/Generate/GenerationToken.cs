using System;
using System.Collections.Generic;
using JWT;

namespace PokerHand.Token
{
    public static class GenerationToken
    {
        public static string GetToken(DateTime date, string email = null)
        {
            const string secretCode = "C8KrE5q544uqDTgSFODd";

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var slip =  Math.Round((date - unixEpoch).TotalSeconds);
            var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
            var expirationTime = now + 24 * 3600;

            var payload = new Dictionary<string, object>();

            payload.Add("UserName", email);
            payload.Add("role", "User");
            payload.Add("exp", expirationTime);

            return JsonWebToken.Encode(payload, secretCode, JwtHashAlgorithm.HS256);
        }
        private static string GetToken(string email, string secretCode)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
            var expirationTime = now + 24 * 3600;

            var payload = new Dictionary<string, object>();

            payload.Add("UserName", email);
            payload.Add("role", "User");
            payload.Add("exp", expirationTime);

            return JsonWebToken.Encode(payload, secretCode, JwtHashAlgorithm.HS256);
        }
    }
}
