using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Database
{
    [System.Serializable]
    public class RoomType
    {
        public int id;
        [Range(0,100)] public int weight = 100;
        public DoorMask allowedDoors = DoorMask.All;
        public bool uniquePerStage = false;
        public bool isStart;
        public bool isBoss;
        public bool canSpawnMinerals = false;
        public GameObject roomPrefab;
    }
    
    [System.Flags]
    public enum DoorMask { None=0, N=1, E=2, S=4, W=8, All=15 }
    
    [CreateAssetMenu(fileName = "RoomDatabase", menuName = "RoomDatabase")]
    public class RoomDatabase : ScriptableObject
    {
        public List<RoomType> Rooms;
    }
}
