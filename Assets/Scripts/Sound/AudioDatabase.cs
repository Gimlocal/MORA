using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sound
{
    public enum AudioType
    {
        Bgm,
        Walk,
        Pickaxe1,
        Pickaxe2,
        Pickaxe3,
        UI,
    }
    
    [Serializable]
    public class AudioData
    {
        public AudioClip audioClip;
        public AudioType audioType;
        public float volume;
    }
    
    [CreateAssetMenu (fileName = "AudioDatabase", menuName = "AudioDatabase")]
    public class AudioDatabase : ScriptableObject
    {
        public List<AudioData> audioData;

        public AudioData GetAudioClips(AudioType audioType)
        {
            foreach (var audio in audioData)
            {
                if (audio.audioType == audioType)
                    return audio;
            }
            return null;
        }
    }
}
