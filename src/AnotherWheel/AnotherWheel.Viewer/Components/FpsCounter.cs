using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Viewer.Components {
    internal sealed class FpsCounter : GameComponent {

        public FpsCounter(Game game)
            : base(game) {
        }

        public long TotalFrames { get; private set; }

        public float TotalSeconds { get; private set; }

        public float Current { get; private set; }

        public float Average { get; private set; }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            Game.Window.Title = "FPS: " + Average.ToString(CultureInfo.InvariantCulture);
        }

        public void Update(float deltaTime) {
            Current = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(Current);

            if (_sampleBuffer.Count > MaximumSamples) {
                _sampleBuffer.Dequeue();
                Average = _sampleBuffer.Average(i => i);
            } else {
                Average = Current;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }

        private readonly Queue<float> _sampleBuffer = new Queue<float>();

        private const int MaximumSamples = 100;

    }
}
