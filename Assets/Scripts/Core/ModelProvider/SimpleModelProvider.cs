namespace Core.ModelProvider
{
    public class SimpleModelProvider : IModelProvider
    {
        private int _currentId = 0;

        public int GetUniqueId()
        {
            _currentId++;
            return _currentId;
        }
    }
}