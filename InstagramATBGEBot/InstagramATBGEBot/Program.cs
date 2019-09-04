using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using InstaSharper;
using InstaSharper.Classes;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Logger;
using System;
using System.IO;
using InstaSharper.Classes.Models;

namespace InstagramATBGEBot
{
    class InstagramBot
    {
        private const string username = "hlsmurf1";
        private const string password = "Bluebunny1";
        private static UserSessionData user;
        private static IInstaApi api;

        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit=5&t=day";
        public static List<Bitmap> photos = new List<Bitmap>();
        static void Main(string[] args)
        {

            Rootobject results = GetJsonForToday();
            TakeImagesFromResults(results);

        }

        static Rootobject GetJsonForToday()
        {
            Uri reddit = new Uri(redditWorkingUrl);
            HttpClient client = new HttpClient();
            string json = new WebClient().DownloadString(reddit);
            Rootobject objectResponse = JsonConvert.DeserializeObject<Rootobject>(json);
            return objectResponse;
        }

        static void TakeImagesFromResults(Rootobject topToday)
        {
            int resultAmnt = (int)topToday.data.children.GetLongLength(0);
            for (int i = 0; i < resultAmnt; i++)
            {
                WebRequest request = WebRequest.Create(topToday.data.children[i].data.url);
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                    response.GetResponseStream();
                Bitmap photo = new Bitmap(responseStream);
                //photo.Save("distortExamie" + i.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
                photos.Add(photo);

                user = new UserSessionData();
                user.UserName = username;
                user.Password = password;
                Login();

                UploadPics(topToday);

                Console.Read();
            }
        }

        public static async void Login()
        {
            api = InstaApiBuilder.CreateBuilder()
                .SetUser(user)
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .Build();

            var loginRequest = await api.LoginAsync();
            if (loginRequest.Succeeded)
                Console.WriteLine("Logged in successfully.");
            else
                Console.WriteLine("Error logging in!" + loginRequest.Info.Message);
        }
        public static async void UploadPics(Rootobject pics)
        {
            int picsAmnt = (int)pics.data.children.GetLongLength(0);
            for (int i = 0; i < picsAmnt; i++)
            {
                InstaImage img = new InstaImage();
            }
            
            var result = await api.UploadPhotoAsync(photos[0], "caption");

        }

    }

}
