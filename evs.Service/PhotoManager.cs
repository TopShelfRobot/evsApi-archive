//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Net.Http;
//using System.IO;
//using evs.Model;



//namespace evs.Service
//{
//    class PhotoManager : IPhotoManager
//    {

//        private string workingFolder { get; set; }

//        public PhotoManager()
//        {

//        }

//        public PhotoManager(string workingFolder)
//        {
//            this.workingFolder = workingFolder;
//            CheckTargetDirectory();
//        }

//        public async Task<IEnumerable<PhotoViewModel> Get()
//        {
//            List<PhotoViewModel> photos = new List<PhotoViewModel>();

//            DirectoryInfo photoFolder = new DirectoryInfo(this.workingFolder);

//            await Task.Factory.StartNew(() =>
//            {
//                photos = photoFolder.EnumerateFiles()
//                                            .Where(fi => new[] { ".jpg", ".bmp", ".png", ".gif", ".tiff" }.Contains(fi.Extension.ToLower()))
//                                            .Select(fi => new PhotoViewModel
//                                            {
//                                                Name = fi.Name,
//                                                Created = fi.CreationTime,
//                                                Modified = fi.LastWriteTime,
//                                                Size = fi.Length / 1024
//                                            })
//                                            .ToList();
//            });

//            return photos;
//        }

//        public async Task<PhotoActionResult> Delete(string fileName)
//        {
//            try
//            {
//                var filePath = Directory.GetFiles(this.workingFolder, fileName)
//                                .FirstOrDefault();

//                await Task.Factory.StartNew(() =>
//                {
//                    File.Delete(filePath);
//                });

//                return new PhotoActionResult { Successful = true, Message = fileName + "deleted successfully" };
//            }
//            catch (Exception ex)
//            {
//                return new PhotoActionResult { Successful = false, Message = "error deleting fileName " + ex.GetBaseException().Message };
//            }
//        }

//        public async Task<IEnumerable<PhotoViewModel>> Add(System.Net.Http.HttpRequestMessage request)
//        {
//            var provider = new PhotoMultipartFormDataStreamProvider(this.workingFolder);

//            await request.Content.ReadAsMultipartAsync(provider);

//            var photos = new List<PhotoViewModel>();

//            foreach (var file in provider.FileData)
//            {
//                var fileInfo = new FileInfo(file.LocalFileName);

//                photos.Add(new PhotoViewModel
//                {
//                    Name = fileInfo.Name,
//                    Created = fileInfo.CreationTime,
//                    Modified = fileInfo.LastWriteTime,
//                    Size = fileInfo.Length / 1024
//                });
//            }

//            return photos;
//        }

//        public bool FileExists(string fileName)
//        {
//            var file = Directory.GetFiles(this.workingFolder, fileName)
//                                .FirstOrDefault();

//            return file != null;
//        }

//        private void CheckTargetDirectory()
//        {
//            if (!Directory.Exists(this.workingFolder))
//            {
//                throw new ArgumentException("the destination path " + this.workingFolder + " could not be found");
//            }
//        }
//    }

//    public class PhotoMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
//    {

//        public PhotoMultipartFormDataStreamProvider(string path)
//            : base(path)
//        {
//        }

//        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
//        {
//            //Make the file name URL safe and then use it & is the only disallowed url character allowed in a windows filename
//            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
//            return name.Trim(new char[] { '"' })
//                        .Replace("&", "and");
//        }
//    }


//    public interface IPhotoManager
//    {
//        Task<IEnumerable<PhotoViewModel>> Get();
//        Task<PhotoActionResult> Delete(string fileName);
//        Task<IEnumerable<PhotoViewModel>> Add(HttpRequestMessage request);
//        bool FileExists(string fileName);
//    }


//    public class PhotoActionResult
//    {
//        public bool Successful { get; set; }
//        public string Message { get; set; }
//    }

//}
