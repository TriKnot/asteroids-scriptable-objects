using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variables
{
    public class VariableBase<T> : ScriptableObject
    {
        [SerializeField] private T _value;
        
        protected T CurrentValue;
        
        public T Value => CurrentValue;

        
        public virtual void ApplyChange(T change){}

        public virtual void SetValue(T newValue)
        {
            CurrentValue = newValue;
        }

        private void OnEnable()
        {
            CurrentValue = _value;
        }
    }
}
