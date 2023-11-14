using Microsoft.EntityFrameworkCore;
using Microsoft.Spatial;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace CROP.API.Models
{
    [Index(nameof(StationId), nameof(DeviceId))]
    public class DeviceData
    {
        public string StationId { get; set; } = "";
        public string DeviceId { get; set; } = "";
        public string DeviceType { get; set; } = "";
        public string DeviceName { get; set; } = "";
        public string DeviceDirection { get; set; } = "";
        public string Front { get; set; } = "";
        public string Rear_Normal { get; set; } = "";
        public string Rear_Reverse { get; set; } = "";
        public string Length { get; set; } = "";
        public string FrontLength { get; set; } = "";
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
        public GeometryMultiPoint FrontPoints { get; set; } = GeometryFactory.MultiPoint();
        public GeometryMultiPoint RearNormalPoints { get; set; } = GeometryFactory.MultiPoint();
        public GeometryMultiPoint RearReversePoints { get; set; } = GeometryFactory.MultiPoint();
        /// <summary>
        /// 挤岔点
        /// </summary>
        public GeometryPoint ErrorDot { get; set; } = GeometryFactory.Point();
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
        public byte Direction { get; set; }

        /// <summary>
        /// 区段线
        /// </summary>
        public GeometryMultiPoint SectionPoints { get; set; } = GeometryFactory.MultiPoint();

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
        public GeometryPoint Indicator { get; set; } = GeometryFactory.Point();
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
        public GeometryMultiPoint EndLinePoints { get; set; } = GeometryFactory.MultiPoint();

        /// <summary>
        /// 脱轨器
        /// </summary>
        public GeometryMultiPoint Derailer { get; set; } = GeometryFactory.MultiPoint();

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
        public GeometryMultiPoint RearPoints { get; set; } = GeometryFactory.MultiPoint();

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
    public static class GraphLoader
    {
        internal enum DeviceType : byte
        {
            Switch = 1,
            Signal = 2,
            Section = 3,
            Button = 4,
            Retarder = 6
        }
        /// <summary>
        /// 设备子类型（各项目解释可参见字符串字典）
        /// </summary>
        internal enum DeviceSubtype : byte
        {
            Switch_Branching = 0x10,
            Switch_Single = 0x11,
            Switch_Equilateral = 0x12,
            Switch_Backward = 0x13,
            Switch_Local = 0x14,

            Signal_Hump = 0x20,
            Signal_Shunting = 0x21,
            Signal_Outbound = 0x22,
            Signal_OutShunting = 0x23,
            Signal_Inbound = 0x24,
            Signal_Virtual = 0x25,
            Signal_VirtualTerminal = 0x26,
            Signal_InShunting = 0x27,

            Section_Middle = 0x30,
            Section_Outbound = 0x31,
            Section_Virtual = 0x32,
            Section_Connect = 0x33,
            Section_CTC = 0x34,
            Section_Track = 0x35,
            Section_Inbound = 0x36,
            Section_PreRetarder = 0x37,
            Section_MiddleSimple = 0x38,
            Section_End = 0x39,
            Section_DowtyRetarder = 0x3A,
            Section_DTC = 0x3D,

            Button_TwoWords = 0x40,
            Button_FourWords = 0x41,
            Button_Pushing = 0x42,
            Button_Switch = 0x43,
            Button_Control = 0x44,
            Button_Hump = 0x45,
            Button_NotRoute = 0x46,
            Button_Leading = 0x47,
            Button_OnHump = 0x48,
            Button_AgreeCancel = 0x4A,
            Button_Rolling = 0x4B,
            Button_Long = 0x4C,

            Retarder_Default = 0x60,
            Retarder_MasterDouble = 0x62,
            Retarder_MasterSingle = 0x63,
            Retarder_GroupDouble = 0x64,
            Retarder_GroupSingle = 0x65,
            Retarder_TangentPointDouble = 0x66,
            Retarder_TangentPointSingle = 0x67,
            Retarder_GroupTangentDouble = 0x68,
            Retarder_GroupTangentSingle = 0x69
        }
        public static bool NextDeviceLine(StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                if (sr.ReadLine() == ";Device Data Start")
                {
                    return true;
                }
            }
            return false;
        }
        public static T ReadDatLine<T>(StreamReader sr, T defaultValue)
        {
            string text = string.Empty;
            while (!sr.EndOfStream)
            {
                var array = sr.ReadLine()?.Split(';');
                if (array != null &&array.Length != 0)
                {
                    text = array[0].Trim();
                    if (text != string.Empty)
                    {
                        break;
                    }
                }
            }
            if (sr.EndOfStream)
            {
                return defaultValue;
            }
            else if (Type.GetTypeCode(typeof(T)) == TypeCode.String)
            {
                return (T)Convert.ChangeType(text, typeof(T));
            }
            else if (Type.GetTypeCode(typeof(T)) == TypeCode.Boolean)
            {
                if (text != "0")
                {
                    return (T)Convert.ChangeType(true, typeof(T));
                }
                return (T)Convert.ChangeType(false, typeof(T));
            }
            else
            {
                int.TryParse(text, out int number);
                return (T)Convert.ChangeType(number, typeof(T));
            }
        }
        public static void LoadMapDataFromDAT(string path, List<DeviceData> deviceList)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new(fs, Encoding.GetEncoding("gb2312"), true);

            //map.Name = ReadDatLine(sr, "");

            //map.Width = ReadDatLine(sr, 0);
            //map.Height = ReadDatLine(sr, 0);

            int mapDirection = ReadDatLine(sr, 0);
            int count = ReadDatLine(sr, 0);
            //map.DeviceCount = (ushort)count;
            //map.DowtyRetarderLevel = (ushort)ReadDatLine(sr, 0);

            while (NextDeviceLine(sr))
            {
                int id = ReadDatLine(sr, 0);
                int subtype = ReadDatLine(sr, 0);
                int left, top, x, y, w, h;
                bool isVisible = ReadDatLine(sr, true);
                int deviceType = subtype / 16;
                int deviceSubtype = subtype;

                var device = deviceList.FirstOrDefault(item => item.Id == id);

                if (device == null)
                {
                    device = new DeviceData()
                    {
                        Id = id,
                        Type = deviceType,
                        Subtype = deviceSubtype,
                        IsVisible = isVisible
                    };
                    deviceList.Add(device);
                }
                else
                {
                    device.Type = deviceType;
                    device.Subtype = deviceSubtype;
                    device.IsVisible = isVisible;
                }

                if (deviceType == (int)DeviceType.Switch)
                {
                    device.Name = ReadDatLine(sr, "").Replace("$", "");
                    left = ReadDatLine(sr, 0);
                    top = ReadDatLine(sr, 0);

                    device.Left = left;
                    device.Top = top;

                    device.Param1 = (byte)ReadDatLine(sr, 0);
                    device.Param2 = (byte)ReadDatLine(sr, 0);
                    device.Param3 = (byte)ReadDatLine(sr, 0);
                    device.ManualParam = (byte)ReadDatLine(sr, 0);

                    device.NormalLink = (byte)ReadDatLine(sr, 0);
                    device.ReverseLink = (byte)ReadDatLine(sr, 0);

                    x = ReadDatLine(sr, 0) - left;
                    y = ReadDatLine(sr, 0) - top;

                    device.Center = GeometryPoint.Create(x,y);
                    var points = GeometryFactory.MultiPoint().Point(x,y);
                    for (int j = 0; j < 3; j += 1)
                    {
                        x = ReadDatLine(sr, 0);
                        y = ReadDatLine(sr, 0);
                        if (x != 0 && y != 0)
                        {
                            points.Point(x - left, y - top);
                        }
                    }
                    device.FrontPoints = points.Build();

                    points = GeometryFactory.MultiPoint();
                    for (int j = 0; j < 4; j += 1)
                    {
                        x = ReadDatLine(sr, 0);
                        y = ReadDatLine(sr, 0);
                        if (x != 0 && y != 0)
                        {
                            if (j == 0)
                            {
                                device.NormalLine = GeometryPoint.Create(x - left, y - top);
                            }
                            points.Point(x - left, y - top);
                        }
                    }
                    device.RearNormalPoints = points.Build();

                    for (int j = 0; j < 4; j += 1)
                    {
                        x = ReadDatLine(sr, 0);
                        y = ReadDatLine(sr, 0);
                        if (x != 0 && y != 0)
                        {
                            if (j == 0)
                            {
                                device.ReverseLine = GeometryPoint.Create(x - left, y - top);
                            }
                            points.Point(x - left, y - top);
                        }
                    }
                    device.RearReversePoints = points.Build();

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    device.ControlArea = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.ErrorDot = GeometryPoint.Create(x - left, y - top);

                    device.SectionName = ReadDatLine(sr, "");

                    device.RetarderCount = (byte)ReadDatLine(sr, 0);
                    device.RetarderX1 = ReadDatLine(sr, 0) - left;
                    device.RetarderY1 = ReadDatLine(sr, 0) - top;
                    device.RetarderX2 = ReadDatLine(sr, 0) - left;
                    device.RetarderY2 = ReadDatLine(sr, 0) - top;

                    device.ShareDG[0] = ReadDatLine(sr, 0);
                    device.ShareDG[1] = ReadDatLine(sr, 0);
                    device.ShareDG[2] = ReadDatLine(sr, 0);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.CutCar = GeometryPoint.Create(x - left, y - top);
                    }
                }
                else if (deviceType == (int)DeviceType.Signal)
                {
                    device.Param1 = (byte)ReadDatLine(sr, 0);
                    device.Param2 = (byte)ReadDatLine(sr, 0);
                    device.Param3 = (byte)ReadDatLine(sr, 0);
                    device.ManualParam = (byte)ReadDatLine(sr, 0);

                    device.NormalLink = (byte)ReadDatLine(sr, 0);
                    device.ReverseLink = (byte)ReadDatLine(sr, 0);

                    device.SignalParam = (byte)ReadDatLine(sr, 0);
                    device.Direction = (byte)ReadDatLine(sr, 0);

                    device.Name = ReadDatLine(sr, "").Replace("$", "");
                    left = ReadDatLine(sr, 0);
                    top = ReadDatLine(sr, 0);

                    device.Left = left;
                    device.Top = top;

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.Pillar = GeometryFactory.MultiPoint().Point(x - left, y - top).Point(w - left, h - top).Build();
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.Circle = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.ControlArea = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.TroubleX1 = x - left;
                        device.TroubleY1 = y - top;
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.TroubleX2 = x - left;
                        device.TroubleY2 = y - top;
                    }

                    if (subtype == (int)DeviceSubtype.Signal_Hump)
                    {
                        var width = device.Circle.Rings[0].Points[1].X - device.Circle.Rings[0].Points[0].X * (mapDirection == 0 ? 1 : -1);
                        var points = GeometryFactory.MultiPoint().Point(device.Pillar.Points[0].X + width / 2, device.Pillar.Points[0].Y).Point(device.Pillar.Points[1].X + width / 2, device.Pillar.Points[1].Y).Build();
                        device.Pillar = points;
                        x = (int)points.Points[0].X;
                        y = (int)(points.Points[0].Y + points.Points[1].Y) / 2;
                        w = (int)((device.Circle.Rings[0].Points[0].X + device.Circle.Rings[0].Points[1].X) / 2);
                        h = (int)((device.Circle.Rings[0].Points[0].Y + device.Circle.Rings[0].Points[1].Y) / 2);
                        device.HighPillar = GeometryFactory.MultiPoint().Point(x, y).Point(w, h).Build();
                    }

                    device.SectionName = ReadDatLine(sr, "");

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.TimeText = GeometryPoint.Create(x - left, y - top);
                    }
                }
                else if (deviceType == (int)DeviceType.Section)
                {
                    device.Param1 = (byte)ReadDatLine(sr, 0);
                    device.Param2 = (byte)ReadDatLine(sr, 0);
                    device.Param3 = (byte)ReadDatLine(sr, 0);
                    device.ManualParam = (byte)ReadDatLine(sr, 0);

                    device.NormalLink = (byte)ReadDatLine(sr, 0);
                    device.DowtyRetarderEnabled = (ReadDatLine(sr, 0) & 0x40) != 0;

                    device.Name = ReadDatLine(sr, "").Replace("$", "");
                    left = ReadDatLine(sr, 0);
                    top = ReadDatLine(sr, 0);

                    if (left == 0 && top == 0)
                    {
                        left = ReadDatLine(sr, 0);
                        top = ReadDatLine(sr, 0);

                        device.Left = left;
                        device.Top = top;

                        x = 0;
                        y = 0;
                    }
                    else
                    {
                        device.Left = left;
                        device.Top = top;

                        x = ReadDatLine(sr, 0) - left;
                        y = ReadDatLine(sr, 0) - top;
                    }

                    device.Center = GeometryPoint.Create(x, y);

                    var points = GeometryFactory.MultiPoint().Point(x, y);
                    for (int j = 0; j < 3; j += 1)
                    {
                        x = ReadDatLine(sr, 0);
                        y = ReadDatLine(sr, 0);
                        if (x != 0 && y != 0)
                        {
                            points.Point(x - left, y - top);
                        }
                    }
                    device.SectionPoints = points.Build();

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.ControlArea = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.ControlArea_Alter = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();
                    }

                    device.SectionName = ReadDatLine(sr, "");

                    device.DowtyRetarderCount = ReadDatLine(sr, 0);
                    device.DowtyRetarderX1 = ReadDatLine(sr, 0);
                    device.DowtyRetarderY1 = ReadDatLine(sr, 0);
                    device.DowtyRetarderX2 = ReadDatLine(sr, 0);
                    device.DowtyRetarderY2 = ReadDatLine(sr, 0);

                    device.DTCCount = ReadDatLine(sr, 0);
                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.DTCCountPoint = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.TimeText = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.Indicator = GeometryPoint.Create(x - left, y - top);

                    device.ShareDG[0] = ReadDatLine(sr, 0);
                    device.ShareDG[1] = ReadDatLine(sr, 0);

                    device.DowtyRetarderX3 = ReadDatLine(sr, 0);
                    device.DowtyRetarderY3 = ReadDatLine(sr, 0);
                }
                else if (deviceType == (int)DeviceType.Button)
                {
                    device.Param1 = (byte)ReadDatLine(sr, 0);
                    device.Param2 = (byte)ReadDatLine(sr, 0);
                    device.Param3 = (byte)ReadDatLine(sr, 0);

                    device.Name = ReadDatLine(sr, "").Replace("$", "");

                    left = ReadDatLine(sr, 0);
                    top = ReadDatLine(sr, 0);

                    if (left != 0 || top != 0)
                    {
                        device.Left = left;
                        device.Top = top;
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.ControlArea = GeometryFactory.Polygon().Ring(x - left, y - top).Build();
                    }
                }
                else if (deviceType == (int)DeviceType.Retarder)
                {
                    device.Param1 = (byte)ReadDatLine(sr, 0);
                    device.Param2 = (byte)ReadDatLine(sr, 0);
                    device.Param3 = (byte)ReadDatLine(sr, 0);

                    device.Name = ReadDatLine(sr, "").Replace("$", "");

                    left = ReadDatLine(sr, 0);
                    top = ReadDatLine(sr, 0);

                    device.Left = left;
                    device.Top = top;

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    device.Rect0 = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    device.Rect1 = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();

                    var points = GeometryFactory.MultiPoint();
                    for (int j = 0; j < 3; j += 1)
                    {
                        x = ReadDatLine(sr, 0);
                        y = ReadDatLine(sr, 0);
                        if (x != 0 && y != 0)
                        {
                            points.Point(x - left, y - top);
                        }
                    }
                    device.FrontPoints = points.Build();

                    points = GeometryFactory.MultiPoint();
                    for (int j = 0; j < 3; j += 1)
                    {
                        x = ReadDatLine(sr, 0);
                        y = ReadDatLine(sr, 0);
                        if (x != 0 && y != 0)
                        {
                            points.Point(x - left, y - top);
                        }
                    }
                    device.RearPoints = points.Build();

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.InSpeed = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.OutSpeed = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.DTC = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.CutAxel = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.CutWeight = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    w = ReadDatLine(sr, 0);
                    h = ReadDatLine(sr, 0);
                    if (x != 0 && y != 0)
                    {
                        device.ControlArea = GeometryFactory.Polygon().Ring(x - left, y - top).LineTo(w - left, h - top).Build();
                    }

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.Radar = GeometryPoint.Create(x - left, y - top);

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.Pedal = GeometryPoint.Create(x - left, y - top);

                    device.SectionName = ReadDatLine(sr, "");

                    x = ReadDatLine(sr, 0);
                    y = ReadDatLine(sr, 0);
                    device.Indicator = GeometryPoint.Create(x - left, y - top);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
