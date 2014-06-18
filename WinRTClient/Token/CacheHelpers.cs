using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auth0.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinRTClient.Token
{
    public class CacheHelpers
    {
        private const string ElementDelimiter = ":";
        private const string SegmentDelimiter = "::";

        public static string EncodeAuthUser(Auth0User cacheKey)
        {
            var keyElements = new SortedDictionary<TokenElement, string>();

            keyElements[TokenElement.Auth0AccessToken] = cacheKey.Auth0AccessToken;
            keyElements[TokenElement.IdToken] = cacheKey.IdToken;
            keyElements[TokenElement.Profile] = JsonConvert.SerializeObject(cacheKey.Profile);

            return CreateKeyFromElements(keyElements);
        }

        internal static string CreateKeyFromElements(SortedDictionary<TokenElement, string> keyElements)
        {
            if (keyElements == null)
                throw new ArgumentNullException("keyElements");

            var keyHeader = String.Join(ElementDelimiter, keyElements.Keys.Select(k => (int)k));
            var keyContent = String.Join(ElementDelimiter, keyElements.Values.Select(Base64Encode));

            return CreateKey(keyHeader, keyContent);
        }

        private static string CreateKey(string keyHeader, string keyContent)
        {
            return string.Join(SegmentDelimiter, new[] { Base64Encode(keyHeader), Base64Encode(keyContent) });
        }

        internal static string Base64Encode(string input)
        {
            var encodedString = String.Empty;

            if (!string.IsNullOrEmpty(input))
                encodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

            return encodedString;
        }

        internal static string Base64Decode(string encodedInput)
        {
            var output = string.Empty;

            if (string.IsNullOrEmpty(encodedInput)) return output;

            var outputBytes = Convert.FromBase64String(encodedInput);
            output = Encoding.UTF8.GetString(outputBytes, 0, outputBytes.Length);

            return output;
        }

        public static Auth0User DecodeAuthUser(string cacheKey)
        {
            var elements = new Auth0User();
            IDictionary<TokenElement, string> elementDictionary = Decode(cacheKey);

            elements.Auth0AccessToken = elementDictionary.ContainsKey(TokenElement.Auth0AccessToken)
                ? elementDictionary[TokenElement.Auth0AccessToken]
                : null;

            elements.IdToken = elementDictionary.ContainsKey(TokenElement.IdToken)
                ? elementDictionary[TokenElement.IdToken]
                : null;

            elements.Profile = elementDictionary.ContainsKey(TokenElement.Profile)
                ? JsonConvert.DeserializeObject<JObject>(elementDictionary[TokenElement.Profile])
                : null;

            return elements;
        }

        internal static Dictionary<TokenElement, string> Decode(string cacheKey)
        {
            var keySegments = cacheKey.Split(new[] { SegmentDelimiter }, StringSplitOptions.None);

            if (keySegments.Length != 2)
                throw new ArgumentException("Invalid key format", "cacheKey");

            var headerElements = Base64Decode(keySegments[0]).Split(new[] { ElementDelimiter }, StringSplitOptions.None);
            var contentElements = Base64Decode(keySegments[1]).Split(new[] { ElementDelimiter }, StringSplitOptions.None);

            if (headerElements.Length != contentElements.Length)
                throw new ArgumentException("Invalid key format", "cacheKey");

            var keyElements = new Dictionary<TokenElement, string>();
            for (var i = 0; i < headerElements.Length; i++)
            {
                keyElements.Add((TokenElement)Enum.Parse(typeof(TokenElement), headerElements[i]), Base64Decode(contentElements[i]));
            }

            return keyElements;
        }
    }
}
