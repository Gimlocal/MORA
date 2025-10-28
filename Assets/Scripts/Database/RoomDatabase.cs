using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Database
{
    [Serializable]
    public class RoomInfo
    {
        public int id;
        public string roomName;
        [Range(0,100)] public int weight = 100;
        public DoorMask allowedDoors = DoorMask.All;
        public bool uniquePerStage;
        public RoomType roomType;
        public GameObject roomPrefab;
    }

    public enum RoomType
    {
        Start,
        Boss,
        Normal,
        Special,
        Trap,
    }
    
    [Flags]
    public enum DoorMask { None=0, N=1, E=2, S=4, W=8, All=15 }
    
    [CreateAssetMenu(fileName = "RoomDatabase", menuName = "RoomDatabase")]
    public class RoomDatabase : ScriptableObject
    {
        public List<RoomInfo> Rooms;
    }
}
