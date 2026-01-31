namespace Script.Tools
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public static class RandomUtil 
    {
        /// <summary>
        /// 生成 n 个不重复随机数 [min, max)
        /// </summary>
        public static List<int> GetUniqueRandoms(int min, int max, int count)
        {
            if (count > max - min)
            {
                Debug.LogError("范围不够，无法生成足够的不重复随机数");
                return null;
            }
        
            HashSet<int> numbers = new HashSet<int>();
            while (numbers.Count < count)
            {
                numbers.Add(Random.Range(min, max));
            }
        
            return numbers.ToList();
        }
    }

}