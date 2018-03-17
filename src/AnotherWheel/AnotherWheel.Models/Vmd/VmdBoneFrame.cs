using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Vmd {
    public sealed class VmdBoneFrame : VmdBaseFrame {

        public VmdBoneFrame() {
            Interpolation = new byte[4, 4, 4];
        }

        [NotNull]
        public string Name { get; internal set; }

        public Vector3 Position { get; internal set; }

        public Quaternion Rotation { get; internal set; }

        [NotNull]
        public byte[,,] Interpolation { get; }

    }
}
