using Newtonsoft.Json;
using System;

namespace BUDSharedWebApiCore.DTO
{
    public class Favorite
    {
        public Guid? Id { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }

        public void Deserialize(string data)
        {
            Favorite config = JsonConvert.DeserializeObject<Favorite>(data);
            if (config != null)
            {
                Id = config.Id;
                Path = config.Path;
                Title = config.Title;
            }
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
