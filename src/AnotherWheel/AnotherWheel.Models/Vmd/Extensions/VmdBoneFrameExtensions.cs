using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Vmd.Extensions {
    public static class VmdBoneFrameExtensions {

        public static VmdBoneFrame Lerp([NotNull] this VmdBoneFrame thisFrame, [NotNull] VmdBoneFrame nextFrame, float t) {
            Debug.Assert(thisFrame.Name == nextFrame.Name, "thisFrame.Name == nextFrame.Name");

            t = MathHelper.Clamp(t, 0, 1);

            var frame = new VmdBoneFrame();

            frame.Name = thisFrame.Name;
            frame.FrameIndex = thisFrame.FrameIndex;
            Buffer.BlockCopy(thisFrame.Interpolation, 0, frame.Interpolation, 0, thisFrame.Interpolation.Length);

            frame.Position = Vector3.Lerp(thisFrame.Position, nextFrame.Position, t);
            frame.Rotation = Quaternion.Lerp(thisFrame.Rotation, nextFrame.Rotation, t);

            return frame;
        }

    }
}
