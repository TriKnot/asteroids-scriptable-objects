using System;
using UnityEngine;

namespace Variables
{
    // TODO Can we use generics to avoid duplication?
    [CreateAssetMenu(fileName = "new FloatVariable", menuName = "ScriptableObjects/Variables/IntVariable")]
    public class IntVariable : VariableBase<int>
    {
        public int IntValue;
        
        public override void SetValue(int newValue)
        {
            CurrentValue = newValue;
        }
    }

}
