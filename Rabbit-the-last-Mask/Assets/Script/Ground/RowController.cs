using System.Collections.Generic;
using PrimeTween;
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
        public void SwitchRows()
        {
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
                    MoveRow(rows[i], transforms[i],i==0);
                }
            }
            
        }

        public void MoveRow(RowBase row,RectTransform nextTransform,bool leave = false)
        {
            if (leave)
            {
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
                        target.rectTransform.anchoredPosition = transforms[0].anchoredPosition;
                        GameCenter.Instance.RefreshRow(row);
                    });
            }
            else
            {
                const float jumpDuration = 0.3f;
                Sequence.Create()
                    .Chain(Tween.Custom(
                        row.rectTransform,
                        row.rectTransform.anchoredPosition,
                        nextTransform.anchoredPosition,
                        duration: jumpDuration,
                        ease: Ease.InOutSine,
                        onValueChange: (t, v) => t.anchoredPosition = v
                    ));
            }

        }

        public RowBase GenrateNewRow()
        {
            var tmp =Instantiate(rowPrefab, rowParent,false);
            tmp.transform.SetParent(rowParent);
            tmp.GetComponent<RectTransform>().anchoredPosition = transforms[0].anchoredPosition;
            GameCenter.Instance.RefreshRow(tmp.GetComponent<RowBase>());
            return tmp.GetComponent<RowBase>();
        }
        
        
    }
}