namespace AnotherWheel.Models.Pmx {
    public sealed class BoneWeight {

        public int BoneIndex { get; internal set; } = InvalidBoneIndex;

        public float Weight { get; internal set; } = 0;

        public bool IsValid => BoneIndex != -1 && !Weight.Equals(0);

        public const int InvalidBoneIndex = -1;

    }
}
