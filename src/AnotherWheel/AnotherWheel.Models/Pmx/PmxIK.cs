using System.Collections.Generic;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxIK {

        public int TargetBoneIndex { get; internal set; }

        public int LoopCount { get; internal set; }

        public float Angle { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IKLink> Links { get; internal set; }

    }
}
