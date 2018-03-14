using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx {
    public sealed class PmxMaterial : IPmxNamedObject {

        public string Name { get; internal set; }

        public string NameEnglish { get; internal set; }

        [NotNull]
        public string TextureFileName { get; internal set; }

        [NotNull]
        public string SphereTextureFileName { get; internal set; }

        [NotNull]
        public string ToonTextureFileName { get; internal set; }

        [NotNull]
        public string MemoTextureFileName { get; internal set; }

        public Vector4 Diffuse { get; internal set; }

        public Vector4 EdgeColor { get; internal set; }

        public Vector3 Specular { get; internal set; }

        public Vector3 Ambient { get; internal set; }

        public float SpecularPower { get; internal set; }

        public float EdgeSize { get; internal set; } = 1;

        public MaterialFlags Flags { get; internal set; }

        public SphereMode SphereMode { get; internal set; }

        public int FaceCount { get; internal set; }

    }

}
