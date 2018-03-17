using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Extensions {
    public static class VectorExtensions {

        public static Vector3 XYZ(this Vector4 vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

    }
}
