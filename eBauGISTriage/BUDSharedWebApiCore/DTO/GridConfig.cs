using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BUDSharedWebApiCore.DTO
{
    public class GridConfig
    {
        public Guid? Id { get; set; }
        public string DisplayText { get; set; }
        public string UserAlias { get; set; }
        public string GridState { get; set; }
        public bool Protected { get; set; }
        public bool Standard { get; set; }

        public void Deserialize(string data)
        {
            GridConfig config = JsonConvert.DeserializeObject<GridConfig>(data);
            if (config != null)
            {
                Id = config.Id;
                DisplayText = config.DisplayText;
                GridState = config.GridState;
                UserAlias = config.UserAlias;
                Protected = config.Protected;
            }
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
