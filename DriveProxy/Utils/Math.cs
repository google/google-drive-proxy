/*
Copyright 2014 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;

namespace DriveProxy.Utils
{
  internal class Math
  {
    public static decimal Round(decimal value)
    {
      try
      {
        return System.Math.Round(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static decimal Round(decimal value, int decimalPlaces)
    {
      try
      {
        return System.Math.Round(value, decimalPlaces);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double Absolute(double value)
    {
      try
      {
        return System.Math.Abs(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static decimal Absolute(decimal value)
    {
      try
      {
        return System.Math.Abs(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static int Absolute(int value)
    {
      try
      {
        return System.Math.Abs(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static long Absolute(long value)
    {
      try
      {
        return System.Math.Abs(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static short Absolute(short value)
    {
      try
      {
        return System.Math.Abs(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static float Absolute(float value)
    {
      try
      {
        return System.Math.Abs(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double Ceiling(double value)
    {
      try
      {
        return System.Math.Ceiling(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static int Divide(int value, int divideBy)
    {
      try
      {
        float tempValue = Convert.ToSingle(value);
        float tempDivideBy = Convert.ToSingle(divideBy);
        float tempResult = tempValue / tempDivideBy;
        int result = Convert.ToInt32(tempResult);

        return result;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static int Divide(int value, int divideBy, out int remainder)
    {
      remainder = 0;

      try
      {
        return System.Math.DivRem(value, divideBy, out remainder);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static long Divide(long value, long divideBy)
    {
      try
      {
        long remainder = 0;

        return Divide(value, divideBy, out remainder);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static long Divide(long value, long divideBy, out long remainder)
    {
      remainder = 0;

      try
      {
        return System.Math.DivRem(value, divideBy, out remainder);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static decimal Divide(decimal value, decimal divideBy)
    {
      try
      {
        if (value <= 0 || divideBy <= 0)
        {
          return 0;
        }

        decimal result = value / divideBy;

        return result;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static decimal Divide(decimal value, decimal divideBy, int decimalPlaces)
    {
      try
      {
        decimal result = Divide(value, divideBy);

        return Round(result, decimalPlaces);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double IsPercentOfWhat(double part, double whole)
    {
      try
      {
        double percentage = (whole / part);

        return percentage;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double IsWhatPercentOf(double part, double whole)
    {
      try
      {
        return IsWhatPercentOf(part, whole, -1);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double IsWhatPercentOf(double part, double whole, int roundToDigits)
    {
      try
      {
        double percentage = 0;

        if (part != 0 && whole != 0)
        {
          percentage = (part / whole) * 100;
        }

        if (roundToDigits > -1)
        {
          percentage = System.Math.Round(percentage, roundToDigits);
        }

        return percentage;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static int Max(int value1, int value2)
    {
      try
      {
        return System.Math.Max(value1, value2);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static int Min(int value1, int value2)
    {
      try
      {
        return System.Math.Min(value1, value2);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static int ProgressPercent(double part, double whole)
    {
      try
      {
        double result = IsWhatPercentOf(part, whole);

        return Convert.ToInt32(result);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double WhatIsPercent(double part, double whole)
    {
      try
      {
        return WhatIsPercent(part, whole, -1);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static double WhatIsPercent(double part, double whole, int roundToDigits)
    {
      try
      {
        double percentage = (whole * part) / 100;

        if (roundToDigits > -1)
        {
          percentage = System.Math.Round(percentage, roundToDigits);
        }

        return percentage;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }
  }
}
