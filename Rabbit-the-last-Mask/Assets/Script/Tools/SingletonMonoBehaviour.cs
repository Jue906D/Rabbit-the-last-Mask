using UnityEngine;

namespace Script.Tools
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
        where T : SingletonMonoBehaviour<T>
    {
        private static readonly object _lock = new object();
        private static T _instance;

        /// <summary>
        /// 全局访问点。第一次访问时会自动创建。
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            // 先在场景中找一份现成的
                            _instance = FindObjectOfType<T>();

                            if (_instance == null)
                            {
                                // 创建隐藏的单例宿主
                                const string holderName = "MonoSingletonHolder";
                                GameObject holder = GameObject.Find(holderName);
                                if (holder == null)
                                {
                                    holder = new GameObject(holderName);
                                    // 切换场景时不销毁，可选
                                    DontDestroyOnLoad(holder);
                                }

                                _instance = holder.AddComponent<T>();
                            }

                            // 初始化
                            _instance.OnSingletonAwake();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 子类可以重载，做真正的初始化。只会调用一次。
        /// </summary>
        protected virtual void OnSingletonAwake()
        {
        }

        /// <summary>
        /// 保证在编辑器停止 Play 模式时，把静态引用清掉，
        /// 防止下次进入 Play 出现“旧单例”。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainReload()
        {
            _instance = null;
        }

        /// <summary>
        /// 如果单例随场景销毁，则把引用清空。
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }

}