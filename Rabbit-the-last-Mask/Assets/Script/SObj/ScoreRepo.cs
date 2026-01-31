using System;
using System.IO;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace Script.SObj
{
    [CreateAssetMenu(fileName = "ScoreRepo")]
    public class ScoreRepo : ScriptableObject
    {
        [SerializeField]
        public ScoreDictionary ScoreBorads ;
        
        private static string filePath => Path.Combine(Application.persistentDataPath, "ScoreRepo.asset");
    }
    
    [Serializable]
    public struct ScoreBorad
    {
        public string name;
        public int totalScore;
        public int stage;
        public int stageScore;

        public int rightCount;
        public int wrongCount;
        public int hitCount;
        public int missCount;
    }
    
    [Serializable]
    public class ScoreDictionary : SerializableDictionaryBase<string, ScoreBorad> { }
}