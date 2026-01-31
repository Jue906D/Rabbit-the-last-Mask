using System.Collections.Generic;
using Script.SObj;
using Script.Tools;
using UnityEngine;

namespace Script
{
    public class GameCenter : SingletonMonoBehaviour<GameCenter>
    {
        [Header("分数阶梯")]
        public List<int> stageScores = new List<int>{0,6,18,24,36,50};
        [Header("结果分数")]
        public int rightScore = 3;
        public int wrongScore = 0;
        public int hitScore = -2;
        public int missScore = -1;
        
        
        public  ScoreBorad CurScoreBorad ;


        public void CountScore(ScoreBorad scoreBorad, bool upgradeStage)
        {
            scoreBorad.totalScore = rightScore*scoreBorad.rightCount + wrongScore*scoreBorad.wrongCount
                +hitScore*scoreBorad.hitCount + missScore*scoreBorad.missCount;
            if (upgradeStage)
            {
                scoreBorad.stageScore = 0;
            }
        }
        
        
        void Start()
        {
                
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}