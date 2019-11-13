
namespace adolli.Engine
{
    public class CubicIn : Ease
    {

        public float f(float x)
        {
            if (x < 0)
            {
                x = 0;
            }
            return (float)System.Math.Pow(x, 1f / 3f);
        }
    }
}

