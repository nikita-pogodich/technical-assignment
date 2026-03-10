using System.Collections.Generic;

namespace Core.SaveSystem
{
    public interface ISaveSystem
    {
        void Save<T>(string slot, T data);
        T Load<T>(string slot);
        bool TryLoad<T>(string slot, out T data);

        bool SlotExists(string slot);
        bool HasAnySaveFiles();

        IReadOnlyList<string> GetAllSlots();
        void DeleteSlot(string slot);
    }
}