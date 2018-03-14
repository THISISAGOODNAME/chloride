using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx {
    public interface IPmxNamedObject {

        [NotNull]
        string Name { get; }

        [NotNull]
        string NameEnglish { get; }

    }
}
