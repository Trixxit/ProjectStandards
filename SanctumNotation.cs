using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlsStandards
{
    /// <summary>
    /// Stores multi-os objects
    /// </summary>
    public static partial class Manta
    {
        
    }

    /// <summary>
    /// Static class holding extension and helper methods
    /// </summary>
    public static partial class Magistrate
    {
        /// <summary>
        /// Returns <paramref name="e"/> with all non-alphanumeral characters removed.
        /// </summary>
        /// <param name="e">The string to enact on</param>
        /// <returns>Processed string</returns>
        public static partial string MakeAlphanumeral(this string e);

        /// <summary>
        /// Throws <paramref name="e"/> if <paramref name="condition"/> is false.
        /// </summary>
        /// <param name="e">The exception to throw</param>
        /// <param name="condition">The condition to check</param>
        /// <returns>false if condition is true</returns>
        public static partial bool ThrowIfNot(this Exception e, bool condition);

        /// <summary>
        /// Does <paramref name="primary"/> if <paramref name="condition"/> is true, else does <paramref name="auxiliary"/>
        /// </summary>
        /// <param name="primary">Main function</param>
        /// <param name="condition">The condition to check</param>
        /// <param name="auxiliary">Auxillary function</param>
        public static partial void DoThisIfThatElseDoThis(this Action primary, bool condition, Action auxiliary);

        /// <summary>
        /// Does <paramref name="primary"/> if <paramref name="condition"/> is true, else does <paramref name="auxiliary"/>
        /// </summary>
        /// <typeparam name="T">Generic output</typeparam>
        /// <param name="primary">Main function</param>
        /// <param name="condition">The condition to check</param>
        /// <param name="auxiliary">Auxillary function</param>
        /// <returns>The output of type <typeparamref name="T"/> from either <paramref name="primary"/> or <paramref name="auxiliary"/></returns>
        public static partial T? DoThisIfThatElseDoThis<T>(this Func<T?> primary, bool condition, Func<T?> auxiliary);

        /// <summary>
        /// Does <paramref name="primary"/> if <paramref name="condition"/> is true, else does <paramref name="auxiliary"/>
        /// </summary>
        /// <typeparam name="T">Generic Output type</typeparam>
        /// <param name="primary">Main function</param>
        /// <param name="condition">The condition to check</param>
        /// <param name="auxiliary">Auxillary function</param>
        /// <param name="parameter">An object containing the input param</param>
        /// <returns></returns>
        public static partial T? DoThisIfThatElseDoThis<T>(this Func<object?, T?> primary, bool condition, Func<object?, T?> auxiliary, object? parameter);

        /// <summary>
        /// Does <paramref name="primary"/> if <paramref name="condition"/> is true, else does <paramref name="auxiliary"/>
        /// </summary>
        /// <typeparam name="T">Generic input type</typeparam>
        /// <param name="primary">Main function</param>
        /// <param name="condition">The condition to check</param>
        /// <param name="auxiliary">Auxillary function</param>
        /// <param name="parameter">An object containing the input param of type <typeparamref name="T"/></param>
        public static partial void DoThisIfThatElseDoThis<T>(this Action<T> primary, bool condition, Action<T> auxiliary, T parameter);

        /// <summary>
        /// Used for consoles, asks the user the message and then returns true if the message contains 'y', else false.
        /// </summary>
        /// <param name="message">A yes or no question</param>
        /// <returns><code>true</code> if input contains "y", else <code>false</code></returns>
        public static partial bool AskConfirm(string message);

        /// <summary>
        /// Generic Bitwise conversion for converting valuetypes into eachother through address, not through casting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        /// <remarks>This is not the same as casting (i.e: <code>float k = 20.39; int f = (int)k</code>).
        /// This will interpret the bits of the value as another type, useful for binary reads.
        /// For example:
        /// <code>
        /// long l = 927361232142;
        /// // l's bits = 0b0000000000000000000000001101011111101011000010010110110100001110
        /// double d = l.BWConvert();
        /// // d's bits = 0b0000000000000000000000001101011111101011000010010110110100001110
        /// // d's value = something else
        /// </code>
        /// </remarks>
        public static unsafe partial J BWConvert<T, J>(this T l)
            where T : unmanaged
            where J : unmanaged;

        /// <summary>
        /// Checks if an enumerable object is empty or null
        /// </summary>
        /// <typeparam name="T">The item type inside the enumerable</typeparam>
        /// <param name="collection">the collection to check</param>
        /// <returns>true if null or empty.</returns>
        public static partial bool IsNullOrEmpty<T>(this IEnumerable<T> collection);

        /// <summary>
        /// Adds one enumerable to a collection
        /// </summary>
        /// <typeparam name="T">Generic type of items in both sets</typeparam>
        /// <param name="collection">The collection being expanded</param>
        /// <param name="items">range of items to add</param>
        public static partial void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items);

        /// <summary>
        /// Clamps <paramref name="value"/> between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <typeparam name="T">The type of all values, must be comparable.</typeparam>
        /// <param name="value">The initial value</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum... what did you expect??</param>
        /// <returns></returns>
        public static partial T Clamp<T>(this T value, T min, T max)
            where T : IComparable<T>;

        /// <summary>
        /// Throws <paramref name="invalidCast"/> if <paramref name="type"/>'s type is not in <paramref name="types"/>
        /// </summary>
        /// <param name="invalidCast">The exception to throw</param>
        /// <param name="type">The object to check the type of</param>
        /// <param name="types">Collection of valid type objects</param>
        /// <returns></returns>
        public static partial bool ThrowIfTypeMismatch(this InvalidCastException invalidCast, object type, params Type[] types);

        /// <summary>
        /// Ensures that all objects within <paramref name="items"/> are of any one type within <paramref name="types"/>, else errors
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="items">The collection of items to check</param>
        /// <param name="types">Collection of valid types.</param>
        /// <exception cref="InvalidCastException"> Thrown if one of <paramref name="items"/> type's not valid.</exception>
        public static partial void EnsureTypes<T>(this IEnumerable<T> items, params Type[] types);

        /// <summary>
        /// Adds all items of <paramref name="additions"/> to the end of <paramref name="original"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="additions"></param>
        /// <returns>The expanded <paramref name="original"/></returns>
        public static partial T[] Combine<T>(T[] original, params T[] additions);

        /// <summary>
        /// Determines if <paramref name="point"/> is within <paramref name="radius"/> given <paramref name="center"/> as the center
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="center">The center point to base <paramref name="radius"/> around.</param>
        /// <param name="radius">The radius around <paramref name="center"/> to check for <paramref name="point"/> to be within</param>
        /// <param name="point">The point to check for</param>
        /// <returns><code>true</code> if <paramref name="point"/> lies within the circle of 2 point radius <paramref name="radius"/></returns>
        public static partial bool CloseTo<T, I, J>(T center, I radius, J point);

        /// <summary>
        /// Determines if <paramref name="point"/> is within <paramref name="radius"/> given <paramref name="center"/> as the center
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="center">The center point to base <paramref name="radius"/> around.</param>
        /// <param name="radius">The radius around <paramref name="center"/> to check for <paramref name="point"/> to be within</param>
        /// <param name="point">The point to check for</param>
        /// <returns><code>true</code> if <paramref name="point"/> lies within the circle of 2 point radius <paramref name="radius"/></returns>
        public static partial bool CloseTo<T, I, J>(Mechanic.Point<T> center, I radius, Mechanic.Point<J> point);

        /// <summary>
        /// Determines if <paramref name="point"/> is within <paramref name="radius"/> given <paramref name="center"/> as the center
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="center">The center point to base <paramref name="radius"/> around.</param>
        /// <param name="radius">The radius around <paramref name="center"/> to check for <paramref name="point"/> to be within</param>
        /// <param name="point">The point to check for</param>
        /// <returns><code>true</code> if <paramref name="point"/> lies within the circle of 2 point radius <paramref name="radius"/></returns>
        /// <remarks><paramref name="radius"/> is a point, so when checking in the y axis for <paramref name="point"/>, the Y value of point is used, else the X value.</remarks>
        public static partial bool CloseTo<T, I, J>(Mechanic.Point<T> center, Mechanic.Point<I> radius, Mechanic.Point<J> point);

    }

    /// <summary>
    /// Contains structs
    /// </summary>
    public static partial class Mechanic
    {
        /// <summary>
        /// Generic Point type for holding two values and operating on them. Has implicit System.Drawing.Point and Size conversion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public readonly partial struct Point<T>;
    }

    /// <summary>
    /// Contains math methods
    /// </summary>
    public static partial class Melon
    {
        /// <summary>
        /// Adds two numeric types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static partial T? Add<T>(T? a, T? b);

        /// <summary>
        /// Subtracts two numeric types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static partial T? Subtract<T>(T? a, T? b);
        
        /// <summary>
        /// Raises <paramref name="a"/> to <paramref name="b"/>, presuming both are numeric types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static partial T? Power<T>(T? a, T? b);

        /// <summary>
        /// Divides two numeric types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static partial T? Divide<T>(T? a, T? b);
    }
}
