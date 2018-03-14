using System.Collections.Generic;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxMorph : IPmxNamedObject {

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        public int Panel { get; internal set; }

        public MorphOffsetKind OffsetKind { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<PmxBaseMorph> Offsets { get; internal set; }

    }
}
