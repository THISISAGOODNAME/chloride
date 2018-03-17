using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer.Internal {
    internal sealed class MeshPart : DisposableBase {

        [NotNull]
        public Effect Effect { get; set; }

        public int VertexStart { get; set; }

        public int IndexStart { get; set; }

        public int TriangleCount { get; set; }

        [NotNull]
        public VertexBuffer VertexBuffer { get; set; }

        [NotNull]
        public IndexBuffer IndexBuffer { get; set; }

        public void Draw([NotNull] GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection) {
            if (TriangleCount <= 0) {
                return;
            }

            var effect = Effect;

            if (!(effect is IEffectMatrices matrices)) {
                return;
            }

            matrices.World = world;
            matrices.View = view;
            matrices.Projection = projection;

            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.Indices = IndexBuffer;

            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, VertexStart, IndexStart, TriangleCount);
            }
        }

        protected override void Dispose(bool disposing) {
            Effect?.Dispose();
        }

    }
}
