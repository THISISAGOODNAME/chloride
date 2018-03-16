using System.Collections.Generic;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxNode : IPmxNamedObject {

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<NodeElement> Elements { get; internal set; }

        internal bool IsSystemNode { get; set; }

    }
}
