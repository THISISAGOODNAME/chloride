using AnotherWheel.Models.Extensions;
using AnotherWheel.Models.Pmx;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Overlay;

namespace AnotherWheel.Viewer.Components {
    public sealed class BoneDebugVisualizer : DrawableGameComponent {

        public BoneDebugVisualizer([NotNull] Game game)
            : base(game) {
            _graphics = new Graphics(game.GraphicsDevice);
            _fontManager = new FontManager();
        }

        public bool SkeletonVisible { get; set; } = true;

        public bool BoneNamesVisible { get; set; } = true;

        public void InitializeContents([NotNull] PmxModel pmxModel, [NotNull] Camera camera, [NotNull] SpriteBatch spriteBatch) {
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
                vertices[i] = new VertexPosition(pmxModel.Bones[i].CurrentPosition);
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

            _spriteBatch = spriteBatch;

            _boneNameFont = _fontManager.CreateFont(DefaultUIFontFamilyName, FontStyle.Regular);
            _boneNameFont.Size = DefaultUIFontSize;

            _boneNameBrush = new SolidBrush(Color.Black);
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            var graphicsDevice = GraphicsDevice;
            var camera = _camera;

            if (SkeletonVisible) {
                graphicsDevice.DepthStencilState = DepthAlwaysPass;
                graphicsDevice.BlendState = BlendState.NonPremultiplied;
                graphicsDevice.RasterizerState = RasterizerState.CullNone;

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

            if (BoneNamesVisible) {
                var spriteBatch = _spriteBatch;
                var graphics = _graphics;
                var viewProjection = camera.ViewMatrix * camera.ProjectionMatrix;
                var viewport = graphicsDevice.Viewport;

                spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthAlwaysPass, rasterizerState: RasterizerState.CullNone);

                graphics.Clear(Color.Transparent);

                foreach (var bone in _pmxModel.Bones) {
                    var bonePositionInScreen = WorldToScreen(viewProjection, bone.CurrentPosition, viewport, out var shouldDraw);

                    if (shouldDraw) {
                        graphics.FillString(_boneNameBrush, _boneNameFont, bone.Name, bonePositionInScreen.XY());
                    }
                }

                graphics.UpdateBackBuffer();
                spriteBatch.Draw(graphics.BackBuffer, Vector2.Zero, Color.White);

                spriteBatch.End();
            }
        }

        protected override void Dispose(bool disposing) {
            _effect?.Dispose();
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();

            _boneNameBrush.Dispose();
            _graphics.Dispose();
            _fontManager.Dispose();

            base.Dispose(disposing);
        }

        private void UpdateVertexData() {
            var vertices = _vertices;
            var pmxModel = _pmxModel;
            var bones = pmxModel.Bones;

            for (var i = 0; i < bones.Count; ++i) {
                vertices[i] = new VertexPosition(bones[i].CurrentPosition);
            }

            _vertexBuffer.SetData(vertices);
        }

        private static readonly DepthStencilState DepthAlwaysPass = new DepthStencilState {
            DepthBufferEnable = true,
            DepthBufferFunction = CompareFunction.Always,
            DepthBufferWriteEnable = true
        };

        private static Vector3 WorldToScreen(Matrix viewProjection, Vector3 worldPosition, Viewport viewport, out bool shouldDraw) {
            var pos = new Vector4(worldPosition, 1.0f);
            var positionTransformed = Vector4.Transform(pos, viewProjection);

            if (positionTransformed.W.Equals(0)) {
                shouldDraw = false;
                return Vector3.Zero;
            }

            positionTransformed /= positionTransformed.W;

            var x = viewport.X + (1.0f + positionTransformed.X) * viewport.Width / 2.0f;
            var y = viewport.Y + (1.0f - positionTransformed.Y) * viewport.Height / 2.0f;
            var z = viewport.MinDepth + positionTransformed.Z * (viewport.MaxDepth - viewport.MinDepth);

            shouldDraw = viewport.MinDepth <= z && z <= viewport.MaxDepth;

            return new Vector3(x, y, z);
        }

        private PmxModel _pmxModel;

        private Camera _camera;

        private (int ParentIndex, int ChildIndex)[] _boneSegmentPairs;
        private VertexPosition[] _vertices;
        private int[] _indices;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private BasicEffect _effect;

        private readonly Graphics _graphics;
        private readonly FontManager _fontManager;
        private Font _boneNameFont;
        private SolidBrush _boneNameBrush;

        private const float DefaultUIFontSize = 12f;
        private const string DefaultUIFontFamilyName = "Microsoft YaHei";

        private SpriteBatch _spriteBatch;

    }
}
