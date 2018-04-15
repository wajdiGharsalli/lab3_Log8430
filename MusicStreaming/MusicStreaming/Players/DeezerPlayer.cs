using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using DeezerPlayerLib.Engine;
using DeezerPlayerLib.Enum;
using System.IO;
using DeezerPlayer.Model;

namespace MusicStreaming.Players
{
    class DeezerPlayer : StreamingPlayer
    {
        private const string PlayingRoot = @"dzmedia:///track/";
        Player m_player;

        private static DeezerPlayer s_instance;

        private DeezerPlayer()
        {
            Connect();
        }

        public static DeezerPlayer GetInstance()
        {
            if (s_instance == null)
                s_instance = new DeezerPlayer();
            return s_instance;
        }

        /// <summary>
        /// permet de se connecter a Deezer, retourne true si la connection reussi et
        /// false sinon
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            var connectConfig = new ConnectConfig()
            {
                ccAppId = "180202",
                product_id = "DeezerWrapper",
                product_build_id = "00001",
                ccUserProfilePath = GetDeezerTempFolder(),
                ccConnectEventCb = OnConnect
            };

            var connect = new Connect(connectConfig);
            connect.Start();
            connect.SetAccessToken("fr49mph7tV4KY3ukISkFHQysRpdCEbzb958dB320pM15OpFsQs");
            connect.ConnectOfflineMode();

            m_player = new Player(connect, null);
            m_player.SongChanged += Player_SongChanged;
            m_player.Start(OnPlayerEvent);
            return true;
        }

        private void OnConnect(Connect connect, ConnectEvent connectEvent)
        {
        }

        private static void OnPlayerEvent(Player player, PlayerEvent playerEvent)
        {
            if (playerEvent.eventType == PLAYER_EVENT_TYPE.DZ_PLAYER_EVENT_RENDER_TRACK_UNDERFLOW)
                player.Next();
        }

        private void Player_SongChanged(object sender, Song e)
        {
           OnTrackChangeHandler();
        }

        /// <summary>
        /// permet d obtenir une reference vers la piste jouee actuellement
        /// </summary>
        /// <returns>LocalTrack</returns>
        public override LocalTrack GetCurrentTrack()
        {
            return new LocalTrack();
        }

        /// <summary>
        /// permet de jouer une piste de Deezer
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public override async Task Play(LocalTrack track)
        {
            await Task.Run(() => {
                var stream = PlayingRoot + track.Id;
                m_player.LoadStream(stream);
                m_player.Play();
            });
        }


        /// <summary>
        /// permet de mettre en pause la lecture d une piste Deezer
        /// </summary>
        /// <returns></returns>
        public override async Task Pause()
        {
            await Task.Run(() => {
                ERRORS errors = m_player.Pause();
            });
        }

        private static string GetDeezerTempFolder()
        {
            var tempPath = Path.GetTempPath();
            var tempFolder = Path.Combine(tempPath, "DeezerPlayer");

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }
    }
}
