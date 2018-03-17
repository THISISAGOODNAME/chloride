using System.Collections.Generic;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Vmd {
    public sealed class VmdIKFrame : VmdBaseFrame {

        public bool Visible { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IKControl> IKControls { get; internal set; }

    }
}
