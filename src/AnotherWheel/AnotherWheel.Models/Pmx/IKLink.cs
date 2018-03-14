using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx {
    // ReSharper disable once InconsistentNaming
    public sealed class IKLink {

        public int BoneIndex { get; internal set; } = InvalidBoneIndex;

        public bool IsLimited { get; internal set; }

        public Vector3 LowerBound { get; internal set; }

        public Vector3 UpperBound { get; internal set; }

        public const int InvalidBoneIndex = -1;

    }
}
