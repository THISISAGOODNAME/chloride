using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxBone : IPmxNamedObject {

        internal PmxBone() {
        }

        public const int InvalidBoneIndex = -1;

        // ReSharper disable once NotNullMemberIsNotInitialized
        public string Name { get; internal set; }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public string NameEnglish { get; internal set; }

        public Vector3 InitialPosition {
            get => _initialPosition;
            internal set {
                _initialPosition = value;
                BindPoseMatrixInverse = Matrix.CreateTranslation(-value);
            }
        }

        public int BoneIndex { get; internal set; }

        public Vector3 CurrentPosition { get; internal set; }

        public Vector3 Axis { get; internal set; }

        public Quaternion InitialRotation { get; internal set; }

        public Quaternion CurrentRotation { get; internal set; }

        public int ParentBoneIndex { get; internal set; } = InvalidBoneIndex;

        public int Level { get; internal set; }

        public int To_Bone { get; internal set; } = InvalidBoneIndex;

        public Vector3 To_Offset { get; internal set; }

        public int AppendParentBoneIndex { get; internal set; } = InvalidBoneIndex;

        public int ExternalParentIndex { get; internal set; }

        public float AppendRatio { get; internal set; } = 1;

        public BoneFlags Flags { get; internal set; } = BoneFlags.Enabled | BoneFlags.Visible | BoneFlags.Rotation;

        [CanBeNull]
        public PmxIK IK { get; internal set; }

        [CanBeNull]
        public PmxBone ParentBone { get; internal set; }

        public Matrix WorldMatrix { get; internal set; }

        public Matrix SkinMatrix { get; internal set; }

        public override string ToString() {
            var description = GetSimpleDescription();

            if (ParentBone != null) {
                var parentInfo = $" Parent \"{ParentBone.Name}\"";

                description += parentInfo;
            }

            return description;
        }

        internal Matrix LocalMatrix { get; set; }

        internal bool IsTransformCalculated { get; set; }

        internal Matrix BindPoseMatrixInverse { get; private set; }

        internal Vector3 AnimatedTranslation { get; set; }

        internal Quaternion AnimatedRotation { get; set; }

        private string GetSimpleDescription() {
            return $"Bone \"{Name}\" [{BoneIndex}] (Position: {CurrentPosition}; Rotation: {CurrentRotation})";
        }

        private Vector3 _initialPosition;

    }
}
