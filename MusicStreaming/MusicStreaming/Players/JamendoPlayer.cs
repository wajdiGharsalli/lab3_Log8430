using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace MusicStreaming.Players
{
    class JamendoPlayer : StreamingPlayer
    {
        private static JamendoPlayer s_instance;

        public static JamendoPlayer GetInstance()
        {
            if (s_instance == null)
                s_instance = new JamendoPlayer();
            return s_instance;
        }

        /// <summary>
        /// permet de se connecter a Deezer, retourne true si la connection reussi et
        /// false sinon
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            return true;
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
            await Task.Run(() => { });
        }


        /// <summary>
        /// permet de mettre en pause la lecture d une piste Deezer
        /// </summary>
        /// <returns></returns>
        public override async Task Pause()
        {
            await Task.Run(() => { });
        }
    }
}
