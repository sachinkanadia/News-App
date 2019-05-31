using Microsoft.Toolkit.Parsers.Rss;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System;

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

            Refresh();
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
                _SelectedArticle = value;
                OnPropertyChanged(nameof(SelectedArticle));

                if (_SelectedArticle == null)
                    return;

                _NewsArticleView.SetSource(_SelectedArticle.FeedUrl);
                this.Navigation.PushAsync(_NewsArticleView, true);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ArticleListView.SelectedItems.Clear();
        }

        private async void Refresh()
        {
            var articles = await Parse("https://feeds.bbci.co.uk/news/uk/rss.xml");

            articles = articles.OrderByDescending(rssSchema => rssSchema.PublishDate);

            Articles.Clear();

            foreach (var article in articles)
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

        private void RefreshMenuItem_Clicked(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ArticleListView.DataSource != null)
            {
                this.ArticleListView.DataSource.Filter = FilterArticles;
                this.ArticleListView.DataSource.RefreshFilter();
            }
        }

        private bool FilterArticles(object obj)
        {
            if (FilterText == null || FilterText.Text == null)
                return true;

            var article = obj as RssSchema;

            if (article.Title.ToLower().Contains(FilterText.Text.ToLower())
                 || article.Title.ToLower().Contains(FilterText.Text.ToLower()))
                return true;
            else
                return false;
        }
    }
}
