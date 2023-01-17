using System;
using UnityEngine.Events;
using Variables;

namespace ScriptableEvents
{
    public class ScriptableEventListenerIntReference : ScriptableEventListener<IntReference, ScriptableEventIntReference, UnityEvent<IntReference>>
    {
       
    }

    [Serializable]
    public class UnityEventInt : UnityEvent<int>
    {
        
    }
    
    [Serializable]
    public class UnityEventGuid : UnityEvent<Guid>
    {
        
    }
}
