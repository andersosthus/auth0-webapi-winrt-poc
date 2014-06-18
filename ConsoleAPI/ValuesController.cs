using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;

namespace ConsoleAPI
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            // Here you can access the claims if you want...
            var claims = ClaimsPrincipal.Current.Claims;

            return new[] { "value1", "value2" };
        }
    }
}
