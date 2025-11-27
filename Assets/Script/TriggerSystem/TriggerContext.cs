// TriggerContext.cs : Description : Base class for trigger context information

using UnityEngine;

namespace TriggerSystem
{
    public abstract class TriggerContext
    {
        public GameObject TriggeringObject { get; protected set; }
        public Collider TriggeringCollider { get; protected set; }

        protected TriggerContext(GameObject triggeringObject, Collider triggeringCollider)
        {
            TriggeringObject = triggeringObject;
            TriggeringCollider = triggeringCollider;
        }
    }
}

