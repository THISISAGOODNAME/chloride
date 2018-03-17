using JetBrains.Annotations;

namespace AnotherWheel.Models.Vmd {
    public sealed class IKControl {

        [NotNull]
        public string Name { get; internal set; }

        public bool Enabled { get; internal set; }

    }
}
