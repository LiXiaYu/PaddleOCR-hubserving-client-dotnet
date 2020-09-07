using System;


namespace PaddleOCR_hubserving_client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var ss = PaddleOCR.hubserving.Client.Recognition(@"http://127.0.0.1:8868/predict/ocr_system", @"I:\programing\CaseHistoryTranscribe\ocr\PaddleOCR\doc\imgs");
            foreach(var s in ss)
            {
                Console.WriteLine(s);
            }

        }
    }
}
