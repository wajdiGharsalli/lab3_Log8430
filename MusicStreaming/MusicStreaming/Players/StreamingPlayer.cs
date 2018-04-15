using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static MusicStreaming.StreamingSystemManager;

namespace MusicStreaming.Players
{
    public abstract class StreamingPlayer
    {
        public event OnPlayStateChangeLocalHandler OnPlayStateChange;
        public event OnTrackTimeChangeLocalHandler OnTrackTimeChange;
        public event OnVolumeChangeLocalHandler OnVolumeChange;
        public event OnTrackChangeLocalHandler OnTrackChange;

        /// <summary>
        /// methode abstraite implementee par les differents managers pour se connecter
        /// a un client, retourne True si l'operation reussi et False sinon
        /// </summary>
        /// <returns>bool</returns>
        public abstract bool Connect();

        /// <summary>
        /// retourne la piste courante
        /// </summary>
        /// <returns>LocalTrack</returns>
        public abstract LocalTrack GetCurrentTrack();

        /// <summary>
        /// Permet de jouer une piste
        /// </summary>
        /// <param name="track"></param>
        /// <returns>Task</returns>
        public abstract Task Play(LocalTrack track);
        /// <summary>
        /// mettre en pause la lecture d une piste 
        /// </summary>
        /// <returns></returns>
        public abstract Task Pause();

        public virtual void Subscribe()
        {
        }

        public virtual void UnSubscribe()
        {
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
