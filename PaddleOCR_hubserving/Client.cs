using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PaddleOCR.hubserving
{
    public static class Client
    {
        public static string Cv2ToBase64(byte[] image)
        {
            return Convert.ToBase64String(image);
        }
        public static List<string> Recognition(string url, string image_path)
        {
            List<string> results = new List<string>();
            string[] image_file_list = Directory.GetFiles(image_path);

            var is_visualize = false;

            var cnt = 0;
            var total_time = 0;
            foreach (var image_file in image_file_list)
            {
                var img = File.ReadAllBytes(image_file);

                //发送HTTP请求
                //var starttime = time.time();

                JObject jObject = new JObject();
                jObject.Add("images", new JArray { Cv2ToBase64(img) });
                string json=jObject.ToString();

                byte[] bytes = Encoding.UTF8.GetBytes(json);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();


                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }

                JObject retJson= JObject.Parse(retString);

                var res = retJson["results"][0].ToString();
                results.Add(res);
            }

            return results;
        }
    }
}
