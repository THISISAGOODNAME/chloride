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
            bone.Position = position;
            bone.Rotation = rotation;
        }

        public static Matrix SetTransform(Vector3 pos, Quaternion rot) {
            Matrix resMat = Matrix.CreateFromQuaternion(rot);
            resMat.M41 = pos.X;
            resMat.M42 = pos.Y;
            resMat.M43 = pos.Z;
            resMat.M44 = 1;

            return resMat;
        }

        internal static void CalculateHierarchy([NotNull] this PmxBone bone) {
            if (bone.HierarchyCalculated) {
                return;
            }

            var pos = bone.Position;
            var rot = bone.Rotation;

            //bone.LocalMatrix = SetTransform(pos, rot);

            if (bone.ReferenceParent != null)
            {
                bone.ReferenceParent.CalculateHierarchy();

                bone.RelativePosition = pos - bone.ReferenceParent.Position;

                //rot = bone.ReferenceParent.Rotation + rot;

                // FIXME: bone.RelativePosition = pos - tail position ?
                bone.RelativePosition = new Vector3(0f, 0f, 0f);

                bone.LocalMatrix = SetTransform(bone.RelativePosition, rot);

                //bone.WorldMatrix = bone.ReferenceParent.WorldMatrix * bone.LocalMatrix;
                bone.WorldMatrix = bone.LocalMatrix;
            }
            else
            {
                bone.RelativePosition = pos;
                //bone.RelativePosition = new Vector3(0f, 0f, 0f);
                bone.LocalMatrix = SetTransform(bone.RelativePosition, rot);
                bone.WorldMatrix = bone.LocalMatrix;
            }

            /**
            var localMatrix = Matrix.CreateFromQuaternion(bone.Rotation);

            Matrix worldMatrix;

            var pos = bone.Position;

            //pos *= 0f;

            if (bone.ReferenceParent != null) {
                bone.ReferenceParent.CalculateHierarchy();

                bone.RelativePosition = pos - bone.ReferenceParent.Position;

                //bone.RelativePosition = bone.ReferenceParent.Position;

                worldMatrix = localMatrix * bone.ReferenceParent.WorldMatrix;
            } else {
                // Already on top
                bone.RelativePosition = pos;
                //bone.RelativePosition = new Vector3(0f, 0f, 0f);
                //bone.RelativePosition *= 0.0f;
                worldMatrix = localMatrix;
            }

            worldMatrix.M41 = pos.X;
            worldMatrix.M42 = pos.Y;
            worldMatrix.M43 = pos.Z;
            worldMatrix.M44 = 1;

            bone.WorldMatrix = worldMatrix;

            if (bone.ReferenceParent != null) {
                //var invWorld = Matrix.Invert(bone.ReferenceParent.WorldMatrix);
                //bone.LocalMatrix = worldMatrix * invWorld;

                localMatrix.M41 = pos.X;
                localMatrix.M42 = pos.Y;
                localMatrix.M43 = pos.Z;
                localMatrix.M44 = 1;

                bone.LocalMatrix = localMatrix;
                
            } else {
                bone.LocalMatrix = worldMatrix;
            }

            */



            bone.HierarchyCalculated = true;
        }

    }
}
