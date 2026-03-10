namespace Settings
{
    public interface ILocalSettings
    {
        IViewNames ViewNames { get; }
        IResourceNames ResourceNames { get; }
        IGameSettings GameSettings { get; }
    }
}