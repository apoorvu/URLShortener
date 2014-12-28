using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bit.Ly_URL_Shortner
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;
            bool isNetwork = NetworkInterface.GetIsNetworkAvailable();
            if(!isNetwork)
            {
                textBox1.Visibility=Visibility.Collapsed;
                button1.Visibility=Visibility.Collapsed;
                textBox2.Text = "No Internet Connectivity Found! Please Recheck";
            }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value ==
                Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                landscapeContent.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                snapViewContent.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Filled ||
                Windows.UI.ViewManagement.ApplicationView.Value ==
                Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape)
            {
                landscapeContent.Visibility = Windows.UI.Xaml.Visibility.Visible;
                snapViewContent.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        public static void AddSettingsCommands(SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            SettingsCommand privacyPref = new SettingsCommand("privacyPref", "Privacy Policy", (uiCommand) =>
                                                                                                   {
                                                                                                       Windows.System.
                                                                                                           Launcher.
                                                                                                           LaunchUriAsync
                                                                                                           (new Uri(
                                                                                                                "http://docsanjay1.blogspot.in/2013/06/windows-8-app-privacy-policy.html"));
                                                                                                   });

            args.Request.ApplicationCommands.Add(privacyPref);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += GroupedItemsPage_CommandsRequested;
        }

        private void GroupedItemsPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            MainPage.AddSettingsCommands(args);
        }

        public async Task<string> GetPhotosStream(string url1)
        {
            HttpClient client = new HttpClient();
            string url = string.Format("https://api-ssl.bitly.com/v3/shorten?login=apoorv001123456&apiKey=R_96c5153c6b9719031bbaf5503268f9b1&LongUrl={0}", url1);
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        public class Data
        {
            public string long_url { get; set; }
            public string url { get; set; }
            public string hash { get; set; }
            public string global_hash { get; set; }
            public int new_hash { get; set; }
        }

        public class RootObject
        {
            public int status_code { get; set; }
            public string status_txt { get; set; }
            public Data data { get; set; }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text))
            {
                MessageDialog mb=new MessageDialog("Don't Leave The Field Empty","Bit.Ly URL Shortner");
                mb.ShowAsync();
            }
            else     if (textBox1.Text.Contains("http://") || textBox1.Text.Contains("HTTP://"))
            {
                try
                {


                    string Text = WebUtility.UrlEncode(textBox1.Text.Trim());

                    string responseText = await GetPhotosStream(Text);

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (RootObject));
                    RootObject root;
                    using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(responseText)))
                    {
                        root = serializer.ReadObject(stream) as RootObject;

                        textBox2.Text ="The URL Which You Entered Was: "+ root.data.long_url +"\n"+"The Shortened URL Is:"
                                         + root.data.url;
                    }
                }
                  catch
                  {
                      MessageDialog mb=new MessageDialog("Are You entering Valid Stuff? Check Again Please","Bit.Ly URL Shortner");
                      mb.ShowAsync();
                  }
            }
            else
            {
                MessageDialog mb = new MessageDialog("Please Enter Valid URLs only staring with http:// or HTTP://",
                                                     "Bit.Ly URL Shortner");
                mb.ShowAsync();
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text))
            {
                MessageDialog mb=new MessageDialog("Dont Leave This Field Blank");
                mb.ShowAsync();
                textBox2.Text = string.Empty;
            }
            

        }
    }
}
