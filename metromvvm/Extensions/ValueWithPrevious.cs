namespace MetroMVVM.Extensions
{
    /// <summary>
    /// Describes an item in a IEnumerable collection, enabling access to the previous element
    /// </summary>
    public struct ValueWithPrevious<T>
    {
        public readonly T Value, Previous;
        public ValueWithPrevious(T value, T previous)
        {
            Value = value;
            Previous = previous;
        }
    }
}