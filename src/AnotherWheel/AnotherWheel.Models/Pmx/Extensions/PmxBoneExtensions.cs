using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Pmx.Extensions {
    public static class PmxBoneExtensions {

        public static bool HasFlag([NotNull] this PmxBone bone, BoneFlags flag) {
            return (bone.Flags & flag) != 0;
        }

        public static void SetVmdAnimation([NotNull] this PmxBone bone, Vector3 vmdTranslation, Quaternion vmdRotation) {
            bone.AnimatedTranslation = vmdTranslation;
            bone.AnimatedRotation = vmdRotation;
        }

        internal static void SetInitialRotationFromRotationAxes([NotNull] this PmxBone bone, Vector3 localX, Vector3 localY, Vector3 localZ) {
            var rotationMatrix = new Matrix(
                localX.X, localX.Y, localX.Z, 0,
                localY.X, localY.Y, localY.Z, 0,
                localZ.X, localZ.Y, localZ.Z, 0,
                0, 0, 0, 1);

            bone.InitialRotation = Quaternion.CreateFromRotationMatrix(rotationMatrix);
            bone.CurrentRotation = bone.InitialRotation;
        }

        // https://github.com/sn0w75/MMP/blob/master/libmmp/motioncontroller.cpp
        internal static void SetToVmdPose([NotNull] this PmxBone bone) {
            if (bone.IsTransformCalculated) {
                return;
            }

            bone.CurrentRotation = bone.AnimatedRotation;

            var parent = bone.ParentBone;

            Vector3 localPosition;

            if (parent == null) {
                localPosition = bone.AnimatedTranslation + bone.InitialPosition;
            } else {
                parent.SetToVmdPose();
                localPosition = bone.AnimatedTranslation + bone.InitialPosition - parent.InitialPosition;
            }

            bone.LocalMatrix = CalculateTransform(localPosition, bone.CurrentRotation);
            bone.WorldMatrix = bone.CalculateWorldMatrix();

            bone.IsTransformCalculated = true;
        }

        // Set to binding pose ("T" pose)
        internal static void SetToBindingPose([NotNull] this PmxBone bone) {
            if (bone.IsTransformCalculated) {
                return;
            }

            var parent = bone.ParentBone;

            Vector3 localPosition;

            if (parent == null) {
                localPosition = bone.InitialPosition;
            } else {
                parent.SetToBindingPose();
                localPosition = bone.InitialPosition - parent.InitialPosition;
            }

            bone.LocalMatrix = CalculateTransform(localPosition, Quaternion.Identity);
            bone.WorldMatrix = bone.CalculateWorldMatrix();

            bone.IsTransformCalculated = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void UpdateSkinMatrix([NotNull] this PmxBone bone) {
            bone.SkinMatrix = bone.BindPoseMatrixInverse * bone.WorldMatrix;
            bone.CurrentPosition = Vector3.Transform(bone.InitialPosition, bone.SkinMatrix);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Matrix CalculateWorldMatrix([NotNull] this PmxBone bone) {
            if (bone.ParentBone != null) {
                return bone.LocalMatrix * bone.ParentBone.WorldMatrix;
            } else {
                return bone.LocalMatrix;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Matrix CalculateTransform(Vector3 translation, Quaternion rotation) {
            var translationMatrix = Matrix.CreateTranslation(translation);
            var rotationanMatrix = Matrix.CreateFromQuaternion(rotation);

            return rotationanMatrix * translationMatrix;
        }

    }
}
