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
        }

        public void SetSource(string url)
        {
            HtmlWebView.Source = url;
        }
    }
}