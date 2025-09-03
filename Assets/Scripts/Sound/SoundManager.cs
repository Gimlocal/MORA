using System;
using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance; 
    
        [SerializeField] private AudioDatabase audioDatabase;
        private AudioSource _audioSourceBgm;
        private AudioSource _audioSourceSfx1;
        private AudioSource _audioSourceSfx2;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _audioSourceBgm = gameObject.AddComponent<AudioSource>();
                _audioSourceBgm.loop = true;
                
                _audioSourceSfx1 = gameObject.AddComponent<AudioSource>();
                _audioSourceSfx1.loop = true;
                AudioData data = audioDatabase.GetAudioClips(AudioType.Walk);
                _audioSourceSfx1.clip = data.audioClip;
                _audioSourceSfx1.volume = data.volume;
                
                _audioSourceSfx2 = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Play(AudioType.Bgm);
        }

        public void Play(AudioType type)
        {
            AudioData data = audioDatabase.GetAudioClips(type);

            if (type == AudioType.Bgm)
            {
                _audioSourceBgm.clip = data.audioClip;
                _audioSourceBgm.volume = data.volume;
                _audioSourceBgm.Play();
            }
            else
            {
                _audioSourceSfx2.PlayOneShot(data.audioClip, data.volume);
            }
        }

        public void PlayWalk(bool isWalking)
        {
            if (isWalking)
            {
                if (!_audioSourceSfx1.isPlaying)
                    _audioSourceSfx1.Play();
            }
            else
            {
                if (_audioSourceSfx1.isPlaying)
                    _audioSourceSfx1.Stop();
            }
        }

        public void StopBgm()
        {
            _audioSourceBgm.Stop();
        }
    }
}
