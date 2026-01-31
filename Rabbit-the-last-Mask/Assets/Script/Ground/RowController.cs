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
        
        public List<Transform> transforms = new List<Transform>(5);
        public Transform leavePos1;
        public Transform leavePos2;
        
        public GameObject rowPrefab;
        public void SwitchRows()
        {
            if (rows.Count <= totalRowNum+1)
            {
                var newRow = GenrateNewRow();
                rows.Insert(0, newRow);
                for (int i = 0; i < totalRowNum; i++)
                {
                    MoveRow(rows[i], transforms[i]);
                }
            }
            else
            {
                RowBase lastItem = rows[^1];
                rows.RemoveAt(rows.Count - 1);
                rows.Insert(0, lastItem);
                for (int i = 0; i < totalRowNum; i++)
                {
                    MoveRow(rows[i], transforms[i],i==0);
                }
            }
            
        }

        public void MoveRow(RowBase row,Transform nextTransform,bool leave = false)
        {
            if (leave)
            {
                const float jumpDuration = 0.3f;
                Sequence.Create()
                    .Chain(Tween.LocalPosition(row.transform, leavePos1.position, jumpDuration))
                    .OnComplete(this, target => {
                        target.transform.position = transforms[0].position;
                    });
            }
            else
            {
                const float jumpDuration = 0.3f;
                Sequence.Create()
                    .Chain(Tween.LocalPosition(row.transform, nextTransform.position, jumpDuration));
            }

        }

        public RowBase GenrateNewRow()
        {
            var tmp =Instantiate(rowPrefab, transforms[0]);
            return tmp.GetComponent<RowBase>();
        }
        
        
    }
}