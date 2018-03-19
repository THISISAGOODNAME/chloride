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

            UpdateVertices(renderer.Vertices);

            ++_frameCounter;
        }

        private void UpdateVertices([NotNull] VertexPositionNormalTexture[] vertices) {
            var pmxModel = _pmxModel;
            var frameCounter = _frameCounter;

            // Calculate all current bone frame values by linear interpolation.
            foreach (var name in _boneFrameNames) {
                var lastBoneFrame = _lastBoneFrames[name];
                var nextBoneFrame = _boneFrameCache[name].Find(frame => frame.FrameIndex > lastBoneFrame.FrameIndex);

                if (nextBoneFrame != null && (int)(nextBoneFrame.FrameIndex * FrameRateRatio) == frameCounter) {
                    lastBoneFrame = nextBoneFrame;
                    nextBoneFrame = _boneFrameCache[name].Find(frame => frame.FrameIndex > lastBoneFrame.FrameIndex);
                    _lastBoneFrames[name] = lastBoneFrame;
                }

                var extendedLastBoneFrameIndex = (int)(lastBoneFrame.FrameIndex * FrameRateRatio);

                if (nextBoneFrame == null) {
                    // The animation has stopped.
                    _currentBoneFrames[name] = lastBoneFrame.CopyWithDifferentFrameIndex((int)(lastBoneFrame.FrameIndex * FrameRateRatio));
                } else {
                    var extendedNextBoneFrameIndex = (int)(nextBoneFrame.FrameIndex * FrameRateRatio);
                    var t = (float)(frameCounter - extendedLastBoneFrameIndex) / (extendedNextBoneFrameIndex - extendedLastBoneFrameIndex);
                    var interpFrame = lastBoneFrame.Lerp(nextBoneFrame, t, FrameRateRatio);

                    _currentBoneFrames[name] = interpFrame;
                }
            }

            // Calculate, set bone animated position & rotation, ...
            foreach (var pmxBone in pmxModel.Bones) {
                if (!_currentBoneFrames.ContainsKey(pmxBone.Name)) {
                    continue;
                }

                var boneFrame = _currentBoneFrames[pmxBone.Name];

                pmxBone.SetVmdAnimation(boneFrame.Position, boneFrame.Rotation);
            }

            // ... and update those values.
            pmxModel.RecalculateAllBoneInfo();

            // Now apply the bone transforms to our vertices.
            var vertexCount = pmxModel.Vertices.Count;

            unsafe {
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

                        var transform = EmptyMatrix;

                        for (var j = 0; j < validBoneCount; ++j) {
                            var pmxBone = pmxModel.Bones[boneWeights[j].BoneIndex];
                            var w = boneWeights[j].Weight / weightTotal;

                            transform += pmxBone.SkinMatrix * w;
                        }

                        var finalPos = Vector3.Transform(pmxVertex.Position, transform);

                        // TODO: Transform normals plz.
                        var pv = pVertices + i;

                        pv->Position = finalPos;
                    }
                }
            }
        }

        private static readonly float TargetFrameRate = 60f;
        private const float VmdStandardFrameRate = 30f;
        private static readonly float FrameRateRatio = TargetFrameRate / VmdStandardFrameRate;

        private static readonly Matrix EmptyMatrix = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        private PmxModel _pmxModel;
        private VmdMotion _vmdMotion;

        private string[] _boneFrameNames;
        private readonly Dictionary<string, List<VmdBoneFrame>> _boneFrameCache = new Dictionary<string, List<VmdBoneFrame>>();
        private readonly Dictionary<string, VmdBoneFrame> _lastBoneFrames = new Dictionary<string, VmdBoneFrame>();
        private readonly Dictionary<string, VmdBoneFrame> _currentBoneFrames = new Dictionary<string, VmdBoneFrame>();

        private int _frameCounter;

    }
}

