using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.PLC
{
    public static class VisionConstHelper
    {
        public const int MaxProductNumber = 4;
    }

    public class PLCProcessTask
    {
        /// <summary>
        /// 任务线别
        /// </summary>
        public uint LineNumber;
        /// <summary>
        /// 检测号、BatchNumber
        /// </summary>
        public uint BatchNumber;
        /// <summary>
        /// 载具编号
        /// </summary>
        public uint TrayPos;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime;

        /// <summary>
        /// 产品信息
        /// </summary>
        public ProductInformation[] ProductsInfo;

        // 是否从数据库中加载
        public bool IsLoadFromDB { get; set; } = false;

        /// <summary>
        /// 检测模式：人工，AI，AI+人工
        /// </summary>
        public string DeterminationMode = string.Empty;

        /// <summary>
        /// 当前产品 v53、v54
        /// </summary>
        public string CurrentProduct = string.Empty;

        /// <summary>
        /// 产品批次码
        /// </summary>
        public string ProductBatchCode = string.Empty;

        /// <summary>
        /// 检测类型
        /// </summary>
        public InspectionTypeEnum InspectionType;

        /// <summary>
        /// 检测原因
        /// </summary>
        public DetectionReasonEnum DetectionReason;

        /// <summary>
        /// 光源电压
        /// </summary>
        public double Voltage { get; set; }

        /// <summary>
        /// 光源电流
        /// </summary>
        public double Current { get; set; }

        public PLCProcessTask(string determinationMode, string currentProduct, string productBatchCode, InspectionTypeEnum inspectionType, DetectionReasonEnum detectionReason)
        {
            DeterminationMode = determinationMode;
            CurrentProduct = currentProduct;
            InspectionType = inspectionType;
            DetectionReason = detectionReason;
            ProductsInfo = new ProductInformation[VisionConstHelper.MaxProductNumber];
            for (int i = 0; i < ProductsInfo.Length; i++)
            {
                ProductsInfo[i] = new ProductInformation();
            }
        }

        public bool GetHasProduct(int position)
        {
            return ProductsInfo[position - 1].HasProduct;
        }

        public string GetSN(int position)
        {
            return ProductsInfo[position - 1].SN;
        }

        public string GetResultStatus(int position)
        {
            return ProductsInfo[position - 1].ResultStatusString;
        }

        public string GetToken(int position)
        {
            return ProductsInfo[position - 1].CheckToken;
        }

        public bool GetIfScanFail(int position)
        {
            return ProductsInfo[position - 1].ScanFail;
        }

        public bool GetIfCheckTokenFail(int position)
        {
            return string.IsNullOrEmpty(ProductsInfo[position - 1].CheckToken) ? false : ProductsInfo[position - 1].CheckToken.StartsWith("error");
        }

        // 深拷贝方法
        public PLCProcessTask Clone()
        {
            var task = new PLCProcessTask(DeterminationMode, CurrentProduct, ProductBatchCode, InspectionType, DetectionReason)
            {
                LineNumber = this.LineNumber,
                BatchNumber = this.BatchNumber,
                CreateTime = this.CreateTime,
                TrayPos = this.TrayPos,
                Voltage = this.Voltage,
                Current = this.Current,
                ProductsInfo = new ProductInformation[VisionConstHelper.MaxProductNumber],
            };
            for (int i = 0; i < VisionConstHelper.MaxProductNumber; i++)
            {
                task.ProductsInfo[i] = this.ProductsInfo[i].Clone();
            }
            return task;
        }

        // 是否检测完成
        // 位置上没有产品或检测结果不为None，返回True
        public bool IsDetectionComplete()
        {
            bool complete = true;
            for (int i = 0; i < VisionConstHelper.MaxProductNumber; i++)
            {
                if (ProductsInfo[i].HasProduct)
                {
                    if (ProductsInfo[i].ResultStatus == ResultDetectionStatus.None)
                    {
                        complete = false;
                        break;
                    }
                }
            }
            return complete;
        }

        public string GetProductsInfo()
        {
            var resultStatuses = new List<string>();
            for (int i = 1; i <= VisionConstHelper.MaxProductNumber; i++)
            {
                resultStatuses.Add($"( P{i} SN-{ProductsInfo[i - 1].SN} )");
            }
            return string.Join(", ", resultStatuses);
        }

        public string GetSNListToString()
        {
            var result = new List<string>();
            for (int i = 0; i < VisionConstHelper.MaxProductNumber; i++)
            {
                if (ProductsInfo[i].HasProduct)
                {
                    result.Add(ProductsInfo[i].SN);
                }
            }
            return string.Join(",", result);
        }

        public bool IsValidTask()
        {
            for (int i = 0; i < VisionConstHelper.MaxProductNumber; i++)
            {
                if (ProductsInfo[i].HasProduct)
                {
                    return true;
                }
            }
            return false;
        }

    }

    public class ProductInformation
    {
        /// <summary>
        /// 位置编号，1-4
        /// </summary>
        public int Position;
        /// <summary>
        /// 是否有料
        /// </summary>
        public bool HasProduct = false;
        /// <summary>
        /// 产品SN码
        /// </summary>
        public string SN = string.Empty;
        /// <summary>
        /// 检测结果，OK或NG或Fail
        /// 1、用于判定本产品是否检测完成（IsDetectionComplete）
        /// 2、用于推送结果时，只要 PASS 或 FAIL
        /// </summary>
        public ResultDetectionStatus ResultStatus;
        /// <summary>
        /// 详细NG缺陷
        /// </summary>
        public string ResultStatusString = string.Empty;
        /// <summary>
        /// AVC MES过站校验令牌，过站失败时，内容为 error:错误内容
        /// </summary>
        public string CheckToken = string.Empty;
        /// <summary>
        /// 产品是否扫码失败
        /// </summary>
        public bool ScanFail = false;
        //结果图名称
        public string PredictionImagePath = string.Empty;
        //产品是否被人工判定，AI判定不使用
        public bool BeJudgementUser = false;
        /// <summary>
        /// Mes上传结果 bool 类型标识是否需要处理上传失败 string类型标识Mes上传结果
        /// </summary>
        public (bool, string) MesUploadResult = (false, string.Empty);
        public ProductInformation() { }
        public ProductInformation(int position, string sn)
        {
            this.Position = position;

            if (sn == "not")
            {
                HasProduct = false;
                SN = string.Empty;
            }
            else if (sn == "error")
            {
                HasProduct = true;
                SN = string.Empty;
                ScanFail = true;
                SN = $"SN{position}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                ScanFail = false;
            }
            else if (string.IsNullOrEmpty(sn))
            {
                //备用
                HasProduct = false;
                SN = string.Empty;
            }
            else
            {
                HasProduct = true;
                SN = sn;
            }
        }

        public ProductInformation Clone()
        {
            return new ProductInformation()
            {
                Position = this.Position,
                HasProduct = this.HasProduct,
                SN = this.SN,
                ResultStatus = this.ResultStatus,
                ResultStatusString = this.ResultStatusString,
                CheckToken = this.CheckToken,
                ScanFail = this.ScanFail,
                PredictionImagePath = this.PredictionImagePath,
                BeJudgementUser = this.BeJudgementUser,
                MesUploadResult = this.MesUploadResult,
            };
        }
    }

    public enum InspectionTypeEnum
    {
        GluePathInspection, // 胶路检测
        WaterInjectionPortInspection, // 注水口检测
    }

    // 检测原因
    public enum DetectionReasonEnum
    {
        Normal, // 正常检测
        Retest, // 复检
        SelfCheck_empty,    // 空载具点检
        SelfCheck_product,  // 带产品点检
        EmptyCheck, // 空检
    }

    // 检测结果状态枚举
    public enum ResultDetectionStatus
    {
        None,   // 检测过程异常，不能正常完成检测
        OK,     // 正常
        NG,     // 图像检测NG
        Fail,   // 图像检测失败
    }
}
