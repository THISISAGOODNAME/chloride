using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Extensions {
    public static class MatrixExtensions {

        public static Vector4 Row1(this Matrix matrix) {
            return new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
        }

        public static Vector4 Row2(this Matrix matrix) {
            return new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
        }

        public static Vector4 Row3(this Matrix matrix) {
            return new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
        }

    }
}
