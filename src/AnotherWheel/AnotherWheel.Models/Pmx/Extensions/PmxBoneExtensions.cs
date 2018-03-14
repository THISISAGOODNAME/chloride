using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx.Extensions {
    public static class PmxBoneExtensions {

        public static bool HasFlag([NotNull] this PmxBone bone, BoneFlags flag) {
            return (bone.Flags & flag) != 0;
        }

        internal static void CalculateHierarchy([NotNull] this PmxBone bone) {
            if (bone.HierarchyCalculated) {
                return;
            }

            // TODO: Right-handed or left-handed?
            var localMatrix = new Matrix(
                bone.LocalX.X, bone.LocalX.Y, bone.LocalX.Z, 0,
                bone.LocalY.X, bone.LocalY.Y, bone.LocalY.Z, 0,
                bone.LocalZ.X, bone.LocalZ.Y, bone.LocalZ.Z, 0,
                0, 0, 0, 1);

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
