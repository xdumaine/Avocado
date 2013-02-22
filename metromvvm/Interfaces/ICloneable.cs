namespace MetroMVVM.Interfaces
{
    public interface ICloneable<T>
    {
        T Clone();

        void Copy(T source);
    }
}