using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using WebTest.Model;
using WebTest.Repository;
using WebTest.Util;

namespace WebTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static string basurl = "https://localhost:5002";

        [HttpGet("GetUserInfo")]
        public ResponseResult<User> GetUserInfo()
        {
            ResponseResult<User> response = new ResponseResult<User>();
            response.Code = 200;
            response.Message = string.Empty;
            response.Data = new User()
            {
                Num = "T1524654",
                UserName = "Hunter"
            };
            return response;
        }

        [HttpPost("AddUser")]
        public ResponseResult<bool> AddUser()
        {
            UserEntity userEntity = new UserEntity()
            {
                name = "Tom"
            };
            using (FileStream stream = new FileStream("D:\\avator.png", FileMode.Open))
            {
                userEntity.avator = new byte[stream.Length];
                stream.Read(userEntity.avator);
            }
            new UserRepository().Insert(userEntity);
            ResponseResult<bool> response = new ResponseResult<bool>();
            response.Code = 200;
            response.Message = string.Empty;
            response.Data = true;
            return response;
        }

        [HttpPost("PostTeacherAvatar")]
        public ResponseResult<bool> PostTeacherAvatar(IFormFile formFile)
        {
            var file = Request.Form.Files[0];
            using (FileStream fs = new FileStream("D:\\avatorBlob_fromdb.png", FileMode.OpenOrCreate))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            ResponseResult<bool> response = new ResponseResult<bool>();
            response.Code = 200;
            response.Message = string.Empty;
            response.Data = true;
            return response;
        }

        [HttpGet("BlobToFile")]
        public ResponseResult<UserEntity> BlobToFile()
        {
            UserEntity userEntity = new UserRepository().GetUser("tom");
            using (FileStream fs = new FileStream("D:\\avatorBlob_fromdb.png", FileMode.OpenOrCreate))
            {
                fs.Write(userEntity.avator);
                fs.Flush();
            }
            ResponseResult<UserEntity> response = new ResponseResult<UserEntity>();
            response.Code = 200;
            response.Message = string.Empty;
            response.Data = userEntity;
            return response;
        }

        [HttpGet("FileCopy")]
        public ResponseResult<bool> FileCopy()
        {
            using (FileStream fs = new FileStream("D:\\avator.png", FileMode.Open))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes);

                using (FileStream fsInner = new FileStream("D:\\avatorBlob.png", FileMode.OpenOrCreate))
                {
                    fsInner.Write(bytes);
                    fsInner.Flush();
                }
            }

            ResponseResult<bool> response = new ResponseResult<bool>();
            response.Code = 200;
            response.Message = string.Empty;
            response.Data = true;
            return response;
        }

        [HttpGet("GetBDSImgUrlByBlob")]
        public ResponseResult<string> GetBDSImgUrlByBlob()
        {
            //string url = $"{basurl}/api/User/PostTeacherAvatar";
            //string path = "D:\\avator.png";
            //string json = Upload(url, path, string.Empty);

            UserEntity userEntity = new UserRepository().GetUser("tom");
            //byte[] bytes = null;
            //using (FileStream fs = new FileStream("D:\\avator.png", FileMode.Open))
            //{
            //    bytes = new byte[fs.Length];
            //    fs.Read(bytes);
            //}
            MemoryStream fileStream = new MemoryStream(userEntity.avator);
            string json = HttpHelper.Upload(new UploadParameterType
            {
                Url = $"{basurl}/api/User/PostTeacherAvatar",
                UploadStream = fileStream,
                FileNameValue = $"{userEntity.name}.png",
                Token = string.Empty
            });
            //string token = GetToken("86D4F4EF369A242AE375D6C773662683", "ED19395661480AB939BB52D37FB658");
            //string json = HttpHelper.HttpPostFile($"{basurl}/api/User/PostTeacherAvatar", bytes, token);
            ResultModel uploadResult = JsonConvert.DeserializeObject<ResultModel>(json);
            ResponseResult<string> response = new ResponseResult<string>();
            response.Code = 200;
            response.Message = uploadResult.msg;
            response.Data = (string)uploadResult.data;
            return response;
        }

        [NonAction]
        private string GetToken(string ak, string sk)
        {
            try
            {
                string url = basurl + "/api/Admin/Authentication";
                string json = "{\"ak\":\"" + ak + "\",\"sk\":\"" + sk + "\"}";
                string jsonstr = HttpHelper.HttpPost(url, json, string.Empty);
                ResultModel jsonobj = new ResultModel();
                jsonobj = JsonConvert.DeserializeObject<ResultModel>(jsonstr);
                if (jsonobj.code < 0)
                {
                    throw new Exception("认证失败，错误：" + jsonobj.msg);
                }
                else
                {
                    return jsonobj.data.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [NonAction]
        private static string Upload(string url, string fileName, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("token", token);
            request.Method = "POST";
            Stream postStream = new MemoryStream();
            #region 处理Form表单文件上传
            //通过表单上传文件
            string boundary = "----" + DateTime.Now.Ticks.ToString("x");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

            try
            {
                //准备文件流
                using (var fileStream = FileToStream(fileName))
                {
                    var formdata = string.Format(formdataTemplate, "", System.IO.Path.GetFileName(fileName) /*Path.GetFileName(fileName)*/);
                    var formdataBytes = Encoding.UTF8.GetBytes(postStream.Length == 0 ? formdata.Substring(2, formdata.Length - 2) : formdata);//第一行不需要换行
                    postStream.Write(formdataBytes, 0, formdataBytes.Length);

                    //写入文件
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        postStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //结尾
            var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            postStream.Write(footer, 0, footer.Length);
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

            #endregion

            request.ContentLength = postStream != null ? postStream.Length : 0;
            request.Accept = "*/*";
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

            #region 输入二进制流
            if (postStream != null)
            {
                postStream.Position = 0;

                //直接写入流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                postStream.Close();//关闭文件访问
            }
            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }

        [NonAction]
        private static Stream FileToStream(string fileName)
        {
            // 打开文件
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[]
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
