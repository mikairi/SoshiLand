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

using Newtonsoft.Json;
using SoshiLandSilverlight.GameData.JSON;

namespace SoshiLandSilverlight
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ClearTextBox(TextBox textBox)
        {
            textBox.Text = "";
        }

        void enterUserName_Click(object sender, RoutedEventArgs e)
        {
            string uri = "http://daum.heroku.com/soshi";

            // Store user's name in a string
            string userName = UserName.Text;

            PlayerJson user = new PlayerJson();
            user.BoardPosition = 10;
            user.Money = 1500;
            user.Name = userName;

            string testUserText = JsonConvert.SerializeObject(user);

            Network.currentRequest = testUserText;

            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
            httpRequest.Method = "POST";
            httpRequest.BeginGetRequestStream(new AsyncCallback(Network.RequestReady), httpRequest);

            // Switch to Chatroom
            App.ChangeGameState(GameState.ChatRoom);
        }

        private void UserName_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearTextBox((TextBox)sender);
        }

    }
}
