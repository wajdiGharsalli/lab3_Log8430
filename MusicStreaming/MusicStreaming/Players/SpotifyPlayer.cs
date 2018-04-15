using SpotifyAPI.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using SpotifyAPI.Local.Models;
using SpotifyAPI.Local.Enums;

namespace MusicStreaming.Players
{
    class SpotifyPlayer : StreamingPlayer
    {
        private readonly SpotifyLocalAPI m_spotifyLocalAPI = new SpotifyLocalAPI();

        private Track m_currentTrack;

        private static SpotifyPlayer s_instance;

        private SpotifyPlayer()
        {
            Connect();         
        }

        public static SpotifyPlayer GetInstance()
        {
            if (s_instance == null)
                s_instance = new SpotifyPlayer();
            return s_instance;
        }

        /// <summary>
        /// permet de se connecter a Spotify, retourne True si l operation reussi et
        /// false sinon
        /// </summary>
        /// <returns>bool</returns>
        public override bool Connect()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning() || !SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                return false;
            }
            bool successful;
            try
            {
                successful = m_spotifyLocalAPI.Connect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (successful)
            {
                m_spotifyLocalAPI.ListenForEvents = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// permet d obtenir une reference vers la piste jouee actuellement
        /// </summary>
        /// <returns>LocalTrack</returns>
        public override LocalTrack GetCurrentTrack()
        {
            LocalTrack track = new LocalTrack();

            StatusResponse status = m_spotifyLocalAPI.GetStatus();
            if (status != null && status.Track != null && !status.Track.IsAd())
            {
                track.Duration = (int)status.Track.Length / 60.0;
                track.Image = status.Track.GetAlbumArtUrl(AlbumArtSize.Size160);
                track.Name = status.Track.TrackResource?.Name;
                track.Artist = status.Track.ArtistResource?.Name;
                track.Album = status.Track.AlbumResource?.Name;
                track.Type = StreamingSystemType.Spotify;
            }
            return track;
        }

        /// <summary>
        /// permet de jouer une jouer une piste de Spotify
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public override async Task Play(LocalTrack track)
        {
            await m_spotifyLocalAPI.PlayURL(track.Id);
        }

        /// <summary>
        /// permet de mettre en pause la lecture d une piste
        /// </summary>
        /// <returns></returns>
        public override async Task Pause()
        {
            await m_spotifyLocalAPI.Pause();
        }

        public override void Subscribe()
        {
            m_spotifyLocalAPI.OnPlayStateChange += OnPlayStateChangeHandler;
            m_spotifyLocalAPI.OnTrackChange += OnTrackChangeHandler;
            m_spotifyLocalAPI.OnTrackTimeChange += OnTrackTimeChangeHandler;
            m_spotifyLocalAPI.OnVolumeChange += OnVolumeChangeHandler;
        }

        public override void UnSubscribe()
        {
            m_spotifyLocalAPI.OnPlayStateChange -= OnPlayStateChangeHandler;
            m_spotifyLocalAPI.OnTrackChange -= OnTrackChangeHandler;
            m_spotifyLocalAPI.OnTrackTimeChange -= OnTrackTimeChangeHandler;
            m_spotifyLocalAPI.OnVolumeChange -= OnVolumeChangeHandler;
        }

        private void OnTrackChangeHandler(object sender, TrackChangeEventArgs e)
        {
            m_currentTrack = e.NewTrack;
            OnTrackChangeHandler();
        }

        private void OnVolumeChangeHandler(object sender, VolumeChangeEventArgs e)
        {
            OnVolumeChangeHandler();
        }

        private void OnPlayStateChangeHandler(object sender, PlayStateEventArgs e)
        {
            OnPlayStateChangeHandler(e.Playing);
        }

        private void OnTrackTimeChangeHandler(object sender, TrackTimeChangeEventArgs e)
        {
            OnTrackTimeChangeHandler((int)e.TrackTime);
        }
    }
}
