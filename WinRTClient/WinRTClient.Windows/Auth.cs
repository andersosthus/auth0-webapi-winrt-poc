using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Auth0.SDK;
using Newtonsoft.Json;
using WinRTClient.Token;

namespace WinRTClient
{
    public class Auth
    {
        private const string Domain = "";
        private const string ClientId = "";
        private readonly TokenCache _tokenCache;

        public Auth0User AuthenticatedUser { get; private set; }

        public Auth()
        {
            _tokenCache = new TokenCache();
        }

        public async Task<bool> Authenticate()
        {
            var cached = _tokenCache.GetUser();

            if (cached == null)
            {
                var authClient = new Auth0Client(Domain, ClientId);
                var result =
                    await
                        authClient.LoginAsync()
                            .ContinueWith(t => t.Result, TaskScheduler.FromCurrentSynchronizationContext());

                if (result == null) return false;

                AuthenticatedUser = result;
                _tokenCache.StoreUser(result);
            }
            else
            {
                AuthenticatedUser = cached;
            }

            return true;
        }

        public async Task<string> GetUserProfile()
        {
            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUser.IdToken);
            var userProfileResult =
                await
                    apiClient.GetStringAsync("https://andersosthus.auth0.com/api/users/" + AuthenticatedUser.Profile["user_id"]);

            return userProfileResult;
        }

        public async Task<AuthenticationHeaderValue> GetDelegatedToken()
        {
            var httpClient = new HttpClient();
            var paramsDict = new Dictionary<string, string>
            {
                {"client_id", ClientId},
                {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                {"id_token", AuthenticatedUser.IdToken},
                {"target", "i3edqLVVpzCJ2560FGYi5LmBlVCqmBxz"},
                {"scope", "openid"}
            };

            var delegationContent = new StringContent(JsonConvert.SerializeObject(paramsDict), Encoding.UTF8, "application/json");
            var delegationResult = await httpClient.PostAsync("https://andersosthus.auth0.com/delegation", delegationContent);
            var delegationString = await delegationResult.Content.ReadAsStringAsync();

            var delegation = await JsonConvert.DeserializeObjectAsync<Dictionary<string, string>>(delegationString);
            var authHeader = new AuthenticationHeaderValue(delegation["token_type"], delegation["id_token"]);

            return authHeader;
        }

        public void Logout()
        {
            _tokenCache.Clear();
            AuthenticatedUser = null;
        }

    }
}
