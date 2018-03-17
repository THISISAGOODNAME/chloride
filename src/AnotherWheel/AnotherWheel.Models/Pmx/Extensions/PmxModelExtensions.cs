using JetBrains.Annotations;

namespace AnotherWheel.Models.Pmx.Extensions {
    public static class PmxModelExtensions {

        public static void RecalculateAllBoneHierarchies([NotNull] this PmxModel pmxModel) {
            foreach (var bone in pmxModel.Bones) {
                bone.HierarchyCalculated = false;
            }

            foreach (var bone in pmxModel.Bones) {
                bone.CalculateHierarchy();
            }
        }

    }
}
