using System;
using System.Collections.Generic;
using Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance; 
    
        [SerializeField] private AudioDatabase audioDatabase;
        [SerializeField] private GameObject bgm;
        [SerializeField] private GameObject sFX;
        [SerializeField] private GameObject uI;
        [SerializeField] private GameObject voice;
        
        private Dictionary<AudioCategory, AudioSource> _audioSources;
        private AudioReverbFilter _sFXReverbFilter;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _audioSources = new Dictionary<AudioCategory, AudioSource>
                {
                    { AudioCategory.Bgm, bgm.gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Player, sFX.gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Tool, sFX.gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.ToolHit, sFX.gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.UI, uI.gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Voice, voice.gameObject.AddComponent<AudioSource>() },
                    { AudioCategory.Effect, sFX.gameObject.AddComponent<AudioSource>() },
                };
                
                foreach (var src in _audioSources.Values)
                {
                    src.loop = true;
                }

                _sFXReverbFilter = sFX.AddComponent<AudioReverbFilter>();
                _sFXReverbFilter.reverbPreset = AudioReverbPreset.Off;

                var audioData = audioDatabase.GetAudioData(AudioCategory.Player, "Walk");
                _audioSources[AudioCategory.Player].clip = audioData.audioClips[0];
                _audioSources[AudioCategory.Player].volume = audioData.volume;
            }
            else
            {
                Destroy(gameObject);
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneDatabase.GetSceneType(scene.name) == SceneType.Underground)
            {
                _sFXReverbFilter.reverbPreset = AudioReverbPreset.Cave;
                _audioSources[AudioCategory.Bgm].pitch = 0.8f;
            }
            else if (SceneDatabase.GetSceneType(scene.name) == SceneType.Normal)
            {
                _sFXReverbFilter.reverbPreset = AudioReverbPreset.Off;
                _audioSources[AudioCategory.Bgm].pitch = 1f;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
