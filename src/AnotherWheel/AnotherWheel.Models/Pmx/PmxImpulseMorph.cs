using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxImpulseMorph : PmxBaseMorph {

        public bool IsLocal { get; internal set; }

        public Vector3 Torque { get; internal set; }

        public Vector3 Velocity { get; internal set; }

    }
}
