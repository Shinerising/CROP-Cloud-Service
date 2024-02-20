using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CROP.API.Models
{
    [Table("Analysis_Device")]
    [Index(nameof(StationId))]
    public class DeviceData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? PrimaryId { get; set; }
        public string StationId { get; set; } = "";
        public int Id { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 设备子类型
        /// </summary>
        public int Subtype { get; set; }
        public string Name { get; set; } = "";

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
        public NpgsqlPolygon? ControlArea { get; set; }

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
        public NpgsqlPoint? Center { get; set; }

        /// <summary>
        /// 定位线段
        /// </summary>
        public NpgsqlPoint? NormalLine { get; set; }

        /// <summary>
        /// 反位线段
        /// </summary>
        public NpgsqlPoint? ReverseLine { get; set; }
        public NpgsqlPath? FrontPoints { get; set; }
        public NpgsqlPath? RearNormalPoints { get; set; }
        public NpgsqlPath? RearReversePoints { get; set; }
        /// <summary>
        /// 挤岔点
        /// </summary>
        public NpgsqlPoint? ErrorDot { get; set; }
        public byte RetarderCount { get; set; }
        public int RetarderX1 { get; set; }
        public int RetarderY1 { get; set; }
        public int RetarderX2 { get; set; }
        public int RetarderY2 { get; set; }

        /// <summary>
        /// 勾序指示坐标
        /// </summary>
        public NpgsqlPoint? CutCar { get; set; }

        public byte ManualParam;
        public byte NormalLink;
        public byte ReverseLink;
        public int[] ShareDG = new int[3];
        public NpgsqlPolygon? Circle { get; set; }
        public NpgsqlPath? Pillar { get; set; }
        public NpgsqlPath? HighPillar { get; set; }

        public NpgsqlPoint? Radar { get; set; }
        public NpgsqlPoint? RadarText { get; set; }

        public NpgsqlPoint? TimeText { get; set; }


        public int TroubleX1 { get; set; }
        public int TroubleY1 { get; set; }

        public int TroubleX2 { get; set; }
        public int TroubleY2 { get; set; }
        public byte SignalParam { get; set; }
        public byte Direction { get; set; }

        /// <summary>
        /// 区段线
        /// </summary>
        public NpgsqlPath? SectionPoints { get; set; }

        /// <summary>
        /// 备用控制区
        /// </summary>
        public NpgsqlPolygon? ControlArea_Alter { get; set; }

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
        public NpgsqlPoint? DTCValuePoint { get; set; }
        /// <summary>
        /// 走长坐标
        /// </summary>
        public NpgsqlPoint? DTCActivePoint { get; set; }
        /// <summary>
        /// 辆数坐标
        /// </summary>
        public NpgsqlPoint? DTCCountPoint { get; set; }

        /// <summary>
        /// 表示器灯位
        /// </summary>
        public NpgsqlPoint? Indicator { get; set; }
        /// <summary>
        /// 表示器灯柱
        /// </summary>
        public NpgsqlPath? IndicatorPillar { get; set; }

        /// <summary>
        /// 绝缘节1
        /// </summary>
        public NpgsqlPath? Insulator0 { get; set; }
        /// <summary>
        /// 绝缘节2
        /// </summary>
        public NpgsqlPath? Insulator1 { get; set; }

        /// <summary>
        /// 尽头线指示
        /// </summary>
        public NpgsqlPath? EndLinePoints { get; set; }

        /// <summary>
        /// 脱轨器
        /// </summary>
        public NpgsqlPath? Derailer { get; set; }

        /// <summary>
        /// 锁闭指示
        /// </summary>
        public NpgsqlPoint? Locking { get; set; }

        public int DTCCount { get; set; }
        /// <summary>
        /// 减速器矩形1
        /// </summary>
        public NpgsqlPolygon? Rect0 { get; set; }
        /// <summary>
        /// 减速器矩形2
        /// </summary>
        public NpgsqlPolygon? Rect1 { get; set; }

        /// <summary>
        /// 后方线点集合
        /// </summary>
        public NpgsqlPath? RearPoints { get; set; }

        /// <summary>
        /// 测长信息
        /// </summary>
        public NpgsqlPoint? DTC { get; set; }

        /// <summary>
        /// 踏板设备
        /// </summary>
        public NpgsqlPoint? Pedal { get; set; }
        /// <summary>
        /// 测重设备
        /// </summary>
        public NpgsqlPoint? Weight { get; set; }
        /// <summary>
        /// 测重文本位置
        /// </summary>
        public NpgsqlPoint? CutWeight { get; set; }

        /// <summary>
        /// 记轴文本位置
        /// </summary>
        public NpgsqlPoint? CutAxel { get; set; }
        /// <summary>
        /// 入速位置
        /// </summary>
        public NpgsqlPoint? InSpeed { get; set; }
        /// <summary>
        /// 出速位置
        /// </summary>
        public NpgsqlPoint? OutSpeed { get; set; }
    }
}