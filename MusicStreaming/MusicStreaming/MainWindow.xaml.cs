using Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MusicStreaming;

namespace UserInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PLAYLISTS_DIRECTORY = "Playlists";

        public static readonly DependencyProperty FoundTracksProperty = DependencyProperty.Register(
            "FoundTracks", typeof(ObservableCollection<LocalTrack>), typeof(MainWindow));

        public static readonly DependencyProperty PlaylistsProperty = DependencyProperty.Register(
            "Playlists", typeof(ObservableCollection<string>), typeof(MainWindow));

        public static readonly DependencyProperty CurrentTrackProperty = DependencyProperty.Register(
            "CurrentTrack", typeof(LocalTrack), typeof(MainWindow), new PropertyMetadata(null, OnCurrentTrackChange));

        public static readonly DependencyProperty CurrentPlaylistProperty = DependencyProperty.Register(
            "CurrentPlaylist", typeof(ObservableCollection<LocalTrack>), typeof(MainWindow));

        /*public static readonly DependencyProperty CurrentPlaylistProperty = DependencyProperty.Register(
        "CurrentPlaylist", typeof(LocalPlaylist), typeof(MainWindow));*/

        public ObservableCollection<LocalTrack> FoundTracks
        {
            get { return (ObservableCollection<LocalTrack>)GetValue(FoundTracksProperty); }
            set { SetValue(FoundTracksProperty, value); }
        }

        public ObservableCollection<string> Playlists
        {
            get { return (ObservableCollection<string>)GetValue(PlaylistsProperty); }
            set { SetValue(PlaylistsProperty, value); }
        }

        public LocalTrack CurrentTrack
        {
            get { return (LocalTrack)GetValue(CurrentTrackProperty); }
            set { SetValue(CurrentTrackProperty, value); }
        }

        public ObservableCollection<LocalTrack> CurrentPlaylist
        {
            get { return (ObservableCollection<LocalTrack>)GetValue(CurrentPlaylistProperty); }
            set { SetValue(CurrentPlaylistProperty, value); }
        }

        /*public LocalPlaylist CurrentPlaylistTracks
        {
            get { return (LocalPlaylist)GetValue(CurrentPlaylistProperty); }
            set { SetValue(CurrentPlaylistProperty, value); }
        }*/

        public bool PlayingPlaylist { get; set; }
        public int CurrentPlaylistTrackIndex { get; set; }

        private StreamingSystemManager m_streamingSystemManager = StreamingSystemManager.GetInstance();

        public MainWindow()
        {
            FoundTracks = new ObservableCollection<LocalTrack>();
            InitializeComponent();
            DataContext = this;
            m_foundDataGrid.DataContext = this;
            m_trackSlider.DataContext = CurrentTrack;
            m_playlistControl.DataContext = this;
            m_playlistDataGrid.DataContext = this;
            EnableSearch();
            PlayingPlaylist = false;
            CurrentPlaylistTrackIndex = 0;

            if (!Directory.Exists(PLAYLISTS_DIRECTORY))
                Directory.CreateDirectory(PLAYLISTS_DIRECTORY);
            string[] files = Directory.GetFiles(PLAYLISTS_DIRECTORY);
            List<string> playlists = files.Select(f => System.IO.Path.GetFileNameWithoutExtension(f)).ToList();
            Playlists = new ObservableCollection<string>(playlists);

            //AuthenticateToSpotify();

            m_streamingSystemManager.OnTrackChange += OnTrackChanged;
            m_streamingSystemManager.OnTrackTimeChange += OnCurrentTrackTimeChange;
            m_streamingSystemManager.OnPlayStateChange += OnPlayStateChanged;
        }

        private void EnablePlaylists()
        {
            m_createPlaylistPanel.Visibility = Visibility.Visible;
            m_playlistControl.Visibility = Visibility.Visible;
            m_playlistDataGrid.Visibility = Visibility.Visible;

            m_searchPanel.Visibility = Visibility.Collapsed;
            m_systemsPanel.Visibility = Visibility.Collapsed;
            m_foundDataGrid.Visibility = Visibility.Collapsed;

            m_searchButton.Background = Brushes.Gray;
            m_playlistsButton.Background = Brushes.LightBlue;
        }

        private void EnableSearch()
        {
            m_createPlaylistPanel.Visibility = Visibility.Collapsed;
            m_playlistControl.Visibility = Visibility.Collapsed;
            m_playlistDataGrid.Visibility = Visibility.Collapsed;

            m_searchPanel.Visibility = Visibility.Visible;
            m_systemsPanel.Visibility = Visibility.Visible;
            m_foundDataGrid.Visibility = Visibility.Visible;

            m_searchButton.Background = Brushes.LightBlue;
            m_playlistsButton.Background = Brushes.Gray;
        }

        private void OnTrackChanged()
        {
            Dispatcher.Invoke(() =>
            {
                /*if(PlayingPlaylist)
                    PlayNextTrackInCurrentPlaylist();*/
            });
        }

        private void OnPlayStateChanged(bool isPlaying)
        {
            Dispatcher.Invoke(() =>
            {
                if(!isPlaying && PlayingPlaylist)
                    PlayNextTrackInCurrentPlaylist();
            });
        }

        private void OnCurrentTrackTimeChange(int time)
        {
            Dispatcher.Invoke(() =>
            {
                m_trackSlider.Value = time;
            });
        }

        /*private async Task AuthenticateToSpotify()
        {
            try
            {
                bool isAuthenticated = await m_streamingSystemManagers[(int)StreamingSystemType.Spotify].RunAuthentication();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }*/

        private void OnClickSearchOption(object sender, RoutedEventArgs e)
        {
            EnableSearch();
        }

        private async void OnClickSearch(object sender, RoutedEventArgs e)
        {
            string keyWord = m_searchTextBox.Text;
            FoundTracks.Clear();
            List<StreamingSystemType> types = new List<StreamingSystemType>();
            if (m_spotifySearchCheck.IsChecked == true)
                types.Add(StreamingSystemType.Spotify);
            if (m_deezerSearchCheck.IsChecked == true)
                types.Add(StreamingSystemType.Deezer);
            if (m_jamendoSearchCheck.IsChecked == true)
                types.Add(StreamingSystemType.Jamendo);

            FoundTracks = new ObservableCollection<LocalTrack>(await m_streamingSystemManager.SearchTrack(m_searchTextBox.Text, types.ToArray()));
        }

        private void OnClickPlaylistsOption(object sender, RoutedEventArgs e)
        {
            EnablePlaylists();
        }

        private void OnClickConnectSpotify(object sender, RoutedEventArgs e)
        {
            bool connectedToClient = m_streamingSystemManager.Connect((int)StreamingSystemType.Spotify);
            if (!connectedToClient)
                MessageBox.Show("Verify that spotify client is running in your machine and try again", "Connaction failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void OnPlayFoundTrack(object sender, RoutedEventArgs e)
        {
            LocalTrack track = FoundTracks[m_foundDataGrid.SelectedIndex];
            m_streamingSystemManager.Play(track);
            CurrentTrack = track;
        }

        private void Window_Closed(object sender, EventArgs e)
        {         
            m_streamingSystemManager?.Pause();
            if (m_streamingSystemManager != null)
            {
                m_streamingSystemManager.OnTrackTimeChange -= OnCurrentTrackTimeChange;
                m_streamingSystemManager.OnTrackChange -= OnTrackChanged;
                m_streamingSystemManager.OnPlayStateChange -= OnPlayStateChanged;
            }
        }

        private static void OnCurrentTrackChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LocalTrack track = e.NewValue as LocalTrack;
            MainWindow thisWindow = d as MainWindow;

            thisWindow.m_trackSlider.Maximum = track.Duration * 60;
            if(track.Image != null)
                thisWindow.m_trackImage.Source = new BitmapImage(new Uri(track.Image));
            thisWindow.m_titleTrackTextBox.Text = "Title : " + track.Name;
            thisWindow.m_albumTrackTextBox.Text = "Album : " + track.Album;
            thisWindow.m_artistTrackTextBox.Text = "Artist : " + track.Artist;
        }

        private void OnClickCreatePlaylist(object sender, RoutedEventArgs e)
        {
            LocalPlaylist playlist = new LocalPlaylist() { Name = m_playListTextBox.Text };
            playlist.Save(System.IO.Path.Combine(PLAYLISTS_DIRECTORY, playlist.Name + ".xml"));
            Playlists.Add(playlist.Name);
        }

        private void OnPlayPlaylist(object sender, RoutedEventArgs e)
        {
            Button play = sender as Button;
            TextBlock textBlock = VisualTreeHelper.GetChild(VisualTreeHelper.GetParent(play), 1) as TextBlock;
            string name = textBlock?.Text;
            if (name != null)
            {
                CurrentPlaylist = (new LocalPlaylist(System.IO.Path.Combine(PLAYLISTS_DIRECTORY, name + ".xml"))).Tracks;
                PlayNextTrackInCurrentPlaylist();
            }
        }

        private void PlayNextTrackInCurrentPlaylist()
        {
            int index = CurrentPlaylistTrackIndex;
            if (index < CurrentPlaylist.Count)
            {
                PlayingPlaylist = true;
                CurrentTrack = CurrentPlaylist[index];
                m_streamingSystemManager.Play(CurrentTrack).Wait();
                CurrentPlaylistTrackIndex++;
            }
            else
            {
                PlayingPlaylist = false;
                m_streamingSystemManager.Pause().Wait();
            }
        }

        private void OnClickAddToPlaylist(object sender, RoutedEventArgs e)
        {
            if (CurrentTrack != null)
            {
                string file = System.IO.Path.Combine(PLAYLISTS_DIRECTORY, (string)m_playlistsComboBox.SelectedItem + ".xml");
                LocalPlaylist playlist = new LocalPlaylist(file);
                bool added = playlist.Add(CurrentTrack);
                playlist.Save(file);
                if (added)
                    MessageBox.Show("Track added");
                else
                    MessageBox.Show("Track exists already");
            }
        }

        private void OnClickPrevious(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickPlayPause(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickNext(object sender, RoutedEventArgs e)
        {

        }
    }
}
