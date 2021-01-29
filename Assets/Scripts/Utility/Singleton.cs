using UnityEngine;

namespace utility
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance => 
            _instance;

        private static T _instance;

        protected virtual void Awake()
        {
            _instance = (T)this;
        }
    }
}