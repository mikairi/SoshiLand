using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.IO;

// For Network
using System.Net;
using System.Xml;

using Newtonsoft.Json;
using SoshiLandSilverlight.GameData.JSON;

namespace SoshiLandSilverlight
{
    public static class Network
    {
        // This api_key is required for all requests
        private static string param_api_key = "api_key=ef05a0898f787292f5d5c14ebc661b22";

        public static string uriRoot = "http://soshiland.ssf.me/?" + param_api_key;
        public static string uriUsers = "http://soshiland.ssf.me/users?" + param_api_key;

        public static string currentResponse = "";
        public static string currentRequest = "";
        public static string currentResponseFromRequest = "";

        // POST request
        public static void RequestReady(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            Stream stream = request.EndGetRequestStream(result);

            // Send the post variables  
            StreamWriter writer = new StreamWriter(stream);

            // Write the request string
            writer.WriteLine(currentRequest);

            //Game1.debugMessageQueue.addMessageToQueue("Writing data: " + testUserText);

            writer.Flush();
            writer.Close();

            request.BeginGetResponse(new AsyncCallback(ResponseReady), request);
        }

        // Get the Result  
        public static void ResponseReady(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            // get the result text  
            string resultString = reader.ReadToEnd();

            currentResponseFromRequest = resultString;

            //Game1.debugMessageQueue.addMessageToQueue("Response: " + resultString);
        }

        public static void HttpResponseHandler(IAsyncResult result)
        {
            // acquire the result.
            HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;

            // acquire the feed response.
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(result);

            // load the response into a stream reader
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            // Convert stream into string
            currentResponse = streamReader.ReadToEnd();

            //User readData = JsonConvert.DeserializeObject<User>(text);

            //Game1.debugMessageQueue.addMessageToQueue(text);
        }

        public static string SurroundWithSquareBrackets(string s)
        {
            return "[" + s + "]";

        }
    }
}
