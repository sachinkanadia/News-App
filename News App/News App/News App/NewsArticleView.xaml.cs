using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace News_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsArticleView : ContentPage
    {
        public NewsArticleView()
        {
            InitializeComponent();

            ShareCommand = new Command(ShareLink);
            HtmlWebView.Navigated += HtmlWebView_Navigated;
        }

        private string _CurrentURL;

        private void HtmlWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            _CurrentURL = e.Url;
        }

        /// <summary>
        /// Command not working so I've used the event handler
        /// 'ShareMenuItem_Clicked' below instead.
        /// </summary>
        public ICommand ShareCommand
        {
            get; private set;
        }

        private void ShareLink()
        {
            if (string.IsNullOrEmpty(_CurrentURL))
                return;

            Share.RequestAsync(new ShareTextRequest
            {
                Uri = _CurrentURL,
                Title = "Share Article"
            });
        }

        public void SetSource(string url)
        {
            HtmlWebView.Source = url;
        }

        private void ShareMenuItem_Clicked(object sender, System.EventArgs e)
        {
            ShareLink();
        }
    }
}