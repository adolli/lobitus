
namespace adolli.Engine
{
    public class ExpoIn : Ease
    {

        public float f(float x)
        {
            return (float)System.Math.Log((System.Math.E - 1) * x + 1);
        }
    }
}

