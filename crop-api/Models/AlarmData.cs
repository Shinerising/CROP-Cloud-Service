﻿using Redis.OM.Modeling;

namespace CROP.API.Models
{
    [Document]
    public class AlarmData
    {
        [Indexed]
        public string Station { get; set; } = "";
        public string Version { get; set; } = "";
        [Indexed(Sortable = true)]
        public DateTimeOffset Time { get; set; } = DateTimeOffset.MinValue;
        [Indexed(Sortable = true)]
        public DateTimeOffset SaveTime { get; set; } = DateTimeOffset.Now;
        public string Data { get; set; } = "";
    }
}
