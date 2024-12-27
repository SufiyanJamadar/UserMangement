using LearnApi.Helper;
using LearnApi.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly LearndataContext context;
        public ProductController(IWebHostEnvironment environment, LearndataContext context)
        {
            this.environment = environment;
            this.context= context;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formfile, string productcode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilePath(productcode);
                if (!System.IO.File.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }

                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formfile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                string Filepath = GetFilePath(productcode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                foreach (var file in filecollection)
                {
                    string imagepath = Filepath + "\\" + file.FileName;

                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;

                    }

                }

            }
            catch (Exception ex)
            {
                errorcount++;
                response.Message = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = $"{passcount} Files Uploaded & {errorcount} Files Failed";
            return Ok(response);
        }


        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productcode)
        {
            string ImageUrl = string.Empty;
            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    ImageUrl = hostUrl + "Upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }

            }
            catch(Exception ex)
            {

            }
            return Ok(ImageUrl);
        }

        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string productcode)
        {
            List<string> ImageUrl = new List<String>();
            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string Filepath = GetFilePath(productcode);

                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileinfos=directoryInfo.GetFiles();
                    foreach(FileInfo fileinfo in fileinfos)
                    {
                        string filename=fileinfo.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))
                        {
                           string _ImageUrl = hostUrl + "Upload/product/" + productcode + "/" + filename;
                            ImageUrl.Add(_ImageUrl);
                        }
                    }
                }
   
            }
            catch (Exception ex)
            {

            }
            return Ok(ImageUrl);
        }




        [HttpGet("download")]
        public async Task<IActionResult> download(string productcode)
        {
            // string ImageUrl = string.Empty;
            // string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream=new MemoryStream();
                    using(FileStream fileStream=new FileStream(imagepath,FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productcode + ".png");
                    //ImageUrl = hostUrl + "Upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }
            
        }


        [HttpGet("remove")]
        public async Task<IActionResult> remove(string productcode)
        {
            // string ImageUrl = string.Empty;
            // string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [HttpDelete("multiremove")]
        public async Task<IActionResult> multiremove(string productcode)
        {
            // string ImageUrl = string.Empty;
            // string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string Filepath = GetFilePath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileinfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileinfo in fileinfos)
                    {
                        fileinfo.Delete();
                    }
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }


        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                foreach (var file in filecollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        this.context.TblProductimages.Add(new Repos.Models.TblProductimage()
                        { 
                            Productcode = productcode,
                            Productimage=stream.ToArray()
                        });
                        await context.SaveChangesAsync();
                        passcount++;
                    }
                }

            }
            catch (Exception ex)
            {
                errorcount++;
                response.Message = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = $"{passcount} Files Uploaded & {errorcount} Files Failed";
            return Ok(response);
        }

        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string productcode)
        {
            List<string> ImageUrl = new List<String>();
            //string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                var _productImage = this.context.TblProductimages.Where(item => item.Productcode == productcode).ToList();
                 if(_productImage!=null && _productImage.Count > 0)
                 {
                    _productImage.ForEach(item =>
                    {
                        ImageUrl.Add(Convert.ToBase64String(item.Productimage));
                    });
                 }
                else
                {
                    return NotFound();
                }
                
            }
            catch (Exception ex)
            {

            }
            return Ok(ImageUrl);
        }


        [HttpGet("dbdownload")]
        public async Task<IActionResult> dbdownload(string productcode)
        {
           
            try
            {

                var _productImage = await this.context.TblProductimages.FirstOrDefaultAsync(item => item.Productcode == productcode);
                if (_productImage != null)
                {
                    return File(_productImage.Productimage, "image/png", productcode + ".png");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [NonAction]
        private string GetFilePath(string productcode)
        {
            return this.environment.WebRootPath + "\\Upload\\product\\" + productcode;
        }
    }
}



