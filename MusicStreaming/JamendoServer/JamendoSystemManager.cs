using Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JamendoApi;
using JamendoApi.ApiCalls.Tracks;
using JamendoApi.ApiEntities.Tracks;
using JamendoApi.ApiCalls.Parameters;

namespace JamendoServer
{
    public class JamendoSystemManager
    {
        JamendoApiClient m_jamedoApiClient = new JamendoApiClient("bc2eac68");

        private static JamendoSystemManager m_instance;

        private JamendoSystemManager()
        {            
        }
        /// <summary>
        /// permet d obtenir l instance unique(Singleton)
        /// </summary>
        /// <returns>JamendoSystemManager</returns>
        public static JamendoSystemManager GetInstance()
        {
            if (m_instance == null)
                m_instance = new JamendoSystemManager();
            return m_instance;
        }

        public async Task<List<LocalTrack>> SearchTrack(string keyWord)
        {
            TracksCall call = new TracksCall();
            call.Name = new NameParameter(keyWord);
            BasicTrack[] search = (await m_jamedoApiClient.CallAsync(call)).Results;
            return new List<LocalTrack>(search.Select(t => ToLocalTrack(t)).ToList());
        }

        private LocalTrack ToLocalTrack(BasicTrack basicTrack)
        {
            LocalTrack track = new LocalTrack();
            track.Name = basicTrack.Name;
            track.Album = basicTrack.AlbumName;
            track.Artist = basicTrack.ArtistName;
            track.Id = basicTrack.Id.ToString();
            track.Duration = (basicTrack.Duration * 10 / 6) / 100.0;
            track.Type = StreamingSystemType.Jamendo;
            track.Image = basicTrack.Image;
            return track;
        }
    }
}
