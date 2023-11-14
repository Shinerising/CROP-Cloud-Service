using Microsoft.EntityFrameworkCore;
using Microsoft.Spatial;

namespace CROP.API.Models
{
    [Index(nameof(StationId), nameof(DeviceId))]
    public class DeviceData
    {
        public string StationId { get; set; } = "";
        public string DeviceId { get; set; } = "";
        public string DeviceType { get; set; } = "";
        public string Name { get; set; } = "";
        public string Direction { get; set; } = "";
        public string Front { get; set; } = "";
        public string Rear_Normal { get; set; } = "";
        public string Rear_Reverse { get; set; } = "";
        public string Length { get; set; } = "";
        public string FrontLength { get; set; } = "";

        /// <summary>
        /// 设备类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 设备子类型
        /// </summary>
        public int Subtype { get; set; }

        /// <summary>
        /// 设备是否显示
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 设备控件距容器左侧的距离
        /// </summary>
        public int Left { get; set; }
        /// <summary>
        /// 设备控件距容器顶部的距离
        /// </summary>
        public int Top { get; set; }
        /// <summary>
        /// 设备控件宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 设备控件高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 控制区域
        /// </summary>
        public GeometryPolygon ControlArea { get; set; } = GeometryFactory.Polygon();

        /// <summary>
        /// 控制参数1
        /// </summary>
        public byte Param1 { get; set; }

        /// <summary>
        /// 控制参数2
        /// </summary>
        public byte Param2 { get; set; }

        /// <summary>
        /// 控制参数3
        /// </summary>
        public byte Param3 { get; set; }
        /// <summary>
        /// 区段名称
        /// </summary>
        public string SectionName { get; set; } = "";

        /// <summary>
        /// 中心点
        /// </summary>
        public GeometryPoint Center { get; set; } = GeometryFactory.Point();

        /// <summary>
        /// 定位线段
        /// </summary>
        public GeometryPoint NormalLine { get; set; } = GeometryFactory.Point();

        /// <summary>
        /// 反位线段
        /// </summary>
        public GeometryPoint ReverseLine { get; set; } = GeometryFactory.Point();
        public GeometryPolygon FrontPoints { get; set; } = GeometryFactory.Polygon();
        public GeometryPolygon RearNormalPoints { get; set; } = GeometryFactory.Polygon();
        public GeometryPolygon RearReversePoints { get; set; } = GeometryFactory.Polygon();
        /// <summary>
        /// 挤岔点
        /// </summary>
        public GeometryPolygon ErrorDot { get; set; } = GeometryFactory.Polygon();
        public byte RetarderCount { get; set; }
        public int RetarderX1 { get; set; }
        public int RetarderY1 { get; set; }
        public int RetarderX2 { get; set; }
        public int RetarderY2 { get; set; }

        /// <summary>
        /// 勾序指示坐标
        /// </summary>
        public GeometryPoint CutCar { get; set; } = GeometryFactory.Point();

        public byte ManualParam;
        public byte NormalLink;
        public byte ReverseLink;
        public int[] ShareDG = new int[3];
        public GeometryPolygon Circle { get; set; } = GeometryFactory.Polygon();
        public GeometryMultiPoint Pillar { get; set; } = GeometryFactory.MultiPoint();
        public GeometryMultiPoint HighPillar { get; set; } = GeometryFactory.MultiPoint();

        public GeometryPoint Radar { get; set; } = GeometryFactory.Point();
        public GeometryPoint RadarText { get; set; } = GeometryFactory.Point();

        public GeometryPoint TimeText { get; set; } = GeometryFactory.Point();


        public int TroubleX1 { get; set; }
        public int TroubleY1 { get; set; }

        public int TroubleX2 { get; set; }
        public int TroubleY2 { get; set; }
        public byte SignalParam { get; set; }

        /// <summary>
        /// 区段线
        /// </summary>
        public GeometryPolygon SectionPoints { get; set; } = GeometryFactory.Polygon();

        /// <summary>
        /// 备用控制区
        /// </summary>
        public GeometryPolygon ControlArea_Alter { get; set; } = GeometryFactory.Polygon();

        public bool DowtyRetarderEnabled { get; set; }
        public int DowtyRetarderCount { get; set; }
        public int DowtyRetarderX1 { get; set; }
        public int DowtyRetarderY1 { get; set; }
        public int DowtyRetarderX2 { get; set; }
        public int DowtyRetarderY2 { get; set; }
        public int DowtyRetarderX3 { get; set; }
        public int DowtyRetarderY3 { get; set; }

        /// <summary>
        /// 停长坐标
        /// </summary>
        public GeometryPoint DTCValuePoint { get; set; } = GeometryFactory.Point();
        /// <summary>
        /// 走长坐标
        /// </summary>
        public GeometryPoint DTCActivePoint { get; set; } = GeometryFactory.Point();
        /// <summary>
        /// 辆数坐标
        /// </summary>
        public GeometryPoint DTCCountPoint { get; set; } = GeometryFactory.Point();

        /// <summary>
        /// 表示器灯位
        /// </summary>
        public GeometryPolygon Indicator { get; set; } = GeometryFactory.Polygon();
        /// <summary>
        /// 表示器灯柱
        /// </summary>
        public GeometryMultiPoint IndicatorPillar { get; set; } = GeometryFactory.MultiPoint();

        /// <summary>
        /// 绝缘节1
        /// </summary>
        public GeometryMultiPoint Insulator0 { get; set; } = GeometryFactory.MultiPoint();
        /// <summary>
        /// 绝缘节2
        /// </summary>
        public GeometryMultiPoint Insulator1 { get; set; } = GeometryFactory.MultiPoint();

        /// <summary>
        /// 尽头线指示
        /// </summary>
        public GeometryPolygon EndLinePoints { get; set; } = GeometryFactory.Polygon();

        /// <summary>
        /// 脱轨器
        /// </summary>
        public GeometryPolygon Derailer { get; set; } = GeometryFactory.Polygon();

        /// <summary>
        /// 锁闭指示
        /// </summary>
        public GeometryPoint Locking { get; set; } = GeometryFactory.Point();

        public int DTCCount { get; set; }
        /// <summary>
        /// 减速器矩形1
        /// </summary>
        public GeometryPolygon Rect0 { get; set; } = GeometryFactory.Polygon();
        /// <summary>
        /// 减速器矩形2
        /// </summary>
        public GeometryPolygon Rect1 { get; set; } = GeometryFactory.Polygon();

        /// <summary>
        /// 后方线点集合
        /// </summary>
        public GeometryPolygon RearPoints { get; set; } = GeometryFactory.Polygon();

        /// <summary>
        /// 测长信息
        /// </summary>
        public GeometryPoint DTC { get; set; } = GeometryFactory.Point();

        /// <summary>
        /// 踏板设备
        /// </summary>
        public GeometryPoint Pedal { get; set; } = GeometryFactory.Point();
        /// <summary>
        /// 测重设备
        /// </summary>
        public GeometryPoint Weight { get; set; } = GeometryFactory.Point();
        /// <summary>
        /// 测重文本位置
        /// </summary>
        public GeometryPoint CutWeight { get; set; } = GeometryFactory.Point();

        /// <summary>
        /// 记轴文本位置
        /// </summary>
        public GeometryPoint CutAxel { get; set; } = GeometryFactory.Point();
        /// <summary>
        /// 入速位置
        /// </summary>
        public GeometryPoint InSpeed { get; set; } = GeometryFactory.Point();
        /// <summary>
        /// 出速位置
        /// </summary>
        public GeometryPoint OutSpeed { get; set; } = GeometryFactory.Point();
    }
}
