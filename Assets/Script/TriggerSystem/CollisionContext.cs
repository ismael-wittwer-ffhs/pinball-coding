// CollisionContext.cs : Description : Context for collision-based triggers

using UnityEngine;

namespace TriggerSystem
{
    public enum TriggerType
    {
        Enter,
        Exit,
        Stay
    }

    public class CollisionContext : TriggerContext
    {
        public TriggerType Type { get; private set; }
        public Collision CollisionData { get; private set; }

        public CollisionContext(GameObject triggeringObject, Collider triggeringCollider, TriggerType type, Collision collisionData = null)
            : base(triggeringObject, triggeringCollider)
        {
            Type = type;
            CollisionData = collisionData;
        }
    }
}

