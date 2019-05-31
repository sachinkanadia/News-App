using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
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
            //Don't know why this is needed, but doesn't work without it!
            OnPropertyChanged(nameof(ShareCommand));

            DownloadCommand = new Command(DownloadLink);
            //Don't know why this is needed, but doesn't work without it!
            OnPropertyChanged(nameof(DownloadCommand));

            HtmlWebView.Navigated += HtmlWebView_Navigated;
        }

        private string _CurrentURL;
        private string _DownloadStatus;
        private IDownloadFile _DownloadFile;
        private bool _IsDownloading;
        private object _Locker = new object();

        private void HtmlWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            _CurrentURL = e.Url;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DownloadStatus = null;
        }

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

        public ICommand DownloadCommand
        {
            get; private set;
        }

        private void DownloadLink()
        {
            if (string.IsNullOrEmpty(_CurrentURL))
                return;

            if (_IsDownloading)
                return;

            lock (_Locker)
            {
                if (_IsDownloading)
                    return;

                _IsDownloading = true;
                var downloadManager = CrossDownloadManager.Current;
                _DownloadFile = downloadManager.CreateDownloadFile(_CurrentURL);
                _DownloadFile.PropertyChanged += _DownloadFile_PropertyChanged;
                downloadManager.Start(_DownloadFile);
            }
        }

        private void _DownloadFile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.ToUpperInvariant().Equals("STATUS"))
            {
                DownloadStatus = _DownloadFile.Status.ToString();

                if(_DownloadFile.Status == DownloadFileStatus.COMPLETED 
                    || _DownloadFile.Status == DownloadFileStatus.CANCELED 
                    || _DownloadFile.Status == DownloadFileStatus.FAILED)
                {
                    _IsDownloading = false;
                }
            }
        }

        public string DownloadStatus
        {
            get
            {
                return _DownloadStatus;
            }
            set
            {
                if (_DownloadStatus == value)
                    return;

                _DownloadStatus = value;
                OnPropertyChanged(nameof(DownloadStatus));
            }
        }
    }
}