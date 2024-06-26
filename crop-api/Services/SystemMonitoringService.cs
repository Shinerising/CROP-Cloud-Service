﻿using CROP.API.Data;
using CROP.API.Models;
using CROP.API.Utility;
using Redis.OM;
using Redis.OM.Searching;

namespace CROP.API.Services
{
    public class SystemMonitoringService(RedisConnectionProvider provider) : BackgroundService
    {
        private readonly RedisCollection<SystemStatus> _systemStatus = (RedisCollection<SystemStatus>)provider.RedisCollection<SystemStatus>();
        private readonly RedisCollection<SystemReport> _systemReport = (RedisCollection<SystemReport>)provider.RedisCollection<SystemReport>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var name = Environment.MachineName;
                try
                {
                    if (!await _systemStatus.AnyAsync(item => item.Id == name))
                    {
                        var status = await GetSystemStatus();
                        await _systemStatus.InsertAsync(status);
                    }
                    else
                    {
                        var old = await _systemStatus.FirstAsync(item => item.Id == name);
                        if (DateTimeOffset.Now - old.Time >= TimeSpan.FromSeconds(10))
                        {
                            var status = await GetSystemStatus();
                            await _systemStatus.UpdateAsync(status);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    if (!await _systemReport.AnyAsync(item => item.Id == name))
                    {
                        var report = await GetSystemReport();
                        await _systemReport.InsertAsync(report);
                    }
                    else
                    {
                        var old = await _systemReport.FirstAsync(item => item.Id == name);
                        if (DateTimeOffset.Now - old.Time >= TimeSpan.FromHours(1))
                        {
                            var report = await GetSystemReport();
                            await _systemReport.UpdateAsync(report);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }

        private static async Task<SystemStatus> GetSystemStatus()
        {
            SystemStatus status;
            var name = Environment.MachineName;
            if (Utils.IsWindows)
            {
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject Win32_Processor | Measure-Object -Property LoadPercentage -Average; Write-Host $CompObject.Average"), out double cpu);
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject WIN32_OperatingSystem; Write-Host ((($CompObject.TotalVisibleMemorySize-$CompObject.FreePhysicalMemory)*100)/$CompObject.TotalVisibleMemorySize)"), out double ram);
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject Win32_LogicalDisk; Write-Host (($CompObject[0].size-$CompObject[0].freespace)*100/$CompObject[0].size)"), out double disk);
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject Win32_PerfFormattedData_Tcpip_NetworkInterface -filter 'BytesTotalPersec>0'; Write-Host ($CompObject.BytesTotalPerSec*800/$CompObject.CurrentBandwidth)"), out double network);
                status = new SystemStatus() { Id = name, CpuUsage = cpu, RamUsage = ram, DiskUsage = disk, NetworkUsage = network };
            }
            else
            {
                _ = double.TryParse(await Utils.ExecuteCommand("vmstat | sed -n 3p | awk '{ print 100-$15 }'"), out double cpu);
                _ = double.TryParse(await Utils.ExecuteCommand("free | sed -n 2p | awk '{ print $3/$2*100 }'"), out double ram);
                _ = double.TryParse(await Utils.ExecuteCommand("df | sed -n 2p | awk '{ print $3/$2*100 }'"), out double disk);
                _ = double.TryParse(await Utils.ExecuteCommand("ifstat 0.1 1 | sed -n 3p | awk '{ print ($1+$2)*800/1000000 }'"), out double network);
                status = new SystemStatus() { Id = name, CpuUsage = cpu, RamUsage = ram, DiskUsage = disk, NetworkUsage = network };
            }
            return status;
        }

        private static async Task<SystemReport> GetSystemReport()
        {
            SystemReport report;
            var name = Environment.MachineName;
            if (Utils.IsWindows)
            {
                string systemreport = await Utils.ExecuteCommand("systeminfo");
                report = new SystemReport() { Id = name, Text = systemreport };
            }
            else
            {
                string systemreport = await Utils.ExecuteCommand("lshw");
                report = new SystemReport() { Id = name, Text = systemreport };
            }
            return report;
        }
    }
}
