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

        /// <summary>
        /// 向后台提交检测结果
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="sliceId"></param>
        /// <param name="cameraId"></param>
        /// <param name="imageId"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="result"></param>
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
        /// <summary>
        /// 4.2.2检测相机参数获取
        /// </summary>
        /// <param name="cameraId"></param>
        /// <returns></returns>
        public List<CameraParamsEntity> GetCameraParams()
        {
            string url = BaseUrl + "/setup/camera_params";
            Dictionary<string, object> body = new Dictionary<string, object>();
            var res = GetData(url, JsonConvert.SerializeObject(body));
            return JsonConvert.DeserializeObject<List<CameraParamsEntity>>(res.ToString());
        }
        /// <summary>
        /// 4.2.3获取检测相机参数(新)
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="recipe"></param>
        /// <param name="mmg"></param>
        /// <returns></returns>
        public CameraRecipeParamsEntity GetCameraRecipeParams(int cameraId, int recipe)
        {
            string url = BaseUrl + "/setup/get_insp_cam_params";
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("cameraId", cameraId);
            body.Add("recipeId", recipe);
            var res = GetData(url, JsonConvert.SerializeObject(body));
            return JsonConvert.DeserializeObject<CameraRecipeParamsEntity>(res.ToString());
        }
        /// <summary>
        /// 4.3.6传统算法检测参数获取
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="mmg"></param>
        /// <returns></returns>
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
        /// 3.2.2高级设备参数获取
        /// </summary>
        /// <returns></returns>
        public DeviceParamExtEntity GetDeviceParamExt()
        {
            string url = BaseUrl + "/setup/dev_params_ext";
            var res = GetData(url, "");
            return JsonConvert.DeserializeObject<DeviceParamExtEntity>(res.ToString());
        }
        /// <summary>
        /// 3.2.1设备参数获取
        /// </summary>
        /// <returns></returns>
        public DeviceParamEntity GetDeviceParam()
        {
            string url = BaseUrl + "/setup/dev_params";
            var res = GetData(url, "");
            return JsonConvert.DeserializeObject<DeviceParamEntity>(res.ToString());
        }
        /// <summary>
        /// 4.3.9图像预处理参数获取
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="mmg"></param>
        /// <returns></returns>
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
