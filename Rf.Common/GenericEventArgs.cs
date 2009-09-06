using System;

namespace BI.CPD3.Common
{
  /// <summary>
  /// Event args that generically allow to transport one safely typed object
  /// </summary>
  /// <typeparam name="T">Any type</typeparam>
  public class GenericEventArgs<T> : EventArgs
  {
    /// <summary>
    /// The value to be transported through this event.
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// ctor that allows to provide the value you want to transport with this event.
    /// </summary>
    /// <param name="value">The instanec to be transported.</param>
    public GenericEventArgs(T value)
    {
      Value = value;
    }
  }
}