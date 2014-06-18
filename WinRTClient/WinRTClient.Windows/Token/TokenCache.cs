using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Security.Credentials;
using Auth0.SDK;

namespace WinRTClient.Token
{
    public class TokenCache
    {
        private readonly PasswordVault _vault;
        private const string Resource = "AuthTest";

        public TokenCache()
        {
            _vault = new PasswordVault();
        }

        public void StoreUser(Auth0User user)
        {
            if(user == null)
                throw new ArgumentNullException("user");

            var credential = GetFromVault(user.Profile["email"].ToString());

            if(credential != null)
                _vault.Remove(credential);

            var newCred = new PasswordCredential
            {
                UserName = user.Profile["email"].ToString(),
                Password = CacheHelpers.EncodeAuthUser(user),
                Resource = Resource
            };

            _vault.Add(newCred);
        }

        public Auth0User GetUser()
        {
            IReadOnlyList<PasswordCredential> credentials;

            try
            {
                credentials = _vault.FindAllByResource(Resource);
            }
            catch (Exception)
            {
                return null;
            }

            var credential = credentials.First();
            credential.RetrievePassword();

            var user = CacheHelpers.DecodeAuthUser(credential.Password);

            return user;
        }

        public void Clear()
        {
            IReadOnlyList<PasswordCredential> credentials;

            try
            {
                credentials = _vault.RetrieveAll();
            }
            catch (Exception)
            {
                return;
            }

            foreach (var credential in credentials)
            {
                _vault.Remove(credential);
            }
        }

        private PasswordCredential GetFromVault(string userName)
        {
            PasswordCredential credential;

            try
            {
                credential = _vault.Retrieve(Resource, userName);
            }
            catch (Exception)
            {
                credential = null;
            }

            return credential;
        }
    }
}
