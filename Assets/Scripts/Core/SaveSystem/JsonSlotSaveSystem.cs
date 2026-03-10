using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.SaveSystem
{
    public sealed class JsonSlotSaveSystem : ISaveSystem
    {
        private readonly string _rootDirectory;
        private readonly ISaveSerializer _serializer;
        private readonly string _fileExtension;

        public JsonSlotSaveSystem(string rootDirectory, ISaveSerializer serializer, string fileExtension = ".save")
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                throw new ArgumentException("Root directory cannot be null or empty.", nameof(rootDirectory));
            }

            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _rootDirectory = rootDirectory;
            _fileExtension = fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension;

            Directory.CreateDirectory(_rootDirectory);
        }

        public void Save<T>(string slot, T data)
        {
            ValidateSlot(slot);

            string path = GetSlotPath(slot);
            string content = _serializer.Serialize(data);

            File.WriteAllText(path, content);
        }

        public T Load<T>(string slot)
        {
            ValidateSlot(slot);

            string path = GetSlotPath(slot);

            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException($"Save slot '{slot}' was not found.", path);
            }

            string content = File.ReadAllText(path);
            return _serializer.Deserialize<T>(content);
        }

        public bool TryLoad<T>(string slot, out T data)
        {
            ValidateSlot(slot);

            string path = GetSlotPath(slot);

            if (File.Exists(path) == false)
            {
                data = default;
                return false;
            }

            string content = File.ReadAllText(path);
            data = _serializer.Deserialize<T>(content);
            return true;
        }

        public bool SlotExists(string slot)
        {
            ValidateSlot(slot);
            return File.Exists(GetSlotPath(slot));
        }

        public bool HasAnySaveFiles()
        {
            if (Directory.Exists(_rootDirectory) == false)
            {
                return false;
            }

            bool hasAnySaveFiles = Directory.EnumerateFiles(_rootDirectory, "*" + _fileExtension).Any();
            return hasAnySaveFiles;
        }

        public IReadOnlyList<string> GetAllSlots()
        {
            if (Directory.Exists(_rootDirectory) == false)
            {
                return Array.Empty<string>();
            }

            return Directory
                .EnumerateFiles(_rootDirectory, "*" + _fileExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }

        public void DeleteSlot(string slot)
        {
            ValidateSlot(slot);

            string path = GetSlotPath(slot);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private string GetSlotPath(string slot)
        {
            string safeSlot = MakeSafeFileName(slot);
            return Path.Combine(_rootDirectory, safeSlot + _fileExtension);
        }

        private static void ValidateSlot(string slot)
        {
            if (string.IsNullOrWhiteSpace(slot))
            {
                throw new ArgumentException("Slot cannot be null or empty.", nameof(slot));
            }
        }

        private static string MakeSafeFileName(string value)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                value = value.Replace(c, '_');
            }

            return value;
        }
    }
}