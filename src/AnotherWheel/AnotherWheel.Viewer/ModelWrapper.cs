using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AnotherWheel.Models.Pmx;
using AnotherWheel.Viewer.Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer {
    public sealed class ModelWrapper : DisposableBase {

        private ModelWrapper() {
        }

        [NotNull]
        public static ModelWrapper Convert([NotNull] PmxModel pmxModel, [NotNull] IReadOnlyDictionary<string, Texture2D> textureMap, [NotNull] GraphicsDevice graphicsDevice) {
            var wrapper = new ModelWrapper();

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

            wrapper._createdVertexBuffers.Add(vertexBuffer);
            #endregion

            #region Index buffer
            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, faceTriangles.Count, BufferUsage.WriteOnly);
            var indices = new int[faceTriangles.Count];

            for (var i = 0; i < faceTriangles.Count; ++i) {
                indices[i] = faceTriangles[i];
            }

            indexBuffer.SetData(indices);

            wrapper._createdIndexBuffers.Add(indexBuffer);
            #endregion

            var currentFaceStartIndex = 0;

            #region Construct base structure for mesh parts
            // WTF
            // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/ModelMeshPart.cs#L26-L33
            // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/ModelMesh.cs#L21-L23
            // If we don't create a number of ModelMeshPart, add them to a ModelMesh first, when setting ModelMeshPart.Effect, it throws a NullReferenceException.
            var modelMeshParts = new List<ModelMeshPart>(pmxMaterials.Count);

            for (var i = 0; i < pmxMaterials.Count; ++i) {
                modelMeshParts.Add(new ModelMeshPart());
            }

            var modelMesh = new ModelMesh(graphicsDevice, modelMeshParts);

            var modelMeshes = new List<ModelMesh> {
                modelMesh
            };
            #endregion

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

                wrapper._createdEffects.Add(effect);

                var meshPart = modelMeshParts[index];

                meshPart.Effect = effect;
                meshPart.VertexBuffer = vertexBuffer;
                meshPart.VertexOffset = 0;
                meshPart.NumVertices = vertexBuffer.VertexCount;
                meshPart.StartIndex = currentFaceStartIndex;
                meshPart.IndexBuffer = indexBuffer;
                meshPart.PrimitiveCount = pmxMaterial.AppliedFaceVertexCount / 3;

                currentFaceStartIndex += pmxMaterial.AppliedFaceVertexCount;
            }

            Debug.Assert(currentFaceStartIndex == pmxModel.FaceTriangles.Count, "currentFaceStartIndex == pmxModel.FaceTriangles.Count");

            var modelBones = new List<ModelBone>(pmxModel.Bones.Count);

            for (var i = 0; i < pmxModel.Bones.Count; ++i) {
                modelBones.Add(new ModelBone());
            }

            for (var i = 0; i < pmxModel.Bones.Count; ++i) {
                var pmxBone = pmxModel.Bones[i];
                var bone = modelBones[i];

                bone.Index = i;
                bone.Name = pmxBone.Name;
                bone.Transform = pmxBone.WorldMatrix;
            }

            for (var i = 0; i < pmxModel.Bones.Count; ++i) {
                var pmxBone = pmxModel.Bones[i];
                var bone = modelBones[i];

                if (pmxBone.ParentBoneIndex >= 0) {
                    bone.Parent = modelBones[pmxBone.ParentBoneIndex];
                }
            }

            var rootBone = modelBones[pmxModel.RootBoneIndices[0]];

            rootBone.AddMesh(modelMesh);

            modelMesh.ParentBone = rootBone;

            var model = new Model(graphicsDevice, modelBones, modelMeshes);

            model.Root = rootBone;

            wrapper.Model = model;

            return wrapper;
        }

        [NotNull]
        public Model Model { get; private set; }

        protected override void Dispose(bool disposing) {
            foreach (var effect in _createdEffects) {
                effect.Dispose();
            }

            foreach (var indexBuffer in _createdIndexBuffers) {
                indexBuffer.Dispose();
            }

            foreach (var vertexBuffer in _createdVertexBuffers) {
                vertexBuffer.Dispose();
            }
        }

        private readonly List<IndexBuffer> _createdIndexBuffers = new List<IndexBuffer>();
        private readonly List<VertexBuffer> _createdVertexBuffers = new List<VertexBuffer>();
        private readonly List<Effect> _createdEffects = new List<Effect>();

    }
}
