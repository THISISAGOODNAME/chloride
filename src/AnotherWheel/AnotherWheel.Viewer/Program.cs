using System;
using JetBrains.Annotations;

namespace AnotherWheel.Viewer {
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main([NotNull, ItemNotNull] string[] args) {
            using (var game = new Game1()) {
                game.Run();
            }
        }

    }
}
