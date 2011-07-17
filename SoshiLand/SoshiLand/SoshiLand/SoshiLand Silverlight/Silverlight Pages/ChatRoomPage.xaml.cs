using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using SoshiLandSilverlight;
using Newtonsoft.Json;
using SoshiLandSilverlight.GameData.JSON;

using System.Windows.Threading;

namespace SoshiLandSilverlight
{
    public partial class ChatRoom : Page
    {
        // Timer for polling
        private DispatcherTimer timer;
        public static string[] chatroomListOfPlayers;
        private List<PlayerJson> listOfPlayers;

        public ChatRoom()
        {
            InitializeComponent();
            
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);  // 1 second interval
            timer.Tick += new EventHandler(updateListOfPlayers);
            timer.Start();

            ListOfCurrentPlayers.Text = "";
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            
            chatroomListOfPlayers = new string[listOfPlayers.Count];

            for (int i = 0; i < listOfPlayers.Count; i++)
            {
                chatroomListOfPlayers[i] = listOfPlayers[i].user_name;
            }
            
            // Temporary
            /*
            chatroomListOfPlayers = new string[2];
            chatroomListOfPlayers[0] = "Mark";
            chatroomListOfPlayers[1] = "Mako";
            */
            App.ChangeGameState(GameState.InGame);
            

        }

        public void updateListOfPlayers(object o, EventArgs sender)
        {
            // URI for list of players
            string uriRequest = Network.uriUsers;

            // Send the GET request for the JSON of players
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(uriRequest));
            httpRequest.BeginGetResponse(new AsyncCallback(Network.HttpResponseHandler), httpRequest);

            // Store the response in a string
            string jsonListOfPlayers = Network.currentResponse;
            // Add square brackets so the string can be deserialized
            //jsonListOfPlayers = Network.SurroundWithSquareBrackets(jsonListOfPlayers);
            if (jsonListOfPlayers != "")
            {
                // Deserialize into a list
                listOfPlayers = JsonConvert.DeserializeObject<List<PlayerJson>>(jsonListOfPlayers);
                
                // Clear Text first
                ListOfCurrentPlayers.Text = "";             

                // Iterate through the string to add 
                foreach (PlayerJson p in listOfPlayers)
                {
                    // Add the player's name line by line
                    ListOfCurrentPlayers.Text += p.user_name + System.Environment.NewLine;
                }
            }
        }
    }
}
