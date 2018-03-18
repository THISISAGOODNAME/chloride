using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx.Extensions {
    public static class PmxBoneExtensions {

        public static bool HasFlag([NotNull] this PmxBone bone, BoneFlags flag) {
            return (bone.Flags & flag) != 0;
        }

        public static void SetLocalRotationAxes([NotNull] this PmxBone bone, Vector3 localX, Vector3 localY, Vector3 localZ) {
            var rotationMatrix = new Matrix(
                localX.X, localX.Y, localX.Z, 0,
                localY.X, localY.Y, localY.Z, 0,
                localZ.X, localZ.Y, localZ.Z, 0,
                0, 0, 0, 1);

            bone.Rotation = Quaternion.CreateFromRotationMatrix(rotationMatrix);
        }

        public static void SetAnimationValue([NotNull] this PmxBone bone, Vector3 position, Quaternion rotation) {
            bone.RelativePosition = position;
            bone.Rotation = rotation;
        }

        internal static void CalculateHierarchy([NotNull] this PmxBone bone) {
            if (bone.HierarchyCalculated) {
                return;
            }

            var localMatrix = Matrix.CreateFromQuaternion(bone.Rotation);

            bone.LocalMatrix = localMatrix;

            Matrix worldMatrix;

            if (bone.ReferenceParent != null) {
                bone.ReferenceParent.CalculateHierarchy();
                bone.RelativePosition = bone.Position - bone.ReferenceParent.Position;
                worldMatrix = localMatrix * bone.ReferenceParent.WorldMatrix;
            } else {
                // Already on top
                bone.RelativePosition = bone.Position;
                worldMatrix = bone.LocalMatrix;
            }

            worldMatrix.M41 = bone.Position.X;
            worldMatrix.M42 = bone.Position.Y;
            worldMatrix.M43 = bone.Position.Z;
            worldMatrix.M44 = 1;

            bone.WorldMatrix = worldMatrix;

            if (bone.ReferenceParent != null) {
                var invWorld = Matrix.Invert(bone.ReferenceParent.WorldMatrix);
                bone.LocalMatrix = bone.WorldMatrix * invWorld;
            } else {
                bone.LocalMatrix = bone.WorldMatrix;
            }

            bone.HierarchyCalculated = true;
        }

    }
}
