namespace AnotherWheel.Models.Pmx {
    public abstract class PmxBaseMorph {

        public int Index { get; internal set; } = InvalidMorphIndex;

        public const int InvalidMorphIndex = -1;

    }
}
