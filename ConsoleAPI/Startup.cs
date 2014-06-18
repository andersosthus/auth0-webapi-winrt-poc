using System.Web.Http;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Owin;

namespace ConsoleAPI
{
    public class Startup
    {
        private const string Issuer = "";
        private const string Audience = "";
        readonly byte[] _secret = TextEncodings.Base64Url.Decode("");
        
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jwtBearerAuthenticationOptions = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { Audience },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(Issuer, _secret)
                }
            };

            app.UseJwtBearerAuthentication(jwtBearerAuthenticationOptions);

            app.UseWebApi(config);
        }
    }
}