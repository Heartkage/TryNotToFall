using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TNTF
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager instance;
        void Awake()
        {
            instance = this;
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
        #endregion

        public Sound[] sounds;

        public void SoundOutput(SoundType type, SoundAction action)
        {
            foreach (Sound s in sounds)
            {
                if (s.name == type)
                {
                    switch (action)
                    {
                        case SoundAction.Play:
                            if (!s.source.isPlaying)
                                s.source.Play();
                            break;
                        case SoundAction.Stop:
                            if (s.source.isPlaying)
                            {
                                s.source.Pause();
                                s.source.Stop();
                            }  
                            break;
                    }
                    
                }
            }
        }


    }

    public enum SoundType
    {
        Main = 0,
        Jump = 1,
        GameOver = 2,
        LevelComplete = 3,
        GameComplete = 4,
        Collect = 5
    }

    public enum SoundAction
    {
        Play = 0,
        Stop = 1,
    }

    [System.Serializable]
    public class Sound
    {
        public SoundType name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch = 1f;
        public bool loop;

        [HideInInspector]
        public AudioSource source;

    }
}

