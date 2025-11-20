using System;
using System.Globalization;

namespace ByteGuard.FileValidator
{
    /// <summary>
    /// Simple helper class to convert KB, MB and GB to byte size.
    /// </summary>
    public static class ByteSize
    {
        private const long KB = 1024;
        private const long MB = KB * 1024;
        private const long GB = MB * 1024;

        /// <summary>
        /// Convert the given value to byte size.
        /// </summary>
        /// <param name="value">Kilobyte value to convert to byte size (e.g. <c>10</c> = 10 KB).</param>
        public static long KiloBytes(long value)
        {
            return value * KB;
        }

        /// <summary>
        /// Convert the given value to byte size.
        /// </summary>
        /// <param name="value">Kilobyte value to convert to byte size (e.g. <c>10.5</c> = 10.5 KB).</param>
        public static long KiloBytes(double value)
        {
            return (long)Math.Ceiling(value * KB);
        }

        /// <summary>
        /// Convert the given value to byte size.
        /// </summary>
        /// <param name="value">Megabyte value to convert to byte size (e.g. <c>10</c> = 10 MB).</param>
        public static long MegaBytes(long value)
        {
            return value * MB;
        }

        /// <summary>
        /// Convert the given value to byte size.
        /// </summary>
        /// <param name="value">Megabyte value to convert to byte size (e.g. <c>10.5</c> = 10.5 MB).</param>
        public static long MegaBytes(double value)
        {
            return (long)Math.Ceiling(value * MB);
        }

        /// <summary>
        /// Convert the given value to byte size.
        /// </summary>
        /// <param name="value">Gigabyte value to convert to byte size (e.g. <c>10</c> = 10 GB).</param>
        public static long GigaBytes(long value)
        {
            return value * GB;
        }

        /// <summary>
        /// Convert the given value to byte size.
        /// </summary>
        /// <param name="value">Gigabyte value to convert to byte size (e.g. <c>10.5</c> = 10.5 GB).</param>
        public static long GigaBytes(double value)
        {
            return (long)Math.Ceiling(value * GB);
        }

        /// <summary>
        /// Convert a string representation of a decimal byte to long.
        /// </summary>
        /// <param name="value">A string representation of byte size (e.g. <c>"20MB"</c>, <c>"14,5 KB"</c>, <c>or "10 GB"</c>).</param>
        public static long Parse(string value)
        {
            return Parse(value, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Convert a string representation of a decimal byte to long.
        /// </summary>
        /// <param name="value">A string representation of byte size (e.g. <c>"20MB"</c>, <c>"14,5 KB"</c>, <c>or "10 GB"</c>).</param>
        /// <param name="formatProvider">Culture-specific formatting options.</param>
        public static long Parse(string value, IFormatProvider formatProvider)
        {
            return Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, formatProvider);
        }

        /// <summary>
        /// Convert a string representation of a decimal byte to long.
        /// </summary>
        /// <param name="value">A string representation of byte size (e.g. <c>"20MB"</c>, <c>"14,5 KB"</c>, <c>or "10 GB"</c>).</param>
        /// <param name="numberStyles">Number style of the string being parsed.</param>
        /// <param name="formatProvider">Culture-specific formatting options.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided value is null or whitespace.</exception>
        /// <exception cref="FormatException">Thrown if the provided value does not adhere to the expected format.</exception>
        public static long Parse(string value, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "String is null or whitespace.");

            value = value.TrimStart();

            var numberFormatInfo = NumberFormatInfo.GetInstance(formatProvider);
            var decimalSeparator = Convert.ToChar(numberFormatInfo.NumberDecimalSeparator);
            var groupSeparator = Convert.ToChar(numberFormatInfo.NumberGroupSeparator);

            // Find first non-digit character which also isn't a separation character.
            var index = 0;
            var found = false;

            for (index = 0; index < value.Length; index++)
            {
                if (!(char.IsDigit(value[index]) || value[index] == decimalSeparator || value[index] == groupSeparator))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new FormatException($"No size indicator found in value '{value}'");

            int lastNumberIndex = index;

            // Identify the full number part (e.g. "23,14" in "23,14 MB").
            var numberPart = value.Substring(0, lastNumberIndex).Trim();

            if (!double.TryParse(numberPart, numberStyles, formatProvider, out var numberValue))
                throw new FormatException($"No number found in value '{value}'");

            // Identify the size indicator (e.g. "MB" in "23,14 MB").
            var sizeIndicator = value.Substring(lastNumberIndex, value.Length - lastNumberIndex).Trim();

            // Handle the remaining size indicators.
            switch (sizeIndicator.ToLowerInvariant())
            {
                case "b": return (long)Math.Ceiling(numberValue);
                case "kb": return KiloBytes(numberValue);
                case "mb": return MegaBytes(numberValue);
                case "gb": return GigaBytes(numberValue);
                default: throw new FormatException($"Unknown or unsupported size indicator '{sizeIndicator}'.");
            }
        }
    }
}
