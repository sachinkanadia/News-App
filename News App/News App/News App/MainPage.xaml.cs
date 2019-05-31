using Microsoft.Toolkit.Parsers.Rss;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Windows.Input;
using Syncfusion.ListView.XForms;

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

            RefreshCommand = new Command(Refresh, CanRefresh);
            //Don't know why this is needed, but doesn't work without it!
            OnPropertyChanged(nameof(RefreshCommand));

            FeedPicker.ItemsSource = _Feeds.Keys.ToArray();
            FeedPicker.Title = "Select Your Feed";
            FeedPicker.SetBinding(Picker.SelectedItemProperty, nameof(SelectedFeed));
        }

        private string _SelectedFeed;
        private readonly NewsArticleView _NewsArticleView = new NewsArticleView();
        private readonly ObservableCollection<RssSchema> _Articles = new ObservableCollection<RssSchema>();
        private readonly Dictionary<string, string> _Feeds = new Dictionary<string, string>
        {
            {"BBC News UK", " https://feeds.bbci.co.uk/news/uk/rss.xml"},
            {"BBC News Technology", "https://feeds.bbci.co.uk/news/technology/rss.xml"},
            {"Reuters UK", "http://feeds.reuters.com/reuters/UKTopNews"},
            {"Reuters Technology", "http://feeds.reuters.com/reuters/technologyNews"}
        };

        public string SelectedFeed
        {
            get
            {
                return _SelectedFeed;
            }
            set
            {
                if (value == _SelectedFeed)
                    return;

                _SelectedFeed = value;

                OnPropertyChanged(nameof(SelectedFeed));

                ((Command)RefreshCommand).ChangeCanExecute();

                Refresh();
            }
        }

        public ICommand RefreshCommand
        {
            get; private set;
        }

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

        private void SetIsBusy(bool isBusy)
        {
            IsBusy = isBusy;
            ((Command)RefreshCommand).ChangeCanExecute();

            if (IsBusy)
            {
                ArticleListView.RemoveBinding(SfListView.SelectedItemProperty);
            }
            else
            {
                ArticleListView.SetBinding(SfListView.SelectedItemProperty, nameof(SelectedArticle));
            }
        }

        private async void Refresh()
        {
            SetIsBusy(true);

            try
            {
                var url = _Feeds[(string)FeedPicker.SelectedItem];

                var articles = await Parse(url);

                articles = articles.OrderByDescending(rssSchema => rssSchema.PublishDate);

                Articles.Clear();

                foreach (var article in articles)
                {
                    Articles.Add(article);
                }
            }
            finally
            {
                SetIsBusy(false);
            }
        }

        private bool CanRefresh()
        {
            return !IsBusy && !string.IsNullOrEmpty(SelectedFeed);
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