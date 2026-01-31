using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Script.Ground;
using Script.Manager;
using Script.SObj;
using Script.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class GameCenter : SingletonMonoBehaviour<GameCenter>
    {
        [Header("局内时间配置")]
        public LevelConfig levelConfig;

        public float timePassed;
        public int currentPart;
        public int realCurrentPart;

        public int interval = 300;
        public float lastMoveTime;
        public int seqInPart = 1;
        
        public HashSet<Animator> actorAnimators;
        
        [Header("分数阶梯")]
        public List<int> stageScores = new List<int>{0,6,18,24,36,50};
        [Header("结果分数")]
        public int rightScore = 3;
        public int wrongScore = 0;
        public int hitScore = -2;
        public int missScore = -1;
        
        
        public  ScoreBorad CurScoreBorad ;
        public TextMeshProUGUI tmp;
        public TextMeshProUGUI timeText;
        public float timeStart;
        public float timeOffset;
        public float timeOverAt;

        public GameObject m_mask;

        public void CountScore(ScoreBorad scoreBorad, bool upgradeStage)
        {
            scoreBorad.totalScore = rightScore*scoreBorad.rightCount + wrongScore*scoreBorad.wrongCount
                +hitScore*scoreBorad.hitCount + missScore*scoreBorad.missCount;
            scoreBorad.totalScore = scoreBorad.totalScore >= 0 ? scoreBorad.totalScore : 0;
            if (upgradeStage)
            {
                scoreBorad.stageScore = 0;
            }
            tmp.text = scoreBorad.totalScore.ToString();
        }

        private void Awake()
        {
            actorAnimators = new HashSet<Animator>();
            timePassed = 0;
            m_mask = levelConfig.Parts[0].mask.gameObject;
        }


        void Start()
        {
                AudioManager.Instance.PlayBGM("main");
                timeStart = Time.fixedTime;
                //AudioManager.Instance.PlaySfx("sheep");
                timePassed = 0;
                currentPart = 0;
                realCurrentPart = -1;
        }

        public void Reset()
        {
            AudioManager.Instance.PlayBGM("main");
            timeStart = Time.fixedTime;
            timePassed = 0;
            currentPart = 0;
            realCurrentPart = -1;
            lastMoveTime = 0;
            seqInPart = 0;
            CurScoreBorad = new ScoreBorad();
            timeOverAt = 0;
            m_mask = null;
        }
        
        void FixedUpdate()
        {
            if (! AudioManager.Instance.bgmSource.isPlaying ||  AudioManager.Instance.bgmSource.clip == null) return;
            
            int currentSample =  AudioManager.Instance.bgmSource.timeSamples;
            var effectiveSample = currentSample - AudioManager.Instance.bgm_delay;

            timePassed =( Time.fixedTime - timeStart  - 0.23f)*1000f+ timeOffset;
            timeText.text =$" {(timePassed- timeOffset+- 0.23f)/1000f:F2}";
            var baseNum = AudioManager.Instance.bgm_interval * 4;
            loopProgress = effectiveSample % baseNum/ baseNum;
            foreach (var a in actorAnimators)
            {
                var stateInfo = a.GetCurrentAnimatorStateInfo(0);
                a.Play(stateInfo.fullPathHash, 0, loopProgress);
            }
            
                if (realCurrentPart < levelConfig.Parts.Count)
                {
                    if (realCurrentPart == -1 || levelConfig.Parts[realCurrentPart].beginAt < timePassed)
                    {
                        if (realCurrentPart < levelConfig.Parts.Count-1)
                            realCurrentPart++;
                    }
                }

                if (currentPart == levelConfig.Parts.Count - 1 )
                {
                    if (timeOverAt <= 1f)
                        timeOverAt = Time.fixedTime * 1000f;
                    if (timeOverAt + 10000f < Time.fixedTime*1000f)
                    {
                        timeOverAt += 10000f;
                        m_mask =
                            KeyManager.Instance.masks[Random.Range(0, KeyManager.Instance.masks.Count())];
                    }
                }
                
                
                if (lastMoveTime + interval <= timePassed)
                {
                    RowController.Instance.SwitchRows();
                    lastMoveTime = timePassed;
                }

        }

        public void RefreshRow(RowBase row)
        {
            List<int> randoms;
            GameObject tmp;
            row.seqInPart = seqInPart;
            row.slots = null;
            row.delay = RowController.Instance.delay;
            if (seqInPart > 4)
            { 
                randoms = RandomUtil.GetUniqueRandoms(0, 5, levelConfig.Parts[currentPart].slotNum[seqInPart-5]);
                row.slots = new List<Slot>();
            }
            else
            {
                randoms = null;
            }

            if (seqInPart == 7)
            {
                RowController.Instance.delay++;
            }
            if (seqInPart == 1 &&realCurrentPart!=-1)
            {
                currentPart = realCurrentPart;
                interval = levelConfig.Parts[currentPart].switchInterval;
                m_mask = levelConfig.Parts[currentPart].mask.gameObject;
            }
            seqInPart =seqInPart % 7 +1;
            for (int i = 0; i < row.points.Count; i++)
            {
                tmp = null;
                if (randoms != null)
                {
                    foreach (var r in randoms)
                    {
                        if (i == r)
                        {
                            tmp = Instantiate(levelConfig.Parts[currentPart].SlotPrefab,row.points[i]);
                            row.slots.Add(tmp.GetComponent<Actor>().slot);
                            break;
                        }
                    }

                    if (tmp == null)
                    {
                        tmp = Instantiate(levelConfig.Parts[currentPart].NpcPrefab, row.points[i]);
                        tmp.GetComponent<Actor>().slot.GetMask(m_mask);
                        tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 200);
                        row.rightMaskName = m_mask.GetComponent<Mask>().maskName; 
                    }

                }
                else
                {
                    tmp = Instantiate(levelConfig.Parts[currentPart].NpcPrefab, row.points[i]);
                    tmp.GetComponent<Actor>().slot.GetMask(m_mask);
                    tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 200);
                }
                tmp.GetComponent<RectTransform>().anchoredPosition = row.actors[i].gameObject.GetComponent<RectTransform>().anchoredPosition;
                
                actorAnimators.Remove(row.actors[i].animator);
                Destroy(row.actors[i].gameObject);
                
                var animator = tmp.GetComponent<Actor>();
                row.actors[i] = animator;
                actorAnimators.Add(animator.animator);
            }
        }

        public List<Mask> maks;
        public float  loopProgress =  0f;
        public Animator normalAnimator;
        
        public void ChangeMask(Mask mask)
        {
            bool masked;
            var curRow = RowController.Instance.nearLeave;
            if (curRow.seqInPart <= 4)
            {
                Debug.Log("Cannot add mask");
                return;
            }

            masked = false;
            foreach (var sl in curRow.slots)
            {
                if (sl.hasMask)
                {
                    continue;
                }
                else
                {
                    masked = true;
                    sl.GetMask(mask.gameObject);
                    sl.mask.RectTransform.sizeDelta = RowController.Instance.maskScale[curRow.sortOrder];
                    break;
                }
            }

            if (masked )
            {
                if (mask.maskName == m_mask.GetComponent<Mask>().maskName)
                {
                    //RIGHT
                    
                    CurScoreBorad.rightCount++;
                    Debug.Log($"Right play{mask.clip.name}");
                    AudioManager.Instance.PlaySfx(mask.clip);
                }
                else
                {
                    CurScoreBorad.wrongCount++;
                    AudioManager.Instance.PlaySfx(mask.clip);
                    Debug.Log($"Wrong play{mask.clip.name}");
                }
            }
            else
            {
                //hit
                CurScoreBorad.hitCount ++;
                AudioManager.Instance.PlaySfx(AudioManager.Instance.hitClip);
                Debug.Log("Hit");
            }
            CountScore(CurScoreBorad,false);
        }
        
    }

}