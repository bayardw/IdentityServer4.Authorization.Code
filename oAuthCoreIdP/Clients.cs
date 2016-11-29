
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace oAuthCoreIdP
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            var secret = new Secret { Value = "mysecret".Sha512() };

            return new List<Client> {
                new Client {
                    ClientId = "authorizationCodeClient2",
                    ClientName = "Authorization Code Client",
                    ClientSecrets = new List<Secret> { secret },
                    Enabled = true,
                    AllowedGrantTypes = new List<string> { "authorization_code" }, //DELTA //IdentityServer3 wanted Flow = Flows.AuthorizationCode,
                    RequireConsent = true,
                    AllowRememberConsent = false,
                    RedirectUris =
                      new List<string> {
                           "http://localhost:5436/account/oAuth2"
                      },
                    PostLogoutRedirectUris =
                      new List<string> {"http://localhost:5436"},
                    AllowedScopes = new List<string> {
                        "api"
                    },
                    AccessTokenType = AccessTokenType.Jwt
                }
            };
        }
    }
}