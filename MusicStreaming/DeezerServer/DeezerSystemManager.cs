using Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E.Deezer;
using E.Deezer.Api;

namespace DeezerServer
{
    public class DeezerSystemManager
    {
        Deezer m_deezer = DeezerSession.CreateNew();

        private static DeezerSystemManager m_instance;

        private DeezerSystemManager()
        {            
        }
        /// <summary>
        /// permet d obtenir l instance unique(Singleton)
        /// </summary>
        /// <returns>SpotifyManager</returns>
        public static DeezerSystemManager GetInstance()
        {
            if (m_instance == null)
                m_instance = new DeezerSystemManager();
            return m_instance;
        }

        /// <summary>
        /// Permet de chercher une piste par mot cle et retourne une liste
        /// des pistes trouvees
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns>Task<List<LocalTrack>></returns>
        public async Task<List<LocalTrack>> SearchTrack(string keyWord)
        {
            var search = await m_deezer?.Search.Tracks(keyWord);
            return new List<LocalTrack>(search.Select(t => ToLocalTrack(t)).ToList());
        }

        private LocalTrack ToLocalTrack(ITrack deezerTrack)
        {
            LocalTrack track = new LocalTrack();
            track.Name = deezerTrack.Title;
            track.Album = deezerTrack.Album.Title;
            track.Artist = deezerTrack.Artist.Name;
            track.Id = deezerTrack.Id.ToString();
            track.Duration = (deezerTrack.Duration * 10 / 6) / 100.0;
            track.Type = StreamingSystemType.Deezer;
            track.Image = deezerTrack.Album.GetPicture(PictureSize.Medium);
            return track;
        }
    }
}
