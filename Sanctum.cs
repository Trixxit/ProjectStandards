using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GirlsStandards
{
    public static partial class Manta
    {
        /// <summary>
        /// Multi-OS File Dialog method.
        /// </summary>
        /// <returns>A list of selected file(s)</returns>
        public static async Task<IReadOnlyList<IStorageFile>> FileDialog(bool AllowMultiple = false, string Title = "Select file to open", string SuggestedFilename = "", string SuggestedStartPath = "", params string[] AllowedExtensions)
        {
            if (App.InvisMain is null)
                return null;
            var tl = App.InvisMain;
            if (!tl.StorageProvider.CanOpen)
                return null;
            IStorageFolder folder = await tl.StorageProvider.TryGetFolderFromPathAsync(SuggestedStartPath);
            FilePickerFileType fpft = new("Allowed Files") { Patterns = AllowedExtensions };
            return await tl.StorageProvider.OpenFilePickerAsync(new() { Title=Title, AllowMultiple=AllowMultiple, FileTypeFilter = [ fpft ], SuggestedFileName=SuggestedFilename, SuggestedStartLocation=folder});
        }
    }

    public static partial class Magistrate
    {
        public static partial string MakeAlphanumeral(this string e)
            => Alphanumeric().Replace(e, "");

        public static partial bool ThrowIfNot(this Exception e, bool condition)
            => condition ? false : throw e;

        public static partial bool AskConfirm(string message)
        {
            message = message.Trim();
            Console.Write(message + ((message.EndsWith('?') || message.EndsWith(':')) ? "" : ": "));
            return Console.ReadLine()?.ToLower().Contains('y') ?? false;
        }

        public static partial void DoThisIfThatElseDoThis(this Action primary, bool condition, Action auxiliary)
        {
            if (condition) primary();
            else auxiliary();
        }

        public static partial T? DoThisIfThatElseDoThis<T>(this Func<T?> primary, bool condition, Func<T?> auxiliary)
            => condition ? primary() : auxiliary();

        public static partial T? DoThisIfThatElseDoThis<T>(this Func<object?, T?> primary, bool condition, Func<object?, T?> auxiliary, object? parameter)
            => condition ? primary(parameter) : auxiliary(parameter);

        public static partial void DoThisIfThatElseDoThis<T>(this Action<T> primary, bool condition, Action<T> auxiliary, T parameter)
        {
            if (condition) primary(parameter);
            else auxiliary(parameter);
        }

        public static unsafe partial J BWConvert<T, J>(this T l) 
            where T: unmanaged 
            where J: unmanaged
        {
            new InvalidCastException($"Type {typeof(T)}'s size does not match type {typeof(J)}'s size!")
                .ThrowIfNot(sizeof(T) == sizeof(J));
            return *(J*)&l;
        }

        public static partial void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        public static partial T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
            => value.CompareTo(min) < 0 ? min : (value.CompareTo(max) > 0) ? max : value;


        public static partial bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
            => collection == null || !collection.Any();

        public static partial bool CloseTo<T, I, J>(T center, I radius, J point)
        {
            var t = Maid.NumericTypes;
            new InvalidCastException($"center '{center}' was not of a numeric or Point type! (Being of type: {((object)center).GetType()})")
                .ThrowIfTypeMismatch((object)center, t);
            new InvalidCastException($"radius '{radius}' was not of a numeric or Point type! (Being of type: {((object)radius).GetType()})")
            .ThrowIfTypeMismatch((object)center, t);
            new InvalidCastException($"Point '{point}' was not of a numeric or Point type! (Being of type: {((object)point).GetType()})")
            .ThrowIfTypeMismatch((object)center, t);
            double c = Convert.ToDouble(center);
            double r = Convert.ToDouble(radius);
            double p = Convert.ToDouble(point);
            return
                (c + r > p) || (c - r < p);
        }

        public static partial bool CloseTo<T, I, J>(Mechanic.Point<T> center, I radius, Mechanic.Point<J> point)
        {
            new InvalidCastException($"center '{center}' was not of a numeric Point type! (Being of type: {((object)center).GetType()})")
            .ThrowIfTypeMismatch(typeof(T), Maid.NumericTypes);
            new InvalidCastException($"radius '{radius}' was not of a numeric Point type! (Being of type: {((object)radius).GetType()})")
            .ThrowIfTypeMismatch(typeof(I), Maid.NumericTypes);
            new InvalidCastException($"Point '{point}' was not of a numeric Point type! (Being of type: {((object)point).GetType()})")
            .ThrowIfTypeMismatch(typeof(J), Maid.NumericTypes);
            double r = Convert.ToDouble(radius);
            return
                (center.DX + r > point.DX) || (center.DX - r < point.DX)
                && (center.DY + r > point.DY) || (center.DY - r < point.DY);
        }

        public static partial bool CloseTo<T, I, J>(Mechanic.Point<T> center, Mechanic.Point<I> radius, Mechanic.Point<J> point)
        {
            new InvalidCastException($"center '{center}' was not of a numeric Point type! (Being of type: {((object)center).GetType()})")
            .ThrowIfTypeMismatch(typeof(T), Maid.NumericTypes);
            new InvalidCastException($"radius '{radius}' was not of a numeric Point type! (Being of type: {((object)radius).GetType()})")
            .ThrowIfTypeMismatch(typeof(I), Maid.NumericTypes);
            new InvalidCastException($"Point '{point}' was not of a numeric Point type! (Being of type: {((object)point).GetType()})")
            .ThrowIfTypeMismatch(typeof(J), Maid.NumericTypes);
            return
                (center.DX + radius.DX > point.DX) || (center.DX - radius.DX < point.DX)
                && (center.DY + radius.DY > point.DY) || (center.DY - radius.DY < point.DY);
        }

        public static partial bool ThrowIfTypeMismatch(this InvalidCastException invalidCast, object type, params Type[] types)
            => types.Contains(type.GetType()) ? false : throw invalidCast;

        public static partial void EnsureTypes<T>(this IEnumerable<T> items, params Type[] types)
        {
            foreach (var item in items)
                new InvalidCastException($"EITM: {item} of type {item?.GetType()}")
                    .ThrowIfTypeMismatch(item, types);
        }

        public static partial T[] Combine<T>(T[] original, params T[] additions)
        {
            var l = original.ToList();
            foreach (var item in additions)
                l.Add(item);
            return l.ToArray();
        }

        [GeneratedRegex("[^a-zA-Z0-9]")]
        private static partial Regex Alphanumeric();
    }

    /// <summary>
    /// Static class holding custom structs and general use entities
    /// </summary>
    public static class Maid
    {
        /// <summary>
        /// Array of all numeric types
        /// </summary>
        public static Type[] NumericTypes { get; } = [
            typeof(double),
                typeof(int),
                typeof(long),
                typeof(float),
                typeof(short),
                typeof(byte),

                typeof(ulong),
                typeof(ushort),
                typeof(uint),
                typeof(sbyte),
        ];

        /// <summary>
        /// Array of all Alpha types
        /// </summary>
        public static Type[] AlphaTypes { get; } = [
            typeof(string),
            typeof(char)
            ];
    }

    public static partial class Mechanic
    {
        public readonly partial struct Point<T>(object x, object y)
        {
            private readonly T? _x = new InvalidCastException($"{x} was not of a numerical type! (Being of type {x.GetType()})")
                    .ThrowIfTypeMismatch(x, Maid.NumericTypes) ? (T)x : default;
            private readonly T? _y = new InvalidCastException($"{y} was not of a numerical type! (Being of type {y.GetType()})")
                    .ThrowIfTypeMismatch(x, Maid.NumericTypes) ? (T)y : default;
            public T? X => _x;
            public T? Y => _y;

            public double? DX => Convert.ToDouble(X);
            public double? DY => Convert.ToDouble(Y);

            public Type PointType 
                => typeof(T);

            public static Point<T> operator +(Point<T> a, Point<T> b)
                => new(Melon.Add(a.X, b.X), Melon.Add(a.Y, b.Y));

            public static Point<T> operator -(Point<T> a, Point<T> b)
                => new(Melon.Subtract(a.X, b.X), Melon.Subtract(a.Y, b.Y));

            public static Point<T> operator *(Point<T> a, Point<T> b)
                => new(Melon.Multiply(a.X, b.X), Melon.Multiply(a.Y, b.Y));

            public static Point<T> operator ^(Point<T> a, Point<T> b)
                => new(Melon.Power(a.X, b.X), Melon.Power(a.Y, b.Y));

            public static Point<T> operator /(Point<T> a, Point<T> b)
                => new(Melon.Divide(a.X, b.X), Melon.Divide(a.Y, b.Y));

            public static implicit operator System.Drawing.Point(Point<T> p)
            {
                if (p.X is null || p.Y is null)
                    throw new InvalidOperationException("Cannot convert a Point<T> with null coordinates to System.Drawing.Point.");

                return new System.Drawing.Point(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
            }

            public static implicit operator Point<T>(System.Drawing.Point p)
                => new(p.X, p.Y);

            public static implicit operator System.Drawing.Size(Point<T> p)
            {
                if (p.X is null || p.Y is null)
                    throw new InvalidOperationException("Cannot convert a Point<T> with null coordinates to System.Drawing.Point.");

                return new System.Drawing.Size(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
            }

            public static implicit operator Point<T>(System.Drawing.Size p)
                => new(p.Width, p.Height);

            public double DistanceTo(Point<T> other)
            {
                if (DX is null || DY is null || other.DX is null || other.DY is null)
                    throw new InvalidOperationException("Cannot calculate distance with null coordinates.");

                var deltaX = DX.Value - other.DX.Value;
                var deltaY = DY.Value - other.DY.Value;

                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }

            public double AngleTo(Point<T> other)
            {
                if (DX is null || DY is null || other.DX is null || other.DY is null)
                    throw new InvalidOperationException("Cannot calculate angle with null coordinates.");

                var deltaX = other.DX.Value - DX.Value;
                var deltaY = other.DY.Value - DY.Value;

                return Math.Atan2(deltaY, deltaX);
            }

            public override bool Equals(object? obj)
            {
                if (obj is Point<T> other)
                {
                    return EqualityComparer<T?>.Default.Equals(X, other.X) &&
                           EqualityComparer<T?>.Default.Equals(Y, other.Y);
                }
                return false;
            }

            public override int GetHashCode()
                => HashCode.Combine(X, Y);

            public static bool operator ==(Point<T> a, Point<T> b)
                => a.Equals(b);

            public static bool operator !=(Point<T> a, Point<T> b)
                => !(a == b);

            public override string ToString()
                => $"Point<{typeof(T).Name}>({X}, {Y})";

            public string ToJson()
                => System.Text.Json.JsonSerializer.Serialize(this);

            public static Point<T> FromJson(string json)
                => System.Text.Json.JsonSerializer.Deserialize<Point<T>>(json);
        }
    }

    public static partial class Melon
    {
        public static partial T? Add<T>(T? a, T? b)
        {
            (new T[] { a, b }).EnsureTypes(Maid.NumericTypes);
            if (a != null && b != null)
                return (T)Convert.ChangeType(Convert.ToDouble(a) + Convert.ToDouble(b), typeof(T));
            return default;
        }

        public static partial T? Subtract<T>(T? a, T? b)
        {
            (new T[] { a, b }).EnsureTypes(Maid.NumericTypes);
            if (a != null && b != null)
                return (T)Convert.ChangeType(Convert.ToDouble(a) - Convert.ToDouble(b), typeof(T));
            return default;
        }

        public static T? Multiply<T>(T? a, T? b)
        {
            (new T[] { a, b }).EnsureTypes(Maid.NumericTypes);
            if (a != null && b != null)
                return (T)Convert.ChangeType(Convert.ToDouble(a) * Convert.ToDouble(b), typeof(T));
            return default;
        }

        public static partial T? Power<T>(T? a, T? b)
        {
            (new T[] { a, b }).EnsureTypes(Maid.NumericTypes);
            if (a != null && b != null)
                return (T)Convert.ChangeType(Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b)), typeof(T));
            return default;
        }

        public static partial T? Divide<T>(T? a, T? b)
        {
            (new T[] { a, b }).EnsureTypes(Maid.NumericTypes);
            if (a != null && b != null && Convert.ToDouble(b) != 0)
                return (T)Convert.ChangeType(Convert.ToDouble(a) / Convert.ToDouble(b), typeof(T));
            return default;
        }
    }
}
