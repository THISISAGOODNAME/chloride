using JetBrains.Annotations;

namespace AnotherWheel.Models.Vmd.Extensions {
    public static class VmdMotionExtensions {

        public static void Scale([NotNull] this VmdMotion motion, float scale) {
            foreach (var boneFrame in motion.BoneFrames) {
                boneFrame.Position *= scale;
            }

            // TODO: Other scales
        }

    }
}
