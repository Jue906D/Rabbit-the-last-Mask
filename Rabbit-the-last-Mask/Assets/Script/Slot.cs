using UnityEngine;

namespace Script
{
    public class Slot :MonoBehaviour
    {
        public bool hasMask;
        
        public Mask mask;

        public void GetMask(Mask mask)
        {
            Debug.Log($"GetMask {mask.name}");
            hasMask = true;
            this.mask = Instantiate(mask.RectTransform.gameObject, transform).GetComponent<Mask>();
            this.mask.RectTransform.SetParent(gameObject.transform);
            this.mask.RectTransform.anchoredPosition = Vector2.zero;
            this.mask.RectTransform.localScale = new Vector3(2, 2, 1);
        }
        
        public void GetMask(GameObject maskPrefab)
        {
            Debug.Log($"GetMask {maskPrefab.name}");
            hasMask = true;
            this.mask = Instantiate(maskPrefab, transform).GetComponent<Mask>();
            this.mask.RectTransform.SetParent(gameObject.transform);
            this.mask.RectTransform.anchoredPosition = Vector2.zero;
            this.mask.RectTransform.localScale = new Vector3(2, 2, 1);
        }
    }
}