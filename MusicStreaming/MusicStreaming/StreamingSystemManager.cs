using Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using MusicStreaming.Players;

namespace MusicStreaming
{
    public class StreamingSystemManager
    {
        public delegate void OnPlayStateChangeLocalHandler(bool isPlaying);
        public delegate void OnTrackTimeChangeLocalHandler(int time);
        public delegate void OnVolumeChangeLocalHandler();
        public delegate void OnTrackChangeLocalHandler();

        public event OnPlayStateChangeLocalHandler OnPlayStateChange;
        public event OnTrackTimeChangeLocalHandler OnTrackTimeChange;
        public event OnVolumeChangeLocalHandler OnVolumeChange;
        public event OnTrackChangeLocalHandler OnTrackChange;

        readonly private RestClient[] m_clients = {
            new RestClient("http://localhost:8000/"),
            new RestClient("http://localhost:8001/"),
            new RestClient("http://localhost:8002/")
        };

        readonly private StreamingPlayer[] m_players = {
            SpotifyPlayer.GetInstance(),
            DeezerPlayer.GetInstance(),
            JamendoPlayer.GetInstance()
        };

        private StreamingPlayer m_currentPlayer;

        public StreamingPlayer CurrentPlayer
        {
            get { return m_currentPlayer; }
            set
            {
                m_currentPlayer?.UnSubscribe();
                UnSubscribeCurrentPlayer();
                m_currentPlayer = value;
                m_currentPlayer.Subscribe();
                SubscribeCurrentPlayer();
            }
        }

        private void SubscribeCurrentPlayer()
        {
            if (m_currentPlayer != null)
            {
                m_currentPlayer.OnPlayStateChange += OnPlayStateChangeHandler;
                m_currentPlayer.OnTrackChange += OnTrackChangeHandler;
                m_currentPlayer.OnTrackTimeChange += OnTrackTimeChangeHandler;
                m_currentPlayer.OnVolumeChange += OnVolumeChangeHandler;
            }
        }

        private void UnSubscribeCurrentPlayer()
        {
            if (m_currentPlayer != null)
            {
                m_currentPlayer.OnPlayStateChange -= OnPlayStateChangeHandler;
                m_currentPlayer.OnTrackChange -= OnTrackChangeHandler;
                m_currentPlayer.OnTrackTimeChange -= OnTrackTimeChangeHandler;
                m_currentPlayer.OnVolumeChange -= OnVolumeChangeHandler;
            }
        }

        private static StreamingSystemManager s_instance;

        private StreamingSystemManager()
        {
        }

        /// <summary>
        /// permet d obtenir une reference vers l instance unique (Singleton)
        /// </summary>
        /// <returns>StreamingSystemManager</returns>
        public static StreamingSystemManager GetInstance()
        {
            if (s_instance == null)
                s_instance = new StreamingSystemManager();
            return s_instance;
        }

        public bool Connect(StreamingSystemType system)
        {
            return m_players[(int)system].Connect();
        }

        /// <summary>
        /// Permet de chercher une piste par mot cle et retourne une liste
        /// des pistes trouvees
        /// </summary>
        /// <param name="keyWords"></param>
        /// <returns>Task<List<LocalTrack>></returns>
        public async Task<List<LocalTrack>> SearchTrack(string keyWords, params StreamingSystemType[] types)
        {
            var request = new RestRequest("Track", Method.GET);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            request.AddParameter("keys", keyWords);

            return await Task.Run(() =>
            {
                List<LocalTrack> tracks = new List<LocalTrack>();
                foreach (StreamingSystemType type in types)
                {
                    var response = m_clients[(int)type].Execute<List<LocalTrack>>(request);
                    tracks.AddRange(response?.Data);
                }
                return tracks;
            });
        }

        /// <summary>
        /// retourne la piste courante
        /// </summary>
        /// <returns>LocalTrack</returns>
        public LocalTrack GetCurrentTrack()
        {
            return m_currentPlayer?.GetCurrentTrack();
        }

        /// <summary>
        /// permet de jouer une jouer une piste de Spotify
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public async Task Play(LocalTrack track)
        {
            CurrentPlayer = m_players[(int)track.Type];
            await CurrentPlayer.Play(track);
        }

        /// <summary>
        /// permet de mettre en pause la lecture d une piste
        /// </summary>
        /// <returns></returns>
        public async Task Pause()
        {
            await CurrentPlayer?.Pause();
        }

        protected void OnTrackChangeHandler()
        {
            OnTrackChange?.Invoke();
        }

        protected void OnVolumeChangeHandler()
        {
            OnVolumeChange?.Invoke();
        }

        protected void OnPlayStateChangeHandler(bool isPlaying)
        {
            OnPlayStateChange?.Invoke(isPlaying);
        }

        protected void OnTrackTimeChangeHandler(int time)
        {
            OnTrackTimeChange?.Invoke(time);
        }
    }
}
