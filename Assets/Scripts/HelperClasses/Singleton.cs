using UnityEngine;

namespace Assets.HelperClasses
{
    public abstract class CSSingleton<T> where T : new()
    {
        static T _instance;
        static readonly object _padlock = new object();

        public static T Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }

                    return _instance;
                }
            }
        }
    }

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;

        public void Awake()
        {
            m_Instance = Instance;
        }

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        // Search for existing instance.
                        m_Instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (m_Instance == null)
                        {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        } else { 
                            DontDestroyOnLoad(m_Instance);
                        }
                    }

                    return m_Instance;
                }
            }
        }


        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }


        private void OnDestroy()
        {
        }
    }
}