using Newtonsoft.Json;

namespace QwFramework.Extension
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object data)
        {
            if (data == null)
                return string.Empty;
            return JsonConvert.SerializeObject(data);
        }
    }
}
