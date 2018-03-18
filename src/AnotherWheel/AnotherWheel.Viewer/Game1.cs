﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AnotherWheel.Models;
using AnotherWheel.Models.Pmx;
using AnotherWheel.Models.Vmd;
using AnotherWheel.Models.Vmd.Extensions;
using AnotherWheel.Viewer.Components;
using AnotherWheel.Viewer.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AnotherWheel.Viewer {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {

        public Game1() {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Contents";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            var cam = new Camera(this);

            Components.Add(cam);

            cam.Position = new Vector3(0, 15, -15);
            cam.Up = Vector3.UnitY;
            cam.LookAtTarget = cam.Up * 12;

            var ksh = new KeyboardStateHandler(this);

            ksh.KeyHold += Ksh_KeyHold;

            Components.Add(ksh);
            Components.Add(new MouseCameraControl(this));
            Components.Add(new FpsCounter(this));

            _pmxVmdAnimator = new PmxVmdAnimator(this);
            Components.Add(_pmxVmdAnimator);

            _pmxRenderer = new PmxRenderer(this);
            Components.Add(_pmxRenderer);

            _boneDebugVisualizer = new BoneDebugVisualizer(this);
            Components.Add(_boneDebugVisualizer);

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            var modelBaseDir = Content.RootDirectory;

            PmxModel pmxModel;

            using (var fileStream = File.Open(Path.Combine(modelBaseDir, "mayu.pmx"), FileMode.Open, FileAccess.Read, FileShare.Read)) {
                pmxModel = PmxReader.ReadModel(fileStream);
            }

            Debug.Assert(pmxModel != null, nameof(pmxModel) + " != null");

            foreach (var mat in pmxModel.Materials) {
                if (string.IsNullOrEmpty(mat.TextureFileName)) {
                    continue;
                }

                TryLoadTexture(mat.TextureFileName);
            }

            if (!_modelTextures.ContainsKey(string.Empty)) {
                var tex = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);

                tex.SetData(new[] {
                    0xffffffff
                });

                _modelTextures[string.Empty] = tex;
            }

            _pmxModel = pmxModel;
            _modelBaseDir = modelBaseDir;

            var camera = this.SimpleFindComponentOf<Camera>();

            Debug.Assert(camera != null, nameof(camera) + " != null");

            _pmxRenderer.InitializeContents(pmxModel, camera, _modelTextures);

            VmdMotion vmdMotion;

            using (var fileStream = File.Open(Path.Combine(modelBaseDir, "LD.vmd"), FileMode.Open, FileAccess.Read, FileShare.Read)) {
                vmdMotion = VmdReader.ReadMotion(fileStream);
            }

            var vmdMotionScaleFactor = TryDetectVmdScaleFactor();

            vmdMotion.Scale(vmdMotionScaleFactor);

            _pmxVmdAnimator.InitializeContents(pmxModel, vmdMotion);

            _vmdMotion = vmdMotion;

            _boneDebugVisualizer.InitializeContents(pmxModel, camera);

            void TryLoadTexture(string relativeFilePath) {
                if (_modelTextures.ContainsKey(relativeFilePath)) {
                    return;
                }

                var texture = ContentHelper.LoadTexture(GraphicsDevice, Path.Combine(modelBaseDir, relativeFilePath));

                if (texture != null) {
                    _modelTextures[relativeFilePath] = texture;
                }
            }

            float TryDetectVmdScaleFactor() {
                const float defaultScaleFactor = 1.0f;

                PmxBone pmxBone1, pmxBone2;

                try {
                    pmxBone1 = pmxModel.Bones.SingleOrDefault(b => string.Equals(b.Name, "全ての親", StringComparison.Ordinal));
                    pmxBone2 = pmxModel.Bones.SingleOrDefault(b => string.Equals(b.Name, "センター", StringComparison.Ordinal));
                } catch (InvalidOperationException ex) {
                    // Multiple bones in PMX model having the same name.
                    Debug.Print(ex.ToString());

                    return defaultScaleFactor;
                }

                var vmdBone1 = vmdMotion.BoneFrames.FirstOrDefault(b => string.Equals(b.Name, "全ての親", StringComparison.Ordinal));
                var vmdBone2 = vmdMotion.BoneFrames.FirstOrDefault(b => string.Equals(b.Name, "センター", StringComparison.Ordinal));

                if (pmxBone1 == null || pmxBone2 == null || vmdBone1 == null || vmdBone2 == null) {
                    Debug.Print("At least one of the standard bones is not found. Returning default scale.");

                    return defaultScaleFactor;
                }

                if (vmdBone1.FrameIndex != 0 || vmdBone2.FrameIndex != 0) {
                    Debug.Print("The two VMD standard bones should have appeared at the first frame (index=0). Returning default scale.");

                    return defaultScaleFactor;
                }

                if (pmxBone2.ReferenceParent != pmxBone1) {
                    Debug.Print("PMX bone #2's parent should be PMX bone #1.");

                    return defaultScaleFactor;
                }

                var pmxRelativePosition = pmxBone2.RelativePosition;
                var vmdRelativePosition = vmdBone2.Position - vmdBone1.Position;

                var pmxLength = pmxRelativePosition.Length();
                var vmdLength = vmdRelativePosition.Length();

                return pmxLength / vmdLength;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void Ksh_KeyHold(object sender, KeyEventArgs e) {
            var cam = this.SimpleFindComponentOf<Camera>();

            if (cam == null) {
                return;
            }

            switch (e.KeyCode) {
                case Keys.A:
                    cam.Strafe(-0.1f);
                    break;
                case Keys.D:
                    cam.Strafe(0.1f);
                    break;
                case Keys.W:
                    cam.Walk(0.1f);
                    break;
                case Keys.S:
                    cam.Walk(-0.1f);
                    break;
                case Keys.Left:
                    cam.Yaw(0.01f);
                    break;
                case Keys.Right:
                    cam.Yaw(-0.01f);
                    break;
                case Keys.Up:
                    cam.Pitch(-0.01f);
                    break;
                case Keys.Down:
                    cam.Pitch(0.01f);
                    break;
                case Keys.R:
                    cam?.Reset();
                    break;
            }
        }

        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private string _modelBaseDir;

        private PmxModel _pmxModel;
        private PmxRenderer _pmxRenderer;
        private VmdMotion _vmdMotion;
        private PmxVmdAnimator _pmxVmdAnimator;
        private BoneDebugVisualizer _boneDebugVisualizer;

        private readonly Dictionary<string, Texture2D> _modelTextures = new Dictionary<string, Texture2D>();

    }
}
