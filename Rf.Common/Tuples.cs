using System.Diagnostics;
using System;

namespace Rf.Common
{
  /// <summary>
  /// Convenience class to create tuples, since compiler inference on method args
  /// works as opposed to creating a new instance where you need to specify
  /// the types
  /// </summary>
  public static class Tuple
  {
    /// <summary>
    /// Create a value-pair based on the passed in arguments. You will usually not need to specify
    /// the type arguments.
    /// </summary>
    public static Tuple<TFirst, TSecond> From<TFirst, TSecond>(TFirst firstValue, TSecond secondValue)
    {
      return new Tuple<TFirst, TSecond>(firstValue, secondValue);
    }

    /// <summary>
    /// Create a value trio based on the passed in arguments. You will usually not need to specify
    /// the type arguments.
    /// </summary>
    public static Tuple<TFirst, TSecond, TThird> From<TFirst, TSecond, TThird>(TFirst firstValue, TSecond secondValue, TThird thirdValue)
    {
      return new Tuple<TFirst, TSecond, TThird>(firstValue, secondValue, thirdValue);
    }
  }

  /// <summary>
  /// A pair of two associated values
  /// </summary>
  /// <typeparam name="T1">Type of First value</typeparam>
  /// <typeparam name="T2">Type of second value</typeparam>
  [Serializable]
  [DebuggerDisplay("First: {First}, Second: {Second}")]
  public class Tuple<T1, T2>
  {
    /// <summary>
    /// The first value
    /// </summary>
    public readonly T1 First;

    /// <summary>
    /// The second value
    /// </summary>
    public readonly T2 Second;

    /// <summary>
    /// Construct a tuple with the provided values
    /// </summary>
    /// <param name="first">the first value</param>
    /// <param name="second">the second value</param>
    public Tuple(T1 first, T2 second)
    {
      First = first;
      Second = second;
    }
  }

  /// <summary>
  /// Three associated values
  /// </summary>
  /// <typeparam name="T1">Type of First value</typeparam>
  /// <typeparam name="T2">Type of second value</typeparam>
  /// <typeparam name="T3">Type of third value</typeparam>
  [DebuggerDisplay("First: {First}, Second: {Second}, Third: {Third}")]
  public class Tuple<T1, T2, T3> : Tuple<T1, T2>
  {
    /// <summary>
    /// The third value
    /// </summary>
    public readonly T3 Third;

    /// <summary>
    /// Construct a tuple with the provided values
    /// </summary>
    /// <param name="first">the first value</param>
    /// <param name="second">the second value</param>
    /// <param name="third">teh third value</param>
    public Tuple(T1 first, T2 second, T3 third) : base(first, second)
    {
      Third = third;
    }
  }
}