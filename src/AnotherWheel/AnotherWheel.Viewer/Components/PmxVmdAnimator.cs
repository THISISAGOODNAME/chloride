using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AnotherWheel.Models.Pmx;
using AnotherWheel.Models.Pmx.Extensions;
using AnotherWheel.Models.Vmd;
using AnotherWheel.Models.Vmd.Extensions;
using AnotherWheel.Viewer.Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer.Components {
    public sealed class PmxVmdAnimator : GameComponent {

        public PmxVmdAnimator([NotNull] Game game)
            : base(game) {
        }

        public void InitializeContents([NotNull] PmxModel pmxModel, [NotNull] VmdMotion vmdMotion) {
            _pmxModel = pmxModel;
            _vmdMotion = vmdMotion;

            foreach (var boneFrame in vmdMotion.BoneFrames) {
                if (!_lastBoneFrames.ContainsKey(boneFrame.Name)) {
                    _lastBoneFrames[boneFrame.Name] = boneFrame;
                }

                List<VmdBoneFrame> cachedBoneFrames;

                if (!_boneFrameCache.ContainsKey(boneFrame.Name)) {
                    cachedBoneFrames = new List<VmdBoneFrame>();
                    _boneFrameCache[boneFrame.Name] = cachedBoneFrames;
                } else {
                    cachedBoneFrames = _boneFrameCache[boneFrame.Name];
                }

                cachedBoneFrames.Add(boneFrame);
            }

            _boneFrameNames = _lastBoneFrames.Keys.ToArray();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            var renderer = Game.SimpleFindComponentOf<PmxRenderer>();

            Trace.Assert(renderer != null);

            UpdateVerticesCallback(renderer.Vertices);

            ++_frameCounter;
        }

        private unsafe void UpdateVerticesCallback([NotNull] VertexPositionNormalTexture[] vertices) {
            var pmxModel = _pmxModel;
            var frameCounter = _frameCounter;

            var currentBoneFrameValues = new Dictionary<string, VmdBoneFrame>(_boneFrameNames.Length);

            // Calculate all current bone frame values by linear interpolation.
            foreach (var name in _boneFrameNames) {
                var lastBoneFrame = _lastBoneFrames[name];
                var nextBoneFrame = _boneFrameCache[name].Find(frame => frame.FrameIndex > lastBoneFrame.FrameIndex);

                if (nextBoneFrame != null && nextBoneFrame.FrameIndex == frameCounter) {
                    lastBoneFrame = nextBoneFrame;
                    nextBoneFrame = _boneFrameCache[name].Find(frame => frame.FrameIndex > lastBoneFrame.FrameIndex);
                    _lastBoneFrames[name] = lastBoneFrame;
                }

                if (nextBoneFrame == null) {
                    // The animation has stopped.
                    currentBoneFrameValues[name] = lastBoneFrame;
                } else {
                    var t = (float)(frameCounter - lastBoneFrame.FrameIndex) / (nextBoneFrame.FrameIndex - lastBoneFrame.FrameIndex);
                    var interpFrame = lastBoneFrame.Lerp(nextBoneFrame, t);

                    currentBoneFrameValues[name] = interpFrame;
                }
            }

            // Calculate, set bone animation position/rotation, ...
            foreach (var pmxBone in pmxModel.Bones) {
                if (!currentBoneFrameValues.ContainsKey(pmxBone.Name)) {
                    continue;
                }

                var boneFrame = currentBoneFrameValues[pmxBone.Name];
                pmxBone.SetAnimationValue(boneFrame.Position, boneFrame.Rotation);
            }

            // ... and update those values.
            pmxModel.RecalculateAllBoneHierarchies();

            // Now apply the bone transforms to our vertices.
            var vertexCount = pmxModel.Vertices.Count;

            fixed (VertexPositionNormalTexture* pVertices = vertices) {
                for (var i = 0; i < vertexCount; ++i) {
                    var pmxVertex = pmxModel.Vertices[i];
                    var boneWeights = pmxVertex.BoneWeights;

                    var validBoneCount = 0;

                    for (var j = 0; j < PmxVertex.MaxBoneWeightCount; ++j) {
                        if (boneWeights[j].IsValid) {
                            ++validBoneCount;
                        } else {
                            break;
                        }
                    }

                    if (validBoneCount == 0) {
                        continue;
                    }

                    float weightTotal = 0;

                    for (var j = 0; j < validBoneCount; ++j) {
                        weightTotal += boneWeights[j].Weight;
                    }

                    var transform = new Matrix();

                    for (var j = 0; j < validBoneCount; ++j) {
                        var pmxBone = pmxModel.Bones[boneWeights[j].BoneIndex];
                        var w = boneWeights[j].Weight / weightTotal;

                        transform += pmxBone.WorldMatrix * w;
                    }

                    var finalPos = Vector3.Transform(pmxVertex.Position, transform);

                    // TODO: Transform normals plz.
                    var pv = pVertices + i;

                    pv->Position = finalPos;
                }
            }
        }

        private PmxModel _pmxModel;
        private VmdMotion _vmdMotion;

        private string[] _boneFrameNames;
        private readonly Dictionary<string, List<VmdBoneFrame>> _boneFrameCache = new Dictionary<string, List<VmdBoneFrame>>();
        private readonly Dictionary<string, VmdBoneFrame> _lastBoneFrames = new Dictionary<string, VmdBoneFrame>();

        private int _frameCounter;

    }
}

