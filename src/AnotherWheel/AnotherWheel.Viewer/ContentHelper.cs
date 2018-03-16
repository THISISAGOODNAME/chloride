using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer {
    public static class ContentHelper {

        [CanBeNull]
        public static Texture2D LoadTexture([NotNull] GraphicsDevice graphicsDevice, [NotNull] string filePath) {
            if (!File.Exists(filePath)) {
                return null;
            }

            Texture2D texture;

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                texture = Texture2D.FromStream(graphicsDevice, fileStream);
            }

            return texture;
        }

    }
}
