﻿namespace CROP.API.Models
{
    public record SystemStatus(double CpuUsage, double RamUsage, double DiskUsage, double NetworkUsage);
    public record SystemReport(string Text);
}
