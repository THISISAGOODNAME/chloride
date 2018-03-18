using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxBone : IPmxNamedObject {

        // ReSharper disable once NotNullMemberIsNotInitialized
        public string Name { get; internal set; }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public string NameEnglish { get; internal set; }

        public Vector3 InitialPosition { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Vector3 RelativePosition { get; internal set; }

        public Vector3 Axis { get; internal set; }

        //public Vector3 LocalX { get; internal set; } = Vector3.UnitX;

        //public Vector3 LocalY { get; internal set; } = Vector3.UnitY;

        //public Vector3 LocalZ { get; internal set; } = Vector3.UnitZ;

        public Quaternion InitialRotation { get; internal set; }

        public Quaternion Rotation { get; internal set; }

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
        public PmxBone ReferenceParent { get; internal set; }

        public Matrix WorldMatrix { get; internal set; }

        public Matrix LocalMatrix { get; internal set; }

        internal bool HierarchyCalculated { get; set; }

        public const int InvalidBoneIndex = -1;

    }
}
