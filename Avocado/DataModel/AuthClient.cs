using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Newtonsoft.Json;

namespace Avocado.DataModel
{

    class AuthClient
    {

        private static string API_URL_BASE = "https://avocado.io/api/";
        private static string API_URL_LOGIN = API_URL_BASE + "authentication/login";
        private static string API_URL_COUPLE = API_URL_BASE + "couple";
        private static string API_URL_ACTIVITIES = API_URL_BASE + "activities";
        private static string API_URL_LISTS = API_URL_BASE + "lists/";
        private static string API_URL_CONVERSATION = API_URL_BASE + "conversation/";
        private static string COOKIE_NAME = "user_email";
        private static string USER_AGENT = "Avocado Windows 8 Client v.1.0";
        private static string ERROR_MSG = "\nFAILED.  Signature was tested and failed. Try again and check the auth information.";
        private static string API_DEV_KEY = "xnWY1vZw064tCoigoeLIUt9wkfpjg2x6DpKjZzEn4YlYAhyULGTuHsBoyKXd+a3x";
        private static string API_DEV_ID = "46";

        Windows.Storage.ApplicationDataContainer roamingSettings;

        public string Email { get; set; }
        public string Password { get; set; }
        private string cookieValue;
        public string CookieValue {
            get
            {
                return cookieValue;
            }
            set
            {
                cookieValue = value;
                roamingSettings.Values["sessionCookie"] = value;
            }
        }
        private string avoSignature;
        public string AvoSignature {
            get
            {
                return avoSignature;
            }
            set
            {
                avoSignature = value;
                roamingSettings.Values["sessionSignature"] = value;
            }
        }

        public AuthClient() 
        {
            roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            var sessionSignature = (string)roamingSettings.Values["sessionSignature"];
            var sessionCookie = (string)roamingSettings.Values["sessionCookie"];
            if (!string.IsNullOrEmpty(sessionSignature) && !string.IsNullOrEmpty(sessionCookie))
            {
                AvoSignature = sessionSignature;
                CookieValue = sessionCookie;
            }
        }

        public bool AttemptLogin()
        {

            //Create the request for authenticating
            var req = new HttpClient();

            //Create the body 
            HttpContent body = new FormUrlEncodedContent(new[] { 
                new KeyValuePair<string, string>("email", Email), 
                new KeyValuePair<string, string>("password", Password) 
            });

            //Create the URI
            var uri = new Uri(API_URL_LOGIN);

            //Add the useragent header
            req.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);

            //Get the response
            var response = req.PostAsync(uri, body);
            response.Result.EnsureSuccessStatusCode();

            //Get the cookie value from the response
            var setCookie = response.Result.Headers.Where(x => x.Key == "Set-Cookie").First();
            var cookie = setCookie.Value.First();
            CookieValue = cookie.Substring(cookie.IndexOf("=") + 1, cookie.IndexOf(";") - cookie.IndexOf("=") - 1);

            //create the signature
            var tohash = CookieValue + API_DEV_KEY;
            var buffer = CryptographicBuffer.ConvertStringToBinary(tohash, BinaryStringEncoding.Utf8);
            var hasher = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hashed = hasher.HashData(buffer);
            AvoSignature = API_DEV_ID + ":" + CryptographicBuffer.EncodeToHexString(hashed);

            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["sessionSignature"] = AvoSignature;
            return true;
        }

        public CoupleModel GetUsers()
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                //create the new request
                var uri = new Uri(API_URL_COUPLE);
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.GetAsync(uri);

                var status2 = response.Result.StatusCode;
                var responseText = response.Result.Content.ReadAsStringAsync().Result;

                var couple = JsonConvert.DeserializeObject<CoupleModel>(responseText);
                return couple;
            }
        }

        public List<Activity> GetActivities()
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                //create the new request
                var uri = new Uri(API_URL_ACTIVITIES);
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.GetAsync(uri);

                var status2 = response.Result.StatusCode;
                var responseText = response.Result.Content.ReadAsStringAsync().Result;

                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                var activities = JsonConvert.DeserializeObject<List<Activity>>(responseText, settings);

                return activities;
            }
        }

        public ListModel GetList(string listId)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                //create the new request
                var uri = new Uri(API_URL_LISTS + listId);
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.GetAsync(uri);

                var status2 = response.Result.StatusCode;
                var responseText = response.Result.Content.ReadAsStringAsync().Result;

                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                var activities = JsonConvert.DeserializeObject<ListModel>(responseText, settings);

                return activities;
            }
        }

        public List<ListModel> GetListModelList()
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                //create the new request
                var uri = new Uri(API_URL_LISTS);
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.GetAsync(uri);

                var status2 = response.Result.StatusCode;
                var responseText = response.Result.Content.ReadAsStringAsync().Result;

                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                var lists = JsonConvert.DeserializeObject<List<ListModel>>(responseText, settings);

                foreach (var list in lists)
                {
                    foreach (var item in list.Items)
                    {
                        item.ListId = list.Id;
                    }
                }

                return lists;
            }
        }

        public void EditListItem(ListItemModel listItem)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                HttpContent body = new FormUrlEncodedContent(new[] { 
                    new KeyValuePair<string, string>("text", listItem.Text), 
                    new KeyValuePair<string, string>("complete", listItem.Complete ? "1" : "0"),
                    new KeyValuePair<string, string>("important", listItem.Important ? "1" : "0")
                });
                //create the new request
                var uri = new Uri(API_URL_LISTS + listItem.ListId + "/" + listItem.Id);
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.PostAsync(uri, body);

                response.Result.EnsureSuccessStatusCode();

            }
        }

        public void SendMessage(string message)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                HttpContent body = new FormUrlEncodedContent(new[] { 
                    new KeyValuePair<string, string>("message", message)
                });
                //create the new request
                var uri = new Uri(API_URL_CONVERSATION);
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.PostAsync(uri, body);

                response.Result.EnsureSuccessStatusCode();

            }
        }
    }
}