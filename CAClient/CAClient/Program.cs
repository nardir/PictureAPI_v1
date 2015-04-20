using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CAClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:4374/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HttpResponseMessage response = await client.GetAsync("api/images/");
                //if (response.IsSuccessStatusCode)
                //{
                //    var images = await response.Content.ReadAsAsync<IEnumerable<ProductImage>>();
                //}

                //HttpResponseMessage response = await client.GetAsync("api/images/100");
                //if (response.IsSuccessStatusCode)
                //{
                //    if (!response.Content.IsMimeMultipartContent())
                //    {
                //        var imageBytes = await response.Content.ReadAsByteArrayAsync();

                //        MemoryStream stream = new MemoryStream(imageBytes);
                //        Image image = Image.FromStream(stream);

                //        image.Save(@"C:\Users\Nardi\Desktop\Images to upload\nrtest.jpg");
                //    }
                //}

                using (var request = new HttpRequestMessage(HttpMethod.Post, "api/images"))
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        var path = @"C:\Users\Nardi\Desktop\Images to upload\flower1.jpg";

                        using (var fileStream = new FileStream(path, FileMode.Open))
                        {
                            var fileName = Path.GetFileName(path);
                            content.Add(new StreamContent(fileStream), "file", fileName);

                            request.Content = content;

                            var response = await client.SendAsync(request);

                            if (response.IsSuccessStatusCode)
                            {

                            }
                        }
                    }
                }
            }
        }
    }
}
