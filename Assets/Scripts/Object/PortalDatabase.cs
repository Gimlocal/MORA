using System.Collections.Generic;
using UnityEngine;

namespace Object
{
    public enum PortalID
    {
        Mora0To1,
        Mora1To0,
        Mora1To2,
        Mora2To1,
        Mora2To3,
        Mora3To2,
    }

    [System.Serializable]
    public class PortalData
    {
        public PortalID portalID;
        public string targetScene;
        public Vector2 targetPos;
    }
    
    [CreateAssetMenu(fileName = "Portal Database", menuName = "Portal Database")]
    public class PortalDatabase : ScriptableObject
    {
        public List<PortalData> portals;
        private Dictionary<PortalID, PortalData> _portalDict;

        public PortalData GetPortal(PortalID portalID)
        {
            if (_portalDict == null)
            {
                _portalDict = new Dictionary<PortalID, PortalData>();
                foreach (PortalData portalData in portals)
                {
                    _portalDict[portalData.portalID] = portalData;
                }
            }
            
            _portalDict.TryGetValue(portalID, out PortalData result);
            return result;
        }
    }
}
