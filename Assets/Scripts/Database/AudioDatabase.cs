using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
    public enum AudioCategory
    {
        Bgm, 
        Player,
        Tool,
        ToolHit,
        UI, 
        Voice,
        Effect,
        Obtain,
    }

    [Serializable]
    public class AudioCategoryData
    {
        public AudioCategory audioCategory;
        public List<AudioData> audioData;
    }
    
    [Serializable]
    public class AudioData
    {
        public string key;
        public List<AudioClip> audioClips;
        public float volume = 1f;
    }
    
    [CreateAssetMenu (fileName = "AudioDatabase", menuName = "AudioDatabase")]
    public class AudioDatabase : ScriptableObject
    {
        public List<AudioCategoryData> audioCategoryData;

        public AudioData GetAudioData(AudioCategory audioCategory, string key)
        {
            var categoryData =  audioCategoryData.Find(x => x.audioCategory == audioCategory);
            var audioData = categoryData.audioData.Find(x => x.key == key);
            return audioData;
        }
    }
}
