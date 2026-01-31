using System.Collections.Generic;
using Script.Tools;
using UnityEngine;

namespace Script.Ground
{
    public class RowBase : MonoBehaviour, IPoolable
    {
        [Header("次序 ")]
        public int sortOrder;
        [Header("点位")]
        public List<Transform> points;


        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            
        }
    }
}
