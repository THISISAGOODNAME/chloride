using System.Collections.Generic;
using AnotherWheel.Models.Pmx;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherWheel.Viewer.Components {
    public sealed class PmxRenderer : DrawableGameComponent {

        public PmxRenderer([NotNull] Game game)
            : base(game) {
        }

        public void InitializeContents([NotNull] PmxModel model, [NotNull] Camera camera, [NotNull] IReadOnlyDictionary<string, Texture2D> textureMap) {
            _model = model;
            _camera = camera;
            _textureMap = textureMap;

            _modelWrapper = ModelWrapper.Convert(model, textureMap, GraphicsDevice);
        }

        protected override void Dispose(bool disposing) {
            _modelWrapper?.Dispose();

            base.Dispose(disposing);
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            var camera = _camera;
            var graphicsDevice = GraphicsDevice;

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _modelWrapper.Model.Draw(Matrix.Identity, camera.ViewMatrix, camera.ProjectionMatrix);
        }

        private PmxModel _model;
        private Camera _camera;
        private IReadOnlyDictionary<string, Texture2D> _textureMap;

        private ModelWrapper _modelWrapper;

    }
}
