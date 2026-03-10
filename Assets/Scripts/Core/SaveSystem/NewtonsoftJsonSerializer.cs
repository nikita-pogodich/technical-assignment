using Newtonsoft.Json;

namespace Core.SaveSystem
{
    public sealed class NewtonsoftJsonSerializer : ISaveSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.Indented
            };
        }

        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data, _settings);
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, _settings);
        }
    }
}