using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace AnotherWheel.Models.Extensions {
    internal static class ArrayExtensions {

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        internal static bool ElementEquals<T>([NotNull, ItemCanBeNull] this T[] array, [NotNull, ItemCanBeNull] T[] other) {
            if (ReferenceEquals(array, other)) {
                return true;
            }

            if (array == null && other == null) {
                return true;
            }

            if (array == null && other != null) {
                return false;
            }

            if (array != null && other == null) {
                return false;
            }

            if (array.Length != other.Length) {
                return false;
            }

            var len = array.Length;
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < len; ++i) {
                if (!comparer.Equals(array[i], other[i])) {
                    return false;
                }
            }

            return true;
        }

    }
}
