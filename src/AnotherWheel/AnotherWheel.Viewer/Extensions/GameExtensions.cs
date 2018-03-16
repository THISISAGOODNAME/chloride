using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Viewer.Extensions {
    public static class GameExtensions {

        [CanBeNull]
        public static IGameComponent SimpleFindComponentOf([NotNull] this Game game, [NotNull] Type componentType) {
            foreach (var component in game.Components) {
                if (component.GetType() == componentType) {
                    return component;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public static T SimpleFindComponentOf<T>([NotNull] this Game game)
            where T : class, IGameComponent {
            return SimpleFindComponentOf(game, typeof(T)) as T;
        }

    }
}
