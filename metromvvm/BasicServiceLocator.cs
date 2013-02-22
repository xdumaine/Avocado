namespace MetroMVVM
{
    using MetroMVVM.Interfaces;

    public class BasicServiceLocator
    {
        public static IBasicServiceLocator Instance { get; private set; }
        public static void SetServiceLocatorProvider(IBasicServiceLocator serviceLocator)
        {
            Instance = serviceLocator;
        }
    }
}