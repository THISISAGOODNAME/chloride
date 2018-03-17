using System.Collections.Generic;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Vmd {
    public sealed class VmdMotion {

        [NotNull]
        public string ModelName { get; internal set; }

        public int Version { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdBoneFrame> BoneFrames { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdFacialFrame> FacialFrames { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdCameraFrame> CameraFrames { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VmdLightFrame> LightFrames { get; internal set; }

        [CanBeNull, ItemNotNull]
        public IReadOnlyList<VmdIKFrame> IKFrames { get; internal set; }

    }
}
