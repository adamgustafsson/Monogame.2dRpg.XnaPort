using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace View
{
    /// <summary>
    ///  Klass för uppspelning av samtliga ljud
    /// </summary>
    class SoundHandler
    {
        //Variabler
        private View.InputHandler m_inputHandler;
        private SoundEffect[] m_soundEffects;
        private Song[] m_soundTracks;
        private int m_activeSong;

        //Publika konstanter för ljudeffekter
        public const int MENU_BUTTON_HOVER = 0;
        public const int MENU_BUTTON_SELECT = 1;
        public const int MENU_BUTTON_SELECT_B = 2;

        //Publika konstanter för soundtracks
        public const int THEME = 0;
        public const int WORLD = 1;
        public const int DUNGEON = 2;
        public const int TOWN = 3;

        public SoundHandler(View.InputHandler a_inputHandler)
        {
            this.m_inputHandler = a_inputHandler;
        }

        //Laddar in samtliga ljudeffekter & soundtracks
        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager a_content)
        {
            m_soundTracks = new Song[4] {a_content.Load<Song>("Sound/Music/HeroicDemise"),
                                         a_content.Load<Song>("Sound/Music/Soliloquy"),
                                         a_content.Load<Song>("Sound/Music/DesertTrauma"),
                                         a_content.Load<Song>("Sound/Music/Caketown")};

            m_soundEffects = new SoundEffect[3] {a_content.Load<SoundEffect>("Sound/Effects/hover"),
                                                  a_content.Load<SoundEffect>("Sound/Effects/menuSelect"),
                                                   a_content.Load<SoundEffect>("Sound/Effects/menuSelect2")};
        }

        //Metod för uppspelning av ljudeffekter
        internal void PlaySound(int a_sound, float a_volume)
        {
            if(!m_inputHandler.SoundDisabled)
                m_soundEffects[a_sound].Play(a_volume, 0f, 0f);
        }

        //Retunerar true om angiven song spelas
        public bool IsPlayingSong(int a_songId)
        {
            return MediaPlayer.State == MediaState.Playing && m_activeSong == a_songId;
        }

        internal void StopSong()
        {
            MediaPlayer.Stop();
        }

        //Metod för uppspelning av angiven song
        internal void PlaySoundTrack(int a_songId)
        {
            if (m_inputHandler.MusicDisabled)
                StopSong();
            else if (!IsPlayingSong(a_songId))
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(m_soundTracks[a_songId]);
                m_activeSong = a_songId;
            }
        }
    }
}
