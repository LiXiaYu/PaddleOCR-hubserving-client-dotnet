using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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


        public static string Url { get; set; }

        /// <summary>
        /// 百度云文字识别风格的识别
        /// </summary>
        /// <param name="image">待识别图片文件的二进制格式</param>
        /// <param name="options">选项（暂时不支持）</param>
        /// <returns></returns>
        public static JObject Identify(byte[] image,Dictionary<string,object> options=null)
        {
            JObject result=new JObject();

            JObject jObject = new JObject();
            jObject.Add("images", new JArray { Cv2ToBase64(image) });
            string json = jObject.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Client.Url);
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

            JObject retJson = JObject.Parse(retString);

            JArray rArray = new JArray();

            int num_rword = 0;
            foreach(var r in retJson["results"][0])
            {
                JObject l = new JObject();

                var ll = r["text_region"];
                var ll_rs = new int[] { ll[0][0].ToObject<int>(), ll[1][0].ToObject<int>(), ll[2][0].ToObject<int>(), ll[3][0].ToObject<int>() };
                var ll_cs = new int[] { ll[0][1].ToObject<int>(), ll[1][1].ToObject<int>(), ll[2][1].ToObject<int>(), ll[3][1].ToObject<int>() };
                
                //注意：PaddleOCR是菱形，跟百度云的长方形不一样，这里目前是简单处理，选取了四极
                l.Add("left", ll_cs.Min());
                l.Add("top", ll_rs.Min());
                l.Add("width",ll_cs.Max()-ll_cs.Min());
                l.Add("height", ll_rs.Max() - ll_rs.Min());

                JObject rword = new JObject();
                rword.Add("location", l);
                rword.Add("words", r["text"].ToString());

                rArray.Add(rword);

                num_rword++;
            }

            result.Add("words_result_num", num_rword);
            result.Add("words_result",rArray);

            return result;
        }
    }
}
