using PictureAPI_v1.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace PictureAPI_v1.Controllers
{
    [RoutePrefix("api/images/v1")]
    public class ProductImageController : ApiController
    {
        static private Dictionary<int, ProductImage> images;

        static ProductImageController()
        {
            images = new Dictionary<int, ProductImage>();

            images.Add(100, new ProductImage() { ProductImageId = 100 });
            images.Add(200, new ProductImage() { ProductImageId = 200 });
            images.Add(300, new ProductImage() { ProductImageId = 300 });
            images.Add(400, new ProductImage() { ProductImageId = 400 });
            images.Add(500, new ProductImage() { ProductImageId = 500 });
        }

        [Route("")]
        public IEnumerable<ProductImage> GetProductImages()
        {
            return images.Values.ToArray();
        }

        [Route("{id:int}", Name = "GetProductImage")]
        public async Task<HttpResponseMessage> GetProductImage(int Id)
        {
            HttpResponseMessage response;

            if (images.ContainsKey(Id))
            {
                String filePath = HostingEnvironment.MapPath(string.Format("~/Content/Images/{0}.jpg", Id));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    using (Image image = Image.FromStream(fileStream))
                    {
                        using(MemoryStream memoryStream = new MemoryStream())
                        {
                            image.Save(memoryStream, ImageFormat.Jpeg);

                            response = new HttpResponseMessage(HttpStatusCode.OK);

                            response.Content = new ByteArrayContent(memoryStream.ToArray());
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        }
                    }                 
                }
            }
            else
                response = new HttpResponseMessage(HttpStatusCode.NotFound);

            return await Task.FromResult(response);

            //if (images.ContainsKey(Id))
            //{
            //    return await Task.FromResult(Ok(images[Id]));
            //}

            //return await Task.FromResult(NotFound());
        }

        [Route("")]
        [HttpPost]
        //public async Task<IHttpActionResult> CreateProductImage()
        public async Task<HttpResponseMessage> CreateProductImage()
        {
            HttpResponseMessage response;

            if (Request.Content.IsMimeMultipartContent())
            {
                var id = images.Keys.Max() + 100;

                images.Add(id, new ProductImage { ProductImageId = id });
                string uri = Url.Link("GetProductImage", new { id = id });

                MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();

                provider = await Request.Content.ReadAsMultipartAsync();

                foreach (HttpContent content in provider.Contents)
                {
                    Stream stream = await content.ReadAsStreamAsync();
                    Image image = Image.FromStream(stream);
                    var testName = content.Headers.ContentDisposition.Name;

                    String filePath = HostingEnvironment.MapPath("~/Content/Images/");
                    //String[] headerValues = (String[])Request.Headers.GetValues("UniqueId");
                    //String fileName = headerValues[0] + ".jpg";
                    String fileName = string.Format("{0}.jpg", id);
                    String fullPath = Path.Combine(filePath, fileName);
                    image.Save(fullPath);
                }

                response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = new Uri(uri);
            }
            else
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            return response;

            //return await Task.FromResult(Created(new Uri(uri), new ProductImage { ProductImageId = id }));

            //var result = new HttpResponseMessage(HttpStatusCode.OK);
            //if (Request.Content.IsMimeMultipartContent())
            //{
            //    Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider()).ContinueWith((task) =>
            //    {
            //        MultipartMemoryStreamProvider provider = task.Result;
            //        foreach (HttpContent content in provider.Contents)
            //        {
            //            Stream stream = content.ReadAsStreamAsync().Result;
            //            Image image = Image.FromStream(stream);
            //            var testName = content.Headers.ContentDisposition.Name;
            //            String filePath = HostingEnvironment.MapPath("~/Images/");
            //            String[] headerValues = (String[])Request.Headers.GetValues("UniqueId");
            //            String fileName = headerValues[0] + ".jpg";
            //            String fullPath = Path.Combine(filePath, fileName);
            //            image.Save(fullPath);
            //        }
            //    });
            //    return result;
            //}
            //else
            //{
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            //}
        }
    } 
}