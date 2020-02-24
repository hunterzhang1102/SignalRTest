using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebTest.Util
{
    public class HttpHelper
    {
        public static string Upload(UploadParameterType parameter)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // 1.分界线
                string boundary = string.Format("----{0}", DateTime.Now.Ticks.ToString("x")),       // 分界线可以自定义参数
                    beginBoundary = string.Format("--{0}\r\n", boundary),
                    endBoundary = string.Format("\r\n--{0}--\r\n", boundary);
                byte[] beginBoundaryBytes = parameter.Encoding.GetBytes(beginBoundary),
                    endBoundaryBytes = parameter.Encoding.GetBytes(endBoundary);

                // 2.组装开始分界线数据体 到内存流中
                memoryStream.Write(beginBoundaryBytes, 0, beginBoundaryBytes.Length);

                // 3.组装 上传文件附加携带的参数 到内存流中
                if (parameter.PostParameters != null && parameter.PostParameters.Count > 0)
                {
                    foreach (KeyValuePair<string, string> keyValuePair in parameter.PostParameters)
                    {
                        string parameterHeaderTemplate = string.Format(
                            "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n{2}", 
                            keyValuePair.Key, keyValuePair.Value, beginBoundary);
                        byte[] parameterHeaderBytes = parameter.Encoding.GetBytes(parameterHeaderTemplate);
                        memoryStream.Write(parameterHeaderBytes, 0, parameterHeaderBytes.Length);
                    }
                }

                // 4.组装文件头数据体 到内存流中
                string fileHeaderTemplate = string
                    .Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n"
                    , parameter.FileNameKey, parameter.FileNameValue);
                byte[] fileHeaderBytes = parameter.Encoding.GetBytes(fileHeaderTemplate);
                memoryStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

                // 5.组装文件流 到内存流中
                byte[] buffer = new byte[1024 * 1024 * 1];
                int size = parameter.UploadStream.Read(buffer, 0, buffer.Length);
                while (size > 0)
                {
                    memoryStream.Write(buffer, 0, size);
                    size = parameter.UploadStream.Read(buffer, 0, buffer.Length);
                }

                // 6.组装结束分界线数据体 到内存流中
                memoryStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

                // 7.获取二进制数据
                byte[] postBytes = memoryStream.ToArray();

                // 8.HttpWebRequest 组装
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(parameter.Url, UriKind.RelativeOrAbsolute));
                webRequest.Headers.Add("token", parameter.Token);
                webRequest.Method = "POST";
                webRequest.Timeout = 10000;
                webRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                webRequest.ContentLength = postBytes.Length;
                webRequest.KeepAlive = true;

                //if (Regex.IsMatch(parameter.Url, "^https://"))
                //{
                //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                //    ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                //}

                // 9.写入上传请求数据
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                }

                // 10.获取响应
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), parameter.Encoding))
                    {
                        string body = reader.ReadToEnd();
                        reader.Close();
                        return body;
                    }
                }
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static string HttpPost(string url, string json, string token)
        {
            string responseText = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json;charset=UTF-8";
                request.Headers.Add("token", token);
                request.Method = "POST";
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                    sw.Flush();
                    sw.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseText = myreader.ReadToEnd();
                myreader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseText;
        }

        public static string HttpPostFile(string url, byte[] bytes, string token)
        {
            string responseText = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("token", token);
                request.Method = "POST";
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseText = myreader.ReadToEnd();
                myreader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseText;
        }
    }
}
