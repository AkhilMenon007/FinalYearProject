using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP.Client 
{
    [System.Serializable]
    public class NetworkPrefab 
    {
        [SerializeField]
        private GameObject _localPrefab=null;
        [SerializeField]
        private GameObject _networkPrefab=null;
        public GameObject localPrefab { get { return _localPrefab; } }
        public GameObject networkPrefab 
        { 
            get 
            { 
                if 
                    (_networkPrefab == null)
                    return _localPrefab;
                else
                    return _networkPrefab; 
            }
        }
    }
    [CreateAssetMenu(menuName = "FYP/Networking/Managers/NetworkSpawnPrefabs")]
    public class NetworkSpawnPrefabs : ScriptableObject
    {
        public List<NetworkPrefab> networkPrefabs = new List<NetworkPrefab>();

        private Dictionary<GameObject, int> prefabIndexLookup = new Dictionary<GameObject, int>();
        private void OnEnable()
        {
            for (int i=0;i<networkPrefabs.Count;i++)
            {
                NetworkPrefab prefab = networkPrefabs[i];
                if (!prefabIndexLookup.ContainsKey(prefab.localPrefab) && prefab.localPrefab!=null)
                {
                    prefabIndexLookup.Add(prefab.localPrefab, i);
                    if (!prefabIndexLookup.ContainsKey(prefab.networkPrefab) && prefab.networkPrefab!=prefab.localPrefab)
                        prefabIndexLookup.Add(prefab.networkPrefab, i);
                }
            }
        }

        public int GetPrefabIndex(GameObject obj) 
        {
            if (prefabIndexLookup.ContainsKey(obj)) 
            {
                return prefabIndexLookup[obj];
            }
            return -1;
        }
        private void OnDisable()
        {
            prefabIndexLookup.Clear();
        }
    }

}
