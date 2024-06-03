using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BUDSharedWebApiCore.DTO
{
    public class GridConfigParams
    {
        public GridConfig Config { get; set; }
        public string ConfigName { get; set; }
        public bool PrivateConfig { get; set; }
    }
}
