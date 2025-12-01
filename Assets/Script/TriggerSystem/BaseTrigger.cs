// BaseTrigger.cs : Description : Abstract base class for all triggers

using System.Collections.Generic;
using UnityEngine;

namespace TriggerSystem
{
    public abstract class BaseTrigger : MonoBehaviour
    {
        #region --- Private Fields ---

        protected List<TriggerAction> triggerActions = new();

        #endregion

        #region --- Unity Methods ---

        protected virtual void Awake()
        {
            // Search for all TriggerAction components on this GameObject and children
            //triggerActions.AddRange(GetComponents<TriggerAction>());
            triggerActions.AddRange(GetComponentsInChildren<TriggerAction>());
        }

        #endregion

        #region --- Methods ---

        protected void InvokeTrigger(TriggerContext context)
        {
            foreach (var action in triggerActions)
                if (action != null && action.enabled)
                    action.Execute(context);
        }

        protected abstract void Trigger(TriggerContext context);

        #endregion
    }
}