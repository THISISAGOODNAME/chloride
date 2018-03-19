using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Extensions {
    public static class VectorExtensions {

        // ReSharper disable once InconsistentNaming
        public static Vector3 XYZ(this Vector4 vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        // ReSharper disable once InconsistentNaming
        public static Vector2 XY(this Vector3 vector) {
            return new Vector2(vector.X, vector.Y);
        }

    }
}
