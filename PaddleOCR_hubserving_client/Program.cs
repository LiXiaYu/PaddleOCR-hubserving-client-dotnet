using System;
using System.IO;

namespace PaddleOCR_hubserving_client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //var ss = PaddleOCR.hubserving.Client.Recognition(@"http://127.0.0.1:8868/predict/ocr_system", @"I:\programing\CaseHistoryTranscribe\ocr\PaddleOCR\doc\imgs");
            //foreach(var s in ss)
            //{
            //    Console.WriteLine(s);
            //}

            var image= File.ReadAllBytes(@"I:\programing\CaseHistoryTranscribe\ocr\PaddleOCR\doc\imgs\1.jpg");
            PaddleOCR.hubserving.Client.Url = @"http://127.0.0.1:8868/predict/ocr_system";
            var res=PaddleOCR.hubserving.Client.Identify(image);

            return;
        }
    }
}
