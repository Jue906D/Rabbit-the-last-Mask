using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.Ground
{
    public class RowController : SingletonMonoBehaviour<RowController>
    {
        public int totalRowNum = 4;
        public List<RowBase> rows = new List<RowBase>(5);
        
        public List<RectTransform> transforms = new List<RectTransform>(5);
        public RectTransform leavePos1;
        public RectTransform rowParent;
        
        public GameObject rowPrefab;
        public RowBase nearLeave;
        public RectTransform last;
        public bool setLeave;
        public int delay = 0;
        
        
        public List<float> rowWidths = new List<float> { 500, 600, 800, 1000, 1380 };

        public List<Vector2> actorScale = new List<Vector2>
        {
            new Vector2(104, 207), new Vector2(104, 207), new Vector2(105, 207), new Vector2(150, 296),
            new Vector2(200, 385),
        };
        public List<Vector2> maskScale = new List<Vector2>
        {
            new Vector2(156, 133), new Vector2(156, 133), new Vector2(156, 133), new Vector2(202,190),
            new Vector2(220,200),
        };
        public void SwitchRows()
        {
            if (nearLeave!=null && nearLeave.seqInPart > 4 && nearLeave.seqInPart <= 7)
            {
                int miss = 0;
                foreach (var sl in nearLeave.slots)
                {
                    if (sl.hasMask == false)
                    {
                        miss ++;
                    }
                }

                if (miss>0)
                {
                    Debug.Log($"Miss:{miss}");
                    GameCenter.Instance.CurScoreBorad.missCount+=miss;
                    GameCenter.Instance.CountScore(GameCenter.Instance.CurScoreBorad,false);
                }
            }
            
            setLeave = false;
            if (rows.Count < totalRowNum+1)
            {
                var newRow = GenrateNewRow();
                rows.Insert(0, newRow);
                for (int i = 0; i < rows.Count; i++)
                {
                    MoveRow(rows[i], transforms[i]);
                }
            }
            else
            {
                RowBase lastItem = rows[^1];
                rows.RemoveAt(rows.Count - 1);
                rows.Insert(0, lastItem);
                for (int i = 0; i < rows.Count; i++)
                {
                    
                    MoveRow(rows[i], transforms[(i-rows[i].delay+5*10000)%5],(i-rows[i].delay+5*10000)%5==0 && !rows[i].leave);
                }
            }
            if (setLeave ==false)
            {
                nearLeave = null;
            }


        }

        public void MoveRow(RowBase row,RectTransform nextTransform,bool leave = false)
        {
            if (nextTransform == last)
            {
                nearLeave = row;
                setLeave = true;
            }

            for (int i =0 ; i< row.transform.childCount ; i++)
            {
                if (nextTransform == transforms[i])
                {
                    row.sortOrder = i;
                    row.rectTransform.SetSiblingIndex(row.sortOrder);
                }
            }
            if (leave)
            {
                row.leave = true;
                const float jumpDuration = 0.3f;
                Sequence.Create()
                    .Chain(Tween.Custom(
                        row.rectTransform,
                        row.rectTransform.anchoredPosition,
                        leavePos1.anchoredPosition,
                        duration: jumpDuration,
                        ease: Ease.InOutSine,
                        onValueChange: (t, v) => t.anchoredPosition = v
                    ))
                    .OnComplete(row, target => {
                        GameCenter.Instance.RefreshRow(row);
                        foreach (var a in row.actors)
                        {
                            a.GetComponent<RectTransform>().sizeDelta = actorScale[0];
                            if (a.slot.mask)
                            {
                                a.slot.mask.GetComponent<RectTransform>().sizeDelta = maskScale[0];
                            }
                        }
                        target.rectTransform.anchoredPosition = transforms[0].anchoredPosition;
                        target.rectTransform.sizeDelta = new Vector2(rowWidths[0], row.rectTransform.sizeDelta.y);
                        
                    });
            }
            else
            {
                row.leave = false;
                const float jumpDuration = 0.3f;
                foreach (var a in row.actors)
                {
                    var rt = a.GetComponent<RectTransform>();
                    Sequence.Create()
                        .Group(Tween.UISizeDelta(
                            rt,
                            actorScale[row.sortOrder],jumpDuration,Ease.InOutSine
                        ));
                    if (a.slot && a.slot.mask)
                    {
                        var mrt = a.slot.mask.GetComponent<RectTransform>();
                        Sequence.Create()
                            .Group(Tween.UISizeDelta(
                                mrt,
                                maskScale[row.sortOrder],jumpDuration,Ease.InOutSine
                            ));
                    }
                }
                Sequence.Create()
                    .Group(Tween.UISizeDelta(
                        row.rectTransform,
                        new Vector2(rowWidths[row.sortOrder], row.rectTransform.sizeDelta.y),jumpDuration,Ease.InOutSine
                        ))
                    .Group(Tween.Custom(
                        row.rectTransform,
                        row.rectTransform.anchoredPosition,
                        nextTransform.anchoredPosition,
                        duration: jumpDuration,
                        ease: Ease.InOutSine,
                        onValueChange: (t, v) => t.anchoredPosition = v
                    )).OnComplete(row, target =>
                        {
                            if (row == nearLeave && row.seqInPart<=4)
                            {
                                foreach (var actor in row.actors)
                                {
                                    if(actor.slot == null || actor.slot.mask == null || actor.slot.hasMask == false)
                                            continue;
                                    AudioManager.Instance.PlaySfx(actor.slot.mask.clip);
                                }
                            }

                        }
                    );
            }

        }

        public RowBase GenrateNewRow()
        {
            var tmp =Instantiate(rowPrefab, rowParent,false);
            tmp.transform.SetParent(rowParent);
            tmp.GetComponent<RectTransform>().anchoredPosition = transforms[0].anchoredPosition;
            GameCenter.Instance.RefreshRow(tmp.GetComponent<RowBase>());
            foreach (var a in tmp.GetComponent<RowBase>().actors)
            {
                a.GetComponent<RectTransform>().sizeDelta = actorScale[0];
                if (a.slot.mask)
                {
                    a.slot.mask.GetComponent<RectTransform>().sizeDelta = maskScale[0];
                }
            }
            return tmp.GetComponent<RowBase>();
        }
        
        
    }
}