using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace oAuthOidcOwinClient.Controllers
{
    public class AccountController : Controller
    {
        //Values that might differ on our side
        private const string clientBaseUri = @"http://localhost:5436";
        private const string validIssuer = "SomeSecureCompany";

        //IdentityServer4
        private const string idPServerBaseUri = @"http://localhost:5000";
        private const string idPServerAuthUri = idPServerBaseUri + @"/connect/authorize";
        private const string idPServerTokenUriFragment = @"connect/token";
        private const string idPServerEndSessionUri = idPServerBaseUri + @"/connect/endsession";

        //These are also registered in the IdP (or Clients.cs of test IdP)
        private const string redirectUri = clientBaseUri + @"/account/oAuth2";
        private const string clientId = "authorizationCodeClient2";
        private const string clientSecret = "mysecret";
        private const string audience = "SomeSecureCompany/resources";
        private const string scope = "api";
        private const string response_type = "code";
        private const string grantType = "authorization_code";

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignIn()
        {
            var state = Guid.NewGuid().ToString("N");

            //Store state using cookie-based authentication middleware
            this.SaveState(state);

            //Redirect to IdP to get an Authorization Code
            var url = idPServerAuthUri +
                "?client_id=" + clientId +
                "&response_type=" + response_type +
                "&redirect_uri=" + redirectUri +
                "&scope=" + scope +
                "&state=" + state;

            return this.Redirect(url); //performs a GET
        }

        [HttpGet]
        public async Task<ActionResult> oAuth2()
        {
            var authorizationCode = this.Request.QueryString["code"];
            var state = this.Request.QueryString["state"];

            //Defend against CSRF attacks http://www.twobotechnologies.com/blog/2014/02/importance-of-state-in-oauth2.html
            await ValidateStateAsync(state);

            //Exchange Authorization Code for an Access Token by POSTing to the IdP's token endpoint
            string json = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(idPServerBaseUri);
                var content = new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string, string>("grant_type", grantType)
                    ,new KeyValuePair<string, string>("code", authorizationCode)
                    ,new KeyValuePair<string, string>("redirect_uri", redirectUri)
                    ,new KeyValuePair<string, string>("client_id", clientId)              //TODO: consider sending via basic authentication header
                    ,new KeyValuePair<string, string>("client_secret", clientSecret)
                });
                var httpResponseMessage = client.PostAsync(idPServerTokenUriFragment, content).Result;
                json = httpResponseMessage.Content.ReadAsStringAsync().Result;
            }

            //Extract the Access Token
            dynamic results = JsonConvert.DeserializeObject<dynamic>(json);
            string accessToken = results.access_token;

            //Validate token crypto and build claims identity principle
            var claims = ValidateToken(accessToken);                    //For OpenId Connect, this passed/validated state too, but with Authentication Code flow's extra hop for Access Token, that validation is required higher up (or CSRF attacks possible)
            var id = new ClaimsIdentity(claims, "Cookie");              //"Cookie" refers back to the middleware named in Startup.cs

            //Sign into the middleware so we can navigate around secured parts of this site (Try /Home/Secured)        
            this.Request.GetOwinContext().Authentication.SignIn(id);

            return this.Redirect("/Home"); 
        }

        private IEnumerable<Claim> ValidateToken(string token)
        {
            //Grab certificate for verifying JWT signature
            //In prod, we'd get this from the certificate store or similar
            var certPath = Path.Combine(Server.MapPath("~/bin"), "SscSign.pfx");
            var cert = new X509Certificate2(certPath);
            var x509SecurityKey = new X509SecurityKey(cert);

            var parameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = audience,
                ValidIssuer = validIssuer,
                IssuerSigningKey = x509SecurityKey,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            //Validate the token and retrieve ClaimsPrinciple
            var handler = new JwtSecurityTokenHandler();
            SecurityToken jwt;
            var id = handler.ValidateToken(token, parameters, out jwt);

            //Discard temp cookie and cookie-based middleware authentication objects (we just needed it for storing State)
            this.Request.GetOwinContext().Authentication.SignOut("TempCookie");

            return id.Claims;
        }

        //Defend against CSRF attacks http://www.twobotechnologies.com/blog/2014/02/importance-of-state-in-oauth2.html
        private async Task<AuthenticateResult> ValidateStateAsync(string state)
        {
            //Retrieve state value from TempCookie
            var authenticateResult = await this.Request
                .GetOwinContext()
                .Authentication
                .AuthenticateAsync("TempCookie");

            if (authenticateResult == null)
                throw new InvalidOperationException("No temp cookie");

            if (state != authenticateResult.Identity.FindFirst("state").Value)
                throw new InvalidOperationException("invalid state");

            return authenticateResult;
        }

        //Store values using cookie-based authentication middleware
        private void SaveState(string state)
        {
            var tempId = new ClaimsIdentity("TempCookie");
            tempId.AddClaim(new Claim("state", state));

            this.Request.GetOwinContext().Authentication.SignIn(tempId);
        }

        public ActionResult SignOut()
        {
            this.Request.GetOwinContext().Authentication.SignOut();
            return this.Redirect(idPServerEndSessionUri);
        }
    }
}