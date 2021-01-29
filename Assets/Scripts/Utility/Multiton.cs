using System.Collections.Generic;
using UnityEngine;

namespace utility
{
    public class Multiton<T> : MonoBehaviour where T : Multiton<T>
    {
        public static List<T> Instances =>
            _instances;

        private static readonly List<T> _instances = new List<T>();

        protected virtual void OnEnable()
        {
            _instances.Add((T) this);
        }

        protected virtual void OnDisable()
        {
            _instances.Remove((T) this);
        }
    }
}