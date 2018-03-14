using System.Collections.Generic;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxModel : IPmxNamedObject {

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        public string Comment { get; internal set; }

        public string CommentEnglish { get; internal set; }

        public int RootNodeIndex { get; internal set; }

        public int FacialExpressionNodeIndex { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxVertex> Vertices { get; internal set; }

        [NotNull]
        public IReadOnlyList<int> Faces { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxMaterial> Materials { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxBone> Bones { get; internal set; }

        [NotNull]
        public IReadOnlyList<int> RootBoneIndices { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxMorph> Morphs { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxNode> Nodes { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxRigidBody> RigidBodies { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxJoint> Joints { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxSoftBody> SoftBodies { get; internal set; }

    }
}
