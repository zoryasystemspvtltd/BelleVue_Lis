using LIS.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HIS.Api.Simujlator.App_Start
{
    public static class GlobalScheduler
    {
        private static ILogger _logger;
        private static System.Timers.Timer _timer;

        private static readonly string DefaultConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static readonly int SchedulerIntervalMinute = Convert.ToInt32(ConfigurationManager.AppSettings["SchedulerIntervalMinute"]);
        private static readonly int DailySchedularHour = Convert.ToInt32(ConfigurationManager.AppSettings["DailySchedularHour"]);

        public static void StartScheduler(ILogger logger)
        {
            //_logger = logger;


            //_timer = new System.Timers.Timer();
            //_timer.Elapsed += _timer_Elapsed;
            //_timer.Interval = 1000 * 60 * SchedulerIntervalMinute; // One Hour
            //_timer.Enabled = true;
            //_logger.LogInfo("Synchronization Scheduler Enabled.");

            //_timer_Elapsed(null, null);
        }

        private static async void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var nowDate = DateTime.Now;

            _logger.LogInfo("Synchronization Scheduler Elapsed Started.");

            await SyncTestRequisition(); // 1 Hour

            if (nowDate.Hour == DailySchedularHour)
            {
                await SyncTestMaster(); // 1 Day
                await SyncTestParameter(); // 1 Day
            }

            _logger.LogInfo("Synchronization Scheduler Elapsed End.");
        }


        private static async Task SyncTestRequisition()
        {
            try
            {
                _logger.LogInfo("Synchronization Test Requisition Started.");
                using (SqlConnection con = new SqlConnection(DefaultConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_HISDataIntegration", con))
                    {
                        cmd.CommandTimeout = 60 * 30;
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        var ststus = await cmd.ExecuteNonQueryAsync();
                    }
                }

                _logger.LogInfo("Synchronization Test Requisition End.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private static async Task SyncTestMaster()
        {
            try
            {
                _logger.LogInfo("Synchronization Test Master Started.");
                using (SqlConnection con = new SqlConnection(DefaultConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_HISTestIntegration", con))
                    {
                        cmd.CommandTimeout = 60 * 30;
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        var ststus = await cmd.ExecuteNonQueryAsync();
                    }
                }
                _logger.LogInfo("Synchronization Test Master End.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private static async Task SyncTestParameter()
        {
            try
            {
                _logger.LogInfo("Synchronization Test Parameter Started.");
                using (SqlConnection con = new SqlConnection(DefaultConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_HISParameterIntegration", con))
                    {
                        cmd.CommandTimeout = 60 * 30;
                        cmd.CommandType = CommandType.StoredProcedure;
                        await con.OpenAsync();
                        var ststus = await cmd.ExecuteNonQueryAsync();
                    }
                }
                _logger.LogInfo("Synchronization Test Parameter End.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
    }
}