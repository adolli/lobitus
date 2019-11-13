
namespace adolli.Engine
{
    public class FunctorEase : Ease
    {
        public delegate float EaseFunctorType(float x);
        private EaseFunctorType fn_;

        public FunctorEase(EaseFunctorType fn)
        {
            fn_ = fn;
        }

        public float f(float x)
        {
            return fn_(x);
        }
    }
}

