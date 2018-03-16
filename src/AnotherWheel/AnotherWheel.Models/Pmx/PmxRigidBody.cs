using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxRigidBody : PmxBaseBody {

        public int BoneIndex { get; internal set; }

        public BoundingBoxKind BoundingBoxKind { get; internal set; }

        public Vector3 BoundingBoxSize { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 Rotation { get; internal set; }

        public float Mass { get; internal set; }

        public float PositionDamping { get; internal set; }

        public float RotationDamping { get; internal set; }

        public float Restitution { get; internal set; }

        public float Friction { get; internal set; }

        public KineticMode KineticMode { get; internal set; }

    }
}
