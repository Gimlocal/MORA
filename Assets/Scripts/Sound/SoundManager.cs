using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance; 
    
        [SerializeField] private AudioDatabase audioDatabase;
        private Dictionary<AudioCategory, AudioSource> _audioSources;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _audioSources = new Dictionary<AudioCategory, AudioSource>
                {
                    { AudioCategory.Bgm, gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Player, gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Tool, gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.ToolHit, gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.UI, gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Voice, gameObject.AddComponent<AudioSource>() }
                };
                
                foreach (var src in _audioSources.Values)
                {
                    src.loop = true;
                }

                var audioData = audioDatabase.GetAudioData(AudioCategory.Player, "Walk");
                _audioSources[AudioCategory.Player].clip = audioData.audioClips[0];
                _audioSources[AudioCategory.Player].volume = audioData.volume;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Play(AudioCategory.Bgm, "Bgm", true);
        }

        public void Play(AudioCategory category, string key, bool loop = false)
        {
            AudioData data = audioDatabase.GetAudioData(category, key);
            if (data == null || data.audioClips.Count == 0) return;
            
            var source = _audioSources[category];
            int ran = UnityEngine.Random.Range(0, data.audioClips.Count);
            if (loop)
            {
                source.clip = data.audioClips[ran];
                source.volume = data.volume;
                source.Play();
            }
            else
            {
                source.PlayOneShot(data.audioClips[ran], data.volume);
            }
        }

        public void PlayWalk(bool isWalking)
        {
            if (isWalking)
            {
                if (!_audioSources[AudioCategory.Player].isPlaying)
                    _audioSources[AudioCategory.Player].Play();
            }
            else
            {
                if (_audioSources[AudioCategory.Player].isPlaying)
                    _audioSources[AudioCategory.Player].Stop();
            }
        }

        public void Stop(AudioCategory category)
        {
            _audioSources[category].Stop();
        }
    }
}
