using AnotherWheel.Models.Pmx;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer.Components {
    public sealed class BoneDebugVisualizer : DrawableGameComponent {

        public BoneDebugVisualizer([NotNull] Game game)
            : base(game) {
        }

        public void InitializeContents([NotNull] PmxModel pmxModel, [NotNull] Camera camera) {
            _pmxModel = pmxModel;
            _camera = camera;

            var boneSegmentCount = pmxModel.Bones.Count - pmxModel.RootBoneIndices.Count;
            var boneSegmentPairs = new(int ParentIndex, int ChildIndex)[boneSegmentCount];
            var boneSegmentPairCounter = 0;

            for (var i = 0; i < pmxModel.Bones.Count; i++) {
                var pmxBone = pmxModel.Bones[i];

                if (pmxBone.ParentBoneIndex < 0) {
                    continue;
                }

                boneSegmentPairs[boneSegmentPairCounter] = (pmxBone.ParentBoneIndex, i);

                ++boneSegmentPairCounter;
            }

            _boneSegmentPairs = boneSegmentPairs;

            var indices = new int[boneSegmentCount * 2];

            for (var i = 0; i < boneSegmentCount; ++i) {
                (indices[i * 2], indices[i * 2 + 1]) = boneSegmentPairs[i];
            }

            _indices = indices;

            var vertices = new VertexPosition[pmxModel.Bones.Count];

            for (var i = 0; i < vertices.Length; ++i) {
                vertices[i] = new VertexPosition(pmxModel.Bones[i].Position);
            }

            _vertices = vertices;

            var vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPosition.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, vertices.Length, BufferUsage.WriteOnly);

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);

            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;

            var effect = new BasicEffect(GraphicsDevice);

            effect.DiffuseColor = Color.Red.ToVector3();
            effect.Alpha = 1.0f;

            _effect = effect;
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            var graphicsDevice = GraphicsDevice;

            graphicsDevice.DepthStencilState = DepthAlwaysPass;
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            var camera = _camera;

            _effect.World = Matrix.Identity;
            _effect.View = camera.ViewMatrix;
            _effect.Projection = camera.ProjectionMatrix;

            UpdateVertexData();

            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            foreach (var pass in _effect.CurrentTechnique.Passes) {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, _indexBuffer.IndexCount / 2);
            }
        }

        protected override void Dispose(bool disposing) {
            _effect?.Dispose();
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();

            base.Dispose(disposing);
        }

        private void UpdateVertexData() {
            var vertices = _vertices;
            var pmxModel = _pmxModel;
            var bones = pmxModel.Bones;

            for (var i = 0; i < bones.Count; ++i) {
                var transformed = Vector3.Transform(bones[i].Position, bones[i].WorldMatrix);

                vertices[i] = new VertexPosition(transformed);
            }

            _vertexBuffer.SetData(vertices);
        }

        private static readonly DepthStencilState DepthAlwaysPass = new DepthStencilState {
            DepthBufferEnable = true,
            DepthBufferFunction = CompareFunction.Always,
            DepthBufferWriteEnable = true
        };

        private PmxModel _pmxModel;

        private Camera _camera;

        private (int ParentIndex, int ChildIndex)[] _boneSegmentPairs;
        private VertexPosition[] _vertices;
        private int[] _indices;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private BasicEffect _effect;

    }
}
