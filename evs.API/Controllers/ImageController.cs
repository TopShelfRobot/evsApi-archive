using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using evs.Service;
using evs.Model;
using Owin;
using System.IO;


namespace evs.API.Controllers
{
    [RoutePrefix("api/image")]
    public class ImageController : ApiController
    {
        public Task<IEnumerable<FileDesc>> Post()
        {
            try
            {
                var folderName = "Images";
                var PATH = HttpContext.Current.Server.MapPath("/Content/" + folderName);
                var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
                if (Request.Content.IsMimeMultipartContent())
                {
                    var streamProvider = new CustomMultipartFormDataStreamProvider(PATH);
                    var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<IEnumerable<FileDesc>>(t =>
                    {

                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                        }

                        var fileInfo = streamProvider.FileData.Select(i =>
                        {
                            var info = new FileInfo(i.LocalFileName);
                            return new FileDesc(info.Name, rootUrl + "/" + folderName + "/" + info.Name, info.Length / 1024);
                        });
                        return fileInfo;
                    });

                    return task;
                }
                else
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
                }
            }
            catch (Exception ex)
            {
                string errrr = ex.Message + "  --  " + ex.InnerException;
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            }
        }



        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path)
                : base(path)
            { }

            public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
            {
                var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
                return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
            }
        }
        public class FileDesc
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public long Size { get; set; }

            public FileDesc(string n, string p, long s)
            {
                Name = n;
                Path = p;
                Size = s;
            }
        }
        //public async Task<IEnumerable<string>> PostMultipartStream()
        //   {
        //      // Check we're uploading a file
        //      if (!Request.Content.IsMimeMultipartContent("form-data"))            
        //          throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

        //    // Create the stream provider, and tell it sort files in my c:\temp\uploads folder
        //     var provider = new MultipartFormDataStreamProvider("c:\\temp\\uploads");

        //     // Read using the stream
        //     var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);            

        //    // Create response.
        //     return Ok;
        //      //return provider.BodyPartFileNames.Select(kv => kv.Value);            
        //  }






        //[RoutePrefix("api/image")]
        //private IPhotoManager photoManager;

        ////public PhotoController()
        ////    : this(new PhotoManager(HttpRuntime.AppDomainAppPath + @"\Album"))
        ////{            
        ////}

        //public PhotoController(IPhotoManager photoManager)
        //{
        //    this.photoManager = photoManager;
        //}

        //// GET: api/Photo
        //public async Task<IHttpActionResult> Get()
        //{
        //    var results = await photoManager.Get();
        //    return Ok(new { photos = results });
        //}

        //// POST: api/Photo
        //public async Task<IHttpActionResult> Post()
        //{
        //    // Check if the request contains multipart/form-data.
        //    if(!Request.Content.IsMimeMultipartContent("form-data"))
        //    {
        //        return BadRequest("Unsupported media type");
        //    }

        //    try
        //    {
        //        var photos = await photoManager.Add(Request);
        //        return Ok(new { Message = "Photos uploaded ok", Photos = photos });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.GetBaseException().Message);
        //    } 
        //}

        // DELETE: api/Photo/5
        //[HttpDelete]
        //[Route("{fileName}")]
        //public async Task<IHttpActionResult> Delete(string fileName)
        //{         
        //    if (!this.photoManager.FileExists(fileName))
        //    {
        //        return NotFound();
        //    }

        //   var result = await this.photoManager.Delete(fileName);

        //   if (result.Successful)
        //   {
        //       return Ok(new { message = result.Message});
        //   } else
        //   {
        //       return BadRequest(result.Message);
        //   }
        //}
    }

}
