using JetBrains.Annotations;

namespace AnotherWheel.Models.Vmd {
    public sealed class VmdFacialFrame : VmdBaseFrame {

        [NotNull]
        public string FacialExpressionName { get; internal set; }

        public float Weight { get; internal set; }

    }
}
