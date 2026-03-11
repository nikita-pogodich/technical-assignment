using R3;

namespace Core.OrientationDetector
{
    public interface IOrientationDetector
    {
        ReadOnlyReactiveProperty<Orientation> Orientation { get; }
    }
}