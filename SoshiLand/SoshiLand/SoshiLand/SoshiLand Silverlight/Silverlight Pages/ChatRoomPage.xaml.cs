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
namespace SoshiLandSilverlight
{
    public partial class ChatRoom : Page
    {
        public ChatRoom()
        {
            InitializeComponent();
            ListOfCurrentPlayers.Text = "";
            updateListOfPlayers();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeGameState(GameState.InGame);
        }

        public void updateListOfPlayers()
        {
            string uriRequest = "http://daum.heroku.com/soshi";

            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(uriRequest));
            httpRequest.BeginGetResponse(new AsyncCallback(Network.HttpResponseHandler), httpRequest);

            string jsonListOfPlayers = Network.currentResponse;
            jsonListOfPlayers = Network.SurroundWithSquareBrackets(jsonListOfPlayers);
            if (jsonListOfPlayers != "")
            {
                List<PlayerJson> listOfPlayers = JsonConvert.DeserializeObject<List<PlayerJson>>(jsonListOfPlayers);

                ListOfCurrentPlayers.Text = "";             // Clear Text first

                // Iterate through the string to add 
                foreach (PlayerJson p in listOfPlayers)
                {
                    ListOfCurrentPlayers.Text += p.Name + System.Environment.NewLine;
                }
                
            }
        }

        private void startGame_MouseEnter(object sender, MouseEventArgs e)
        {
            updateListOfPlayers();
        }

    }
}
