using System.Collections.Generic;
using Script.Ground;
using Script.SObj;
using Script.Tools;
using UnityEngine;

namespace Script.Manager
{
    public class KeyManager : SingletonMonoBehaviour<KeyManager>
    {
        // Start is called before the first frame update

        public List<Slot> slots = new List<Slot>();
        public Mask CurChooseMask;
        public int CurChooseMaskIndex;
        [SerializeField]
        public List<GameObject> masks = new List<GameObject>();
        public RectTransform chooseArrow;
        void Awake()
        {
            masks = new List<GameObject>();
            foreach (var m in GameCenter.Instance.levelConfig.maskDict)
            {
                masks.Add(m.Value.gameObject);
            }
        }

        void Start()
        {
            foreach (var sl in slots)
            {
                var rand = Random.Range(0,masks.Count);
                sl.GetMask(masks[rand]);
            }

            CurChooseMaskIndex = 0;
            CurChooseMask = slots[CurChooseMaskIndex].mask;
        }
        
        void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Space) && RowController.Instance.nearLeave != null)
            {
                GameCenter.Instance.ChangeMask(CurChooseMask);
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                foreach (var sl in slots)
                {
                    var rand = Random.Range(0,masks.Count);
                    sl.GetMask(masks[rand]);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                CurChooseMaskIndex = (CurChooseMaskIndex -1 + slots.Count)% slots.Count;
                CurChooseMask = slots[CurChooseMaskIndex].mask;
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                CurChooseMaskIndex = (CurChooseMaskIndex +1)% slots.Count;
                CurChooseMask = slots[CurChooseMaskIndex].mask;
            }
            
            chooseArrow.anchoredPosition = slots[CurChooseMaskIndex].GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
