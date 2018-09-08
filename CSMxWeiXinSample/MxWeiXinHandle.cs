using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using RestSharp;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace CSMxWeiXinSample
{
    public class MxWeiXinHandle
    {
        //验证身份
        public static void Valid()
        {
            string signature = HttpContext.Current.Request["signature"];
            string timestamp = HttpContext.Current.Request["timestamp"];
            string nonce = HttpContext.Current.Request["nonce"];
            string echostr = HttpContext.Current.Request["echostr"];

            if (HttpContext.Current.Request.HttpMethod == "GET")
            {
                if (CheckSignature(signature, timestamp, nonce))
                {
                    HttpContext.Current.Response.Output.Write(echostr);
                }
                else
                {
                    HttpContext.Current.Response.Output.Write("Failed valid");
                }
                HttpContext.Current.Response.End();
            }
        }

        //回复消息
        public static void ResponseMsg()
        {
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                try
                {
                    string postString = string.Empty;
                    using (Stream stream = HttpContext.Current.Request.InputStream)
                    {
                        Byte[] postBytes = new Byte[stream.Length];
                        stream.Read(postBytes, 0, (Int32)stream.Length);
                        postString = Encoding.UTF8.GetString(postBytes);
                    }
                    Hashtable postObj = ParseXml(postString);
                    string fromUsername = postObj["FromUserName"].ToString();
                    string toUsername = postObj["ToUserName"].ToString();
                    string keyword = postObj["Content"].ToString();
                    if (!String.IsNullOrEmpty(keyword))
                    {

                        //这里回复 前面几个都不要动
                        String responseContent = string.Format(Message_Text,fromUsername, toUsername, DateTime.Now.Ticks,
                            "欢迎来到佳蒂亚树Gattia Su"
                            + "\r\n<a href=\"https://music.163.com/#/program?id=799680534\">点击欣赏佳蒂亚树主题曲</a>")
                            +"\r\n <a href=\" \">体验Syli Custom Voice Model，请点击这里</a>";
                        //String responseContent = string.Format(Message_Text, fromUsername, toUsername, DateTime.Now.Ticks, SendAIMsg(keyword));

                        HttpContext.Current.Response.Write(responseContent);


                    }
                    else
                    {
                        HttpContext.Current.Response.Write("Input something...");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
        }

        //检验签名
        private static bool CheckSignature(String signature, String timestamp, String nonce)
        {
            String[] arr = new String[] { ConfigurationManager.AppSettings["TOKEN"].ToString(), timestamp, nonce };
            Array.Sort<String>(arr);
            StringBuilder content = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                content.Append(arr[i]);
            }
            String tmpStr = SHA1_Encrypt(content.ToString());
            return tmpStr != null ? tmpStr.Equals(signature) : false;
        }

        //SHA1
        private static string SHA1_Encrypt(string Source_String)
        {
            byte[] StrRes = Encoding.Default.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        //解析XML
        private static Hashtable ParseXml(String xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            XmlNode bodyNode = xmlDocument.ChildNodes[0];
            Hashtable ht = new Hashtable();
            if (bodyNode.ChildNodes.Count > 0)
            {
                foreach (XmlNode xn in bodyNode.ChildNodes)
                {
                    ht.Add(xn.Name, xn.InnerText);
                }
            }
            return ht;
        }

        private static string Message_Text
        {
            get { return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                           </xml>"; }
        }


        //发送Bot消息
        private static string SendAIMsg(string question)
        {
            var client = new RestClient("https://grapebot.azurewebsites.net/qnamaker/knowledgebases/84376fd5-512b-498c-bbdb-cce159efdc43/generateAnswer");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "EndpointKey 70df815a-7934-4d9e-9134-623bdd26d618");
            request.AddHeader("Content-Type", "application/json");
            string s = ("{\"question\":\"" + question + "\"}");


            request.AddParameter("undefined", s, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            QnAMakerResult Ansresponse;
            try
            {
                Ansresponse = JsonConvert.DeserializeObject<QnAMakerResult>(response.Content);
                return Ansresponse.Answers[0].AnswerAnswer;

            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

        }



        public void playCRIS()
        {

            var client = new RestClient("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "26c47b9e-82ee-f6cb-4bb0-9d3d6020dad1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-length", "0");
            request.AddHeader("ocp-apim-subscription-key", "376f7e9f102e453186e79fcac3f878fa");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            IRestResponse response = client.Execute(request);



            //var request1 = new RestRequest(Method.POST);
            //request.AddHeader("postman-token", "af76cb09-c273-fa5d-1be6-9fb79db7c15c");
            //request.AddHeader("cache-control", "no-cache");
            //request.AddHeader("content-type", "application/ssml+xml");
            //request.AddHeader("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");
            //request.AddHeader("authorization", "Bearer "+ );
            //request.AddHeader("content-length", "199");
            //request.AddParameter("application/ssml+xml", "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">\r\n<voice  name=\"Gattia_VoiceModel\">\r\n    hello world\r\n</voice> </speak>", ParameterType.RequestBody);
            //IRestResponse response1 = client1.Execute(request1);

            string url = "https://westus.voice.speech.microsoft.com/cognitiveservices/v1?deploymentId=399100a0-f981-4051-aefe-ac3f7299ad30";
            string tokenString = "Bearer " + response.Content.ToString();

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/ssml+xml";
            webRequest.Headers.Add("X-MICROSOFT-OutputFormat", "riff-16khz-16bit-mono-pcm");
            webRequest.Headers["Authorization"] = tokenString;
            webRequest.Timeout = 6000000;
            webRequest.Method = "POST";


            //StreamReader sr = new StreamReader(@"D:\testcode\ssml_customer.txt");
            string ssml = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice  name=\"Gattia_VoiceModel\">" + "hellow" + "</voice> </speak>"; //sr.ReadToEnd();

            byte[] btBodys = Encoding.UTF8.GetBytes(ssml);
            webRequest.ContentLength = btBodys.Length;
            webRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            WebResponse httpWebResponse = null;
            try
            {
                httpWebResponse = webRequest.GetResponse();

                Debug.WriteLine(httpWebResponse.GetType().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }


            //创建文件读取对象 
            using (Stream fileReader = httpWebResponse.GetResponseStream())
            {
                //创建文件写入对象

                using (FileStream fileWrite = new FileStream("./1.wav", FileMode.OpenOrCreate))
                {
                    //指定文件一次读取时的字节长度
                    byte[] by = new byte[1024 * 1024 * 10];
                    int count = 0;
                    while (true)
                    {
                        //将文件转换为二进制数据保存到内存中，同时返回读取字节的长度
                        count = fileReader.Read(by, 0, by.Length);
                        if (count == 0)//文件是否全部转换为二进制数据
                        {
                            break;
                        }
                        //将二进制数据转换为文件对象并保存到指定的物理路径中
                        fileWrite.Write(by, 0, count);
                    }
                    Debug.WriteLine("OK");
                }
            }


            //SoundPlayer player = new SoundPlayer(httpWebResponse.GetResponseStream());

            //player.Play();


            //Debug.WriteLine();
            //return View();
        }



    }


    public class QnAMakerResult
    {
        [JsonProperty("answers")]
        public Answer[] Answers { get; set; }
    }

    public class Answer
    {
        [JsonProperty("answer")]
        public string AnswerAnswer { get; set; }

        [JsonProperty("questions")]
        public string[] Questions { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }
}