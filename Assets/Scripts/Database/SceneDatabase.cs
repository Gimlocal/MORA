using System.Collections.Generic;
using UnityEngine;

namespace Database
{
    public enum SceneType { Normal, Underground, Out }

    public static class SceneDatabase
    {
        private static Dictionary<string, SceneType> _map = new Dictionary<string, SceneType>()
        {
            {"Title", SceneType.Out},
            {"Ending", SceneType.Out},
            {"MORA-0", SceneType.Normal},
            {"MORA-1", SceneType.Normal},
            {"MORA-2", SceneType.Normal},
            {"MORA-3", SceneType.Normal},
            {"MORA-B1", SceneType.Underground},
            {"MORA-F1", SceneType.Underground},
        };

        public static SceneType GetSceneType(string sceneName)
        {
            return _map.ContainsKey(sceneName) ? _map[sceneName] : SceneType.Normal;
        }
    }
}
