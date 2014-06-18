using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinRTClient
{
    public sealed partial class MainPage : Page
    {
        private readonly Auth _authenticator;

        public MainPage()
        {
            _authenticator = new Auth();
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var success = await _authenticator.Authenticate();

            if (success)
            {
                AuthText.Text = _authenticator.AuthenticatedUser.Profile["email"].ToString();
                UserProfile.Text = await _authenticator.GetUserProfile();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Authorization = await _authenticator.GetDelegatedToken();
            var apiResult = await apiClient.GetStringAsync("http://localhost:3000/api/values");

            ApiText.Text = apiResult;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _authenticator.Logout();
            AuthText.Text = "";
            UserProfile.Text = "";
            ApiText.Text = "";
        }
    }
}
