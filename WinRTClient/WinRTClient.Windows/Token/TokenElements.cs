using Newtonsoft.Json.Linq;

namespace WinRTClient.Token
{
    internal class TokenElements
    {
        public string Auth0AccessToken { get; set; }
        public string IdToken { get; set; }
        public JObject Profile { get; set; }
    }

    enum TokenElement
    {
        Auth0AccessToken = 0,
        IdToken = 1,
        Profile = 2
    }
}
