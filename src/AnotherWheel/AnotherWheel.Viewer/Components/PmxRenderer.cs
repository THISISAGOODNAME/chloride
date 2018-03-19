using System;
using System.Collections.Generic;
using System.Diagnostics;
using AnotherWheel.Models.Extensions;
using AnotherWheel.Models.Pmx;
using AnotherWheel.Viewer.Extensions;
using AnotherWheel.Viewer.Internal;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer.Components {
    public sealed class PmxRenderer : DrawableGameComponent {

        public PmxRenderer([NotNull] Game game)
            : base(game) {
        }

        public void InitializeContents([NotNull] PmxModel pmxModel, [NotNull] Camera camera, [NotNull] IReadOnlyDictionary<string, Texture2D> textureMap) {
            _pmxModel = pmxModel;
            _camera = camera;
            _textureMap = textureMap;

            var graphicsDevice = GraphicsDevice;

            var pmxMaterials = pmxModel.Materials;
            var faceTriangles = pmxModel.FaceTriangles;
            var pmxVertices = pmxModel.Vertices;

            #region Vertex buffer
            var vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, pmxVertices.Count, BufferUsage.WriteOnly);
            var vertices = new VertexPositionNormalTexture[pmxVertices.Count];

            for (var i = 0; i < pmxVertices.Count; ++i) {
                var v = pmxVertices[i];
                vertices[i] = new VertexPositionNormalTexture(v.Position, v.Normal, v.UV);
            }

            vertexBuffer.SetData(vertices);
            _vertices = vertices;
            _vertexBuffer = vertexBuffer;
            #endregion

            #region Index buffer
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, faceTriangles.Count, BufferUsage.WriteOnly);
            var indices = new int[faceTriangles.Count];

            for (var i = 0; i < faceTriangles.Count; ++i) {
                indices[i] = faceTriangles[i];
            }

            indexBuffer.SetData(indices);
            _indices = indices;
            _indexBuffer = indexBuffer;
            #endregion

            var currentFaceStartIndex = 0;

            for (var index = 0; index < pmxMaterials.Count; index++) {
                var pmxMaterial = pmxMaterials[index];
                var effect = new BasicEffect(graphicsDevice);
                // A white pixel texture is set with key = string.Empty.
                var textureKey = textureMap.ContainsKey(pmxMaterial.TextureFileName) ? pmxMaterial.TextureFileName : string.Empty;

                effect.AmbientLightColor = pmxMaterial.Ambient;
                effect.DiffuseColor = pmxMaterial.Diffuse.XYZ();
                effect.Alpha = pmxMaterial.Diffuse.W;
                effect.SpecularColor = pmxMaterial.Specular;
                effect.SpecularPower = pmxMaterial.SpecularPower;
                effect.TextureEnabled = true;
                effect.Texture = textureMap[textureKey];

                var meshPart = new MeshPart();

                meshPart.Effect = effect;
                meshPart.VertexBuffer = vertexBuffer;
                meshPart.VertexStart = 0;
                meshPart.IndexBuffer = indexBuffer;
                meshPart.IndexStart = currentFaceStartIndex;
                meshPart.TriangleCount = pmxMaterial.AppliedFaceVertexCount / 3;

                _meshParts.Add(meshPart);

                currentFaceStartIndex += pmxMaterial.AppliedFaceVertexCount;
            }

            Debug.Assert(currentFaceStartIndex == pmxModel.FaceTriangles.Count, "currentFaceStartIndex == pmxModel.FaceTriangles.Count");

#if DEBUG
            Debug.Print("PMX bone list:");

            foreach (var pmxBone in pmxModel.Bones) {
                Debug.Print(pmxBone.ToString());
            }
#endif
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            var camera = _camera;
            var graphicsDevice = GraphicsDevice;

            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            var world = Matrix.Identity;
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            _vertexBuffer.SetData(_vertices);

            foreach (var part in _meshParts) {
                part.Draw(graphicsDevice, world, view, projection);
            }
        }

        internal VertexPositionNormalTexture[] Vertices => _vertices;

        protected override void Dispose(bool disposing) {
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();

            foreach (var part in _meshParts) {
                part.Dispose();
            }

            base.Dispose(disposing);
        }

        [ItemNotNull]
        private readonly List<MeshPart> _meshParts = new List<MeshPart>();

        private PmxModel _pmxModel;
        private Camera _camera;
        private IReadOnlyDictionary<string, Texture2D> _textureMap;

        private VertexPositionNormalTexture[] _vertices;
        private int[] _indices;

        private IndexBuffer _indexBuffer;
        private VertexBuffer _vertexBuffer;

    }
}
