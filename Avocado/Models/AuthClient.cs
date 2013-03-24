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
using System.Collections.ObjectModel;
using Avocado.ViewModels;
using Windows.Storage.Streams;

namespace Avocado.Models
{

    class AuthClient
    {

        private static string API_URL_BASE = "https://avocado.io/api/";
        private static string API_URL_LOGIN = API_URL_BASE + "authentication/login";
        private static string API_URL_COUPLE = API_URL_BASE + "couple";
        private static string API_URL_ACTIVITIES = API_URL_BASE + "activities";
        private static string API_URL_LISTS = API_URL_BASE + "lists/";
        private static string API_URL_CONVERSATION = API_URL_BASE + "conversation/";
        private static string API_URL_CALENDAR = API_URL_BASE + "calendar/";
        private static string API_URL_MEDIA = API_URL_BASE + "media/";
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

        #region DataAccessHelpers

        private HttpResponseMessage PostBuffer(Uri uri, MultipartFormDataContent content)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));

                var response = client.PostAsync(uri, content);

                return response.Result;

            }
        }

        private HttpResponseMessage Post(Uri uri, FormUrlEncodedContent body)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.PostAsync(uri, body);

                return response.Result;

            }
        }

        private HttpResponseMessage Get(Uri uri)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));

                var response = client.GetAsync(uri);
                return response.Result;
            }
        }
        
        #endregion

        #region GetData

        public Couple GetUsers()
        {
            var uri = new Uri(API_URL_COUPLE);
            var response = Get(uri);
            response.EnsureSuccessStatusCode();

            var couple = JsonConvert.DeserializeObject<Couple>(response.Content.ReadAsStringAsync().Result);
            return couple;
        }

        public List<Activity> GetActivities()
        {
            var uri = new Uri(API_URL_ACTIVITIES);
            var response = Get(uri);
            response.EnsureSuccessStatusCode();
            
            var activities = JsonConvert.DeserializeObject<List<Activity>>(response.Content.ReadAsStringAsync().Result);
            return activities;
        }

        public AvoList GetList(string listId)
        {

            var uri = new Uri(API_URL_LISTS + listId);
            var response = Get(uri);
            response.EnsureSuccessStatusCode();

            var list = JsonConvert.DeserializeObject<AvoList>(response.Content.ReadAsStringAsync().Result);
            return list;
        }

        public List<AvoList> GetListModelList()
        {
            var uri = new Uri(API_URL_LISTS);
            var response = Get(uri);

            var lists = JsonConvert.DeserializeObject<List<AvoList>>(response.Content.ReadAsStringAsync().Result);
            foreach (var list in lists)
            {
                foreach (var item in list.Items)
                {
                    item.ListId = list.Id;
                }
            }

            return lists;
        }

        public ObservableCollection<CalendarItem> GetCalendar()
        {
            var uri = new Uri(API_URL_CALENDAR + "?days=120");
            var response = Get(uri);

            var calendarItems = JsonConvert.DeserializeObject<ObservableCollection<CalendarItem>>(response.Content.ReadAsStringAsync().Result);
            return calendarItems;
        }

        public List<Media> GetMedia()
        {
            var uri = new Uri(API_URL_MEDIA);
            var response = Get(uri);
            response.EnsureSuccessStatusCode();

            var media = JsonConvert.DeserializeObject<List<Media>>(response.Content.ReadAsStringAsync().Result);
            return media;
        }

        #endregion

        #region EditData

        public void EditListItem(AvoListItem listItem, int index, bool delete = false)
        {
            var uri = new Uri(API_URL_LISTS + listItem.ListId + "/" + listItem.Id + (delete ? "/delete" : ""));
            var body = new FormUrlEncodedContent(new[] { 
                    new KeyValuePair<string, string>("text", listItem.Text), 
                    new KeyValuePair<string, string>("complete", listItem.Complete ? "1" : "0"),
                    new KeyValuePair<string, string>("important", listItem.Important ? "1" : "0"),
                    new KeyValuePair<string, string>("index", index.ToString())
                });
            if (delete)
            {
                body = new FormUrlEncodedContent(new KeyValuePair<string, string>[] { });
            }
            var response = Post(uri, body);
            response.EnsureSuccessStatusCode();
        }

        public void CreateListItem(string listItemText, string listId, int index)
        {
            if (string.IsNullOrEmpty(listItemText))
            {
                return;
            }

            var uri = new Uri(API_URL_LISTS + listId);
            var body = new FormUrlEncodedContent(new[] { 
                new KeyValuePair<string, string>("text", listItemText),
                new KeyValuePair<string, string>("index", index.ToString())
            });
            var response = Post(uri, body);
            response.EnsureSuccessStatusCode();
        }

        public void SendMessage(string message)
        {
            var uri = new Uri(API_URL_CONVERSATION);
            var body = new FormUrlEncodedContent(new[] { 
                new KeyValuePair<string, string>("message", message)
            });

            var response = Post(uri, body);
            response.EnsureSuccessStatusCode();
        }

        public void UploadPhoto(string fileName, string contentType, System.IO.Stream photoContents, string caption = "")
        {
            var uri = new Uri(API_URL_MEDIA);
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(caption), "\"caption\"");

            var t = new StreamContent(photoContents);
            t.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            fileName = string.IsNullOrEmpty(fileName) ? "img.png" : fileName;
            content.Add(t, "\"media\"", "\"" + fileName + "\"");

            var response = PostBuffer(uri, content);

            response.EnsureSuccessStatusCode();
        }

        public void DeleteActivity(string id)
        {
            var uri = new Uri(string.Format("{0}/{1}/delete", API_URL_ACTIVITIES, id));
            var body = new FormUrlEncodedContent(new KeyValuePair<string, string>[] { });
            
            var response = Post(uri, body);
            response.EnsureSuccessStatusCode();
        }

        #endregion
    }
}