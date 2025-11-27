// TriggerAction.cs : Description : Abstract base class for all trigger actions

using UnityEngine;

namespace TriggerSystem
{
    public abstract class TriggerAction : MonoBehaviour
    {
        public abstract void Execute(TriggerContext context);
    }
}

