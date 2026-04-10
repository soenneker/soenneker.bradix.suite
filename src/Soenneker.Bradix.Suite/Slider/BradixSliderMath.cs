using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Soenneker.Bradix.Suite.Slider;

internal static class BradixSliderMath
{
    public static IReadOnlyList<double> NormalizeValues(IEnumerable<double>? values, double min, double max)
    {
        List<double> normalized = values?.Select(value => Clamp(value, min, max)).OrderBy(value => value).ToList() ?? [];
        return normalized.Count == 0 ? [min] : normalized;
    }

    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }

    public static double ConvertValueToPercentage(double value, double min, double max)
    {
        double range = max - min;

        if (range <= 0)
            return 0;

        double percentage = ((value - min) / range) * 100;
        return Clamp(percentage, 0, 100);
    }

    public static int GetClosestValueIndex(IReadOnlyList<double> values, double nextValue)
    {
        if (values.Count <= 1)
            return 0;

        double closestDistance = double.MaxValue;
        int closestIndex = 0;

        for (var i = 0; i < values.Count; i++)
        {
            double distance = Math.Abs(values[i] - nextValue);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public static List<double> GetNextSortedValues(IReadOnlyList<double> previousValues, double nextValue, int atIndex)
    {
        List<double> nextValues = previousValues.ToList();

        if (atIndex < 0 || atIndex >= nextValues.Count)
            return nextValues;

        nextValues[atIndex] = nextValue;
        nextValues.Sort();
        return nextValues;
    }

    public static bool HasMinStepsBetweenValues(IReadOnlyList<double> values, double minStepsBetweenValues)
    {
        if (minStepsBetweenValues <= 0 || values.Count <= 1)
            return true;

        for (var i = 0; i < values.Count - 1; i++)
        {
            if ((values[i + 1] - values[i]) < minStepsBetweenValues)
                return false;
        }

        return true;
    }

    public static int GetDecimalCount(double value)
    {
        string[] segments = value.ToString(CultureInfo.InvariantCulture).Split('.');
        return segments.Length > 1 ? segments[1].Length : 0;
    }

    public static double RoundValue(double value, int decimalCount)
    {
        double rounder = Math.Pow(10, decimalCount);
        return Math.Round(value * rounder) / rounder;
    }

    public static string? GetThumbLabel(int index, int totalValues)
    {
        if (totalValues > 2)
            return $"Value {index + 1} of {totalValues}";

        if (totalValues == 2)
            return index == 0 ? "Minimum" : "Maximum";

        return null;
    }
}
