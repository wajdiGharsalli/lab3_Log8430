using Utilities;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyServer
{
    public class SpotifySystemManager
    {
        private SpotifyWebAPI m_spotifyWebAPI;
        
        private static SpotifySystemManager m_instance;

        private SpotifySystemManager()
        {
            RunAuthentication().Wait();
        }
        /// <summary>
        /// permet d obtenir l instance unique(Singleton)
        /// </summary>
        /// <returns>SpotifyManager</returns>
        public static SpotifySystemManager GetInstance()
        {
            if (m_instance == null)
                m_instance = new SpotifySystemManager();
            return m_instance;
        }

        /// <summary>
        /// fait l authentification pour Spotify, retourne True si l operation reussi 
        /// et false sinon
        /// </summary>
        /// <returns>Task<bool></returns>
        public async Task<bool> RunAuthentication()
        {
            WebAPIFactory webApiFactory = new WebAPIFactory(
                "http://localhost",
                7000,
                "8554a963221c499fa356f8b4a95e79f8",
                Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistReadCollaborative |
                Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState | Scope.UserModifyPlaybackState);

            try
            {
                m_spotifyWebAPI = await webApiFactory.GetWebApi();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (m_spotifyWebAPI == null)
                return false;
            return true;
        }

        /// <summary>
        /// Permet de chercher une piste par mot cle et retourne une liste
        /// des pistes trouvees
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns>Task<List<LocalTrack>></returns>
        public async Task<List<LocalTrack>> SearchTrack(string keyWord)
        {
            SearchItem search = await m_spotifyWebAPI.SearchItemsAsync(keyWord, SearchType.Track);
            return new List<LocalTrack>(search.Tracks.Items.Select(t => ToLocalTrack(t)).ToList());
        }

        private LocalTrack ToLocalTrack(FullTrack fullTrack)
        {
            LocalTrack track = new LocalTrack();
            track.Name = fullTrack.Name;
            track.Album = fullTrack.Album.Name;
            track.Artist = fullTrack.Artists.FirstOrDefault().Name;
            track.Id = fullTrack.ExternUrls["spotify"];
            track.Duration = (fullTrack.DurationMs / (10 * 60)) / 100.0;
            track.Type = StreamingSystemType.Spotify;
            track.Image = fullTrack.Album.Images.Count > 0 ? fullTrack.Album.Images[0].Url : null;
            return track;
        }
    }
}
