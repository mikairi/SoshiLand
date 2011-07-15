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
            // Store user's name in a string
            string userName = UserName.Text;

            // Switch to Chatroom
            App.ChangeGameState(GameState.ChatRoom);
        }

        private void UserName_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearTextBox((TextBox)sender);
        }

    }
}
