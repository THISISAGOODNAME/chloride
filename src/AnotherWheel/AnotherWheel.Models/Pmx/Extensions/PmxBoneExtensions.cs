using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx.Extensions {
    public static class PmxBoneExtensions {

        public static bool HasFlag([NotNull] this PmxBone bone, BoneFlags flag) {
            return (bone.Flags & flag) != 0;
        }

        public static void SetAnimationValue([NotNull] this PmxBone bone, Vector3 vmdPosition, Quaternion rotation) {
            bone.Position = bone.InitialPosition + vmdPosition;
            bone.Rotation = rotation;
        }

        internal static void SetLocalRotationAxes([NotNull] this PmxBone bone, Vector3 localX, Vector3 localY, Vector3 localZ) {
            var rotationMatrix = new Matrix(
                localX.X, localX.Y, localX.Z, 0,
                localY.X, localY.Y, localY.Z, 0,
                localZ.X, localZ.Y, localZ.Z, 0,
                0, 0, 0, 1);

            bone.InitialRotation = Quaternion.CreateFromRotationMatrix(rotationMatrix);
            bone.Rotation = bone.InitialRotation;
        }

        internal static void CalculateHierarchy([NotNull] this PmxBone bone) {
            if (bone.HierarchyCalculated) {
                return;
            }

            var parent = bone.ReferenceParent;

            if (parent == null) {
                // A root bone
                bone.RelativePosition = bone.Position;
                bone.LocalMatrix = CalculateTransform(bone.RelativePosition, bone.Rotation);
                bone.WorldMatrix = bone.LocalMatrix;
            } else {
                // A bone with parent
                parent.CalculateHierarchy();

                bone.RelativePosition = bone.Position - parent.Position;
                bone.LocalMatrix = CalculateTransform(bone.RelativePosition, bone.Rotation);
                bone.WorldMatrix = parent.WorldMatrix * bone.LocalMatrix;
            }

            bone.HierarchyCalculated = true;
        }

        private static Matrix CalculateTransform(Vector3 translation, Quaternion rotation) {
            var transformMatrix = Matrix.CreateFromQuaternion(rotation);
            transformMatrix.M41 = translation.X;
            transformMatrix.M42 = translation.Y;
            transformMatrix.M43 = translation.Z;
            transformMatrix.M44 = 1;

            return transformMatrix;
        }

    }
}
