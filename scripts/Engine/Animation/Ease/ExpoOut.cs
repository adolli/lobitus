
namespace adolli.Engine
{
    public class ExpoOut : Ease
    {
        public float f(float x)
        {
            return (float)((System.Math.Exp(x) - 1) / (System.Math.E - 1));
        }
    }
}

