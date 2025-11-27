// BaseTrigger.cs : Description : Abstract base class for all triggers

using UnityEngine;
using System.Collections.Generic;

namespace TriggerSystem
{
    public abstract class BaseTrigger : MonoBehaviour
    {
        protected List<TriggerAction> triggerActions = new List<TriggerAction>();

        protected virtual void Awake()
        {
            // Search for all TriggerAction components on this GameObject and children
            triggerActions.AddRange(GetComponents<TriggerAction>());
            triggerActions.AddRange(GetComponentsInChildren<TriggerAction>());
        }

        protected abstract void Trigger(TriggerContext context);

        protected void InvokeTrigger(TriggerContext context)
        {
            foreach (var action in triggerActions)
            {
                if (action != null && action.enabled)
                {
                    action.Execute(context);
                }
            }
        }
    }
}

