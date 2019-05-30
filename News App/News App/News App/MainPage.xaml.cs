using Microsoft.Toolkit.Parsers.Rss;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace News_App
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private readonly NewsArticleView _NewsArticleView = new NewsArticleView();
        private readonly ObservableCollection<RssSchema> _Articles = new ObservableCollection<RssSchema>();

        public ObservableCollection<RssSchema> Articles
        {
            get { return _Articles; }
        }

        private RssSchema _SelectedArticle;

        public RssSchema SelectedArticle
        {
            get
            {
                return _SelectedArticle;
            }
            set
            {
                if (_SelectedArticle == value)
                    return;

                _SelectedArticle = value;
                OnPropertyChanged(nameof(SelectedArticle));

                _NewsArticleView.SetSource(_SelectedArticle.FeedUrl);
                this.Navigation.PushAsync(_NewsArticleView, true);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var articles = await Parse("https://feeds.bbci.co.uk/news/uk/rss.xml");
            
            foreach(var article in articles)
            {
                Articles.Add(article);
            }
        }

        public async Task<IEnumerable<RssSchema>> Parse(string url)
        {
            string feed = null;

            using (var client = new HttpClient())
            {
                feed = await client.GetStringAsync(url);
            }

            if (feed == null) return new List<RssSchema>();

            var parser = new RssParser();
            var rss = parser.Parse(feed);
            return rss;
        }
    }
}
