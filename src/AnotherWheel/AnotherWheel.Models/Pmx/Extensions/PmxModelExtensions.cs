using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx.Extensions {
    public static class PmxModelExtensions {

        public static void RecalculateAllBoneInfo([NotNull] this PmxModel pmxModel) {
            for (var i = 0; i < pmxModel.Bones.Count; i++) {
                var bone = pmxModel.Bones[i];
                bone.IsTransformCalculated = false;
            }

            for (var i = 0; i < pmxModel.Bones.Count; i++) {
                var bone = pmxModel.Bones[i];

                // VMD controlled pose
                bone.SetToVmdPose();
                // Binding pose
                //bone.SetToBindingPose();
            }

            for (var i = 0; i < pmxModel.Bones.Count; i++) {
                var bone = pmxModel.Bones[i];
                bone.UpdateSkinMatrix();
            }
        }

    }
}
