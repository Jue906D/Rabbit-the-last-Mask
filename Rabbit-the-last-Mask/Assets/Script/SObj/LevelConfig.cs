using System;
using System.Collections.Generic;
using System.IO;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace Script.SObj
{
    [CreateAssetMenu(fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField]
        public string BgmName ;

        public List<PartConfig> Parts; 
        
        [SerializeField]
        public MaskDict maskDict;
        
        private static string filePath => Path.Combine(Application.persistentDataPath, "ScoreRepo.asset");
    }
    
    [Serializable]
    public struct PartConfig
    {
        public int switchInterval;
        public int beginAt;
        
        public GameObject NpcPrefab;
        public GameObject SlotPrefab;
        public List<int> slotNum;
        
        public Mask mask;
    }
    [Serializable]
    public class MaskDict : SerializableDictionaryBase<string, Mask> { }
    
}