using System;
using System.Collections.Generic;
using Script.Tools;
using UnityEngine;

namespace Script.Ground
{
    public class RowBase : MonoBehaviour, IPoolable
    {
        public RectTransform rectTransform;
        [Header("次序 ")]
        public int sortOrder;
        [Header("点位")]
        public List<RectTransform> points;
        [Header("角色")]
        public List<Actor> actors;

        public int seqInPart;

        public List<Slot> slots;

        public int delay = 0;
        public bool leave = false;

        public string  rightMaskName;
        
        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            
        }
    }
}
