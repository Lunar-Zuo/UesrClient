using CwCommon.Entities;
using CwCommon.Utils;
using Inspect.Commons;
using Inspect.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Adapter
{
    public class CommitAdapter : BaseCommitAdapter
    {
        public static CommitAdapter Instance = new CommitAdapter();

        public async void CommitInspectResult(string sn, string panelId, int sliceId, int cameraId, int imageId, string fileName, object data, int result)
        {
            string url = BaseUrl + "/inspect/defect_data";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("serialNumber", sn);
            body.Add("panelCode", panelId);
            body.Add("sliceId", sliceId);
            body.Add("cameraId", cameraId);
            body.Add("imageNumber", imageId);
            body.Add("filename", fileName);
            body.Add("data", data);
            body.Add("result", result);
            await HttpHelper.PostJsonAsync(url, JsonConvert.SerializeObject(body));
        }

        public CameraParamsEntity GetCameraParams(int cameraId)
        {
            string url = BaseUrl + "/setup/module_params";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("cameraId", cameraId);
            body.Add("moduleId", 2);
            var res = GetData(url, JsonConvert.SerializeObject(body));
            return JsonConvert.DeserializeObject<CameraParamsEntity>(res.ToString());
        }

        public CameraRecipeParamsEntity GetCameraRecipeParams(int cameraId, int recipe, int mmg = 0)
        {
            string url = BaseUrl + "/setup/camera_params";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("cameraId", cameraId);
            body.Add("recipePlc", recipe);
            body.Add("mmg", mmg);
            var res = GetData(url, JsonConvert.SerializeObject(body));
            return JsonConvert.DeserializeObject<CameraRecipeParamsEntity>(res.ToString());
        }

        public object GetTradAlgoRecipeParams(int recipe, int mmg = 0)
        {
            string url = BaseUrl + "/setup/trad_algo_insp_params";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("recipePlc", recipe);
            body.Add("mmg", mmg);
            var res = GetData(url, JsonConvert.SerializeObject(body));
            return res;
        }
        /// <summary>
        /// 获取Recipe基础参数
        /// </summary>
        /// <returns></returns>
        public RecipeParamEntity GetRecipeParam(int recipe, int mmg = 0)
        {
            string url = BaseUrl + "/setup/recipe_base";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("recipePlc", recipe);
            body.Add("mmg", mmg);
            var res = GetData(url, JsonConvert.SerializeObject(body));

            return JsonConvert.DeserializeObject<RecipeParamEntity>(res.ToString());
        }

        public DeviceParamEntity GetDeviceParam()
        {
            string url = BaseUrl + "/setup/dev_params";
            var res = GetData(url, "");
            return JsonConvert.DeserializeObject<DeviceParamEntity>(res.ToString());
        }

        public ImagePreProcessParamsEntity GetImagePreProcessParams(int recipe, int mmg = 0)
        {
            string url = BaseUrl + "/setup/image_pre_process_params";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("recipePlc", recipe);
            body.Add("mmg", mmg);
            var res = GetData(url, JsonConvert.SerializeObject(body));
            return JsonConvert.DeserializeObject<ImagePreProcessParamsEntity>(res.ToString());
        }

        private object GetData(string url, string body)
        {
            var res = HttpHelper.PostJsonAsync(url, body).Result;
            ResponseEntity entity = JsonConvert.DeserializeObject<ResponseEntity>(res);
            if (entity.Code != 0) throw new Exception(entity.Message);
            return entity.Data;
        }
    }
}
