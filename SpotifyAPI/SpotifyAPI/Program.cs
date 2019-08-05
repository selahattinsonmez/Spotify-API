using System;

using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SpotifyAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientId = "YOUR_CLİENT_ID";
            String clientSecret = "YOUR_CLIENT_SECRET";
            AccessToken token = GetToken(clientId, clientSecret).Result;
            Console.WriteLine(String.Format("Access Token: {0}", token.access_token));
            var response = getArtist(token, "ARTIST_ID");
            Console.ReadLine();
        }
        public class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
        }

        static async Task<AccessToken> GetToken(string clientId, string clientSecret)
        {
            string credentials = String.Format("{0}:{1}", clientId, clientSecret);

            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new System.Collections.Generic.List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync("https://accounts.spotify.com/api/token", requestBody);
                var response = await request.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AccessToken>(response);
            }
        }

        static async Task<dynamic> getArtist(AccessToken token, string artistId)
        {

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);


                var request = await client.GetAsync("https://api.spotify.com/v1/artists/" + artistId);
                request.Content.Equals("application/json");
                var response = await request.Content.ReadAsStringAsync();
                dynamic artist = JsonConvert.DeserializeObject(response);

                return artist;


            }
        }
    }
}
