
namespace adolli.Engine
{
    /**
	 * @brief ease接口，ease动画对时间t属于[0,1]范围内进行映射，配合Lerp以产生ease效果
	 */
    public interface Ease
    {
        float f(float x);
    }
}

