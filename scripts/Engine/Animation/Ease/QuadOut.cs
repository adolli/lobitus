
namespace adolli.Engine
{
    public class QuadOut : Ease
    {

        public float f(float x)
        {
            return System.Math.Sign(x) * System.Math.Abs(x * x * x * x);
        }
    }
}

