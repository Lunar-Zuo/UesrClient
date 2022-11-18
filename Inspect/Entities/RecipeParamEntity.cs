using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class RecipeParamEntity
    {
        //需要跟界面设置参数名统一
        [JsonProperty("cameras")]
        public List<ConfigCameraEntity> Cameras { get; set; }
    }
}
