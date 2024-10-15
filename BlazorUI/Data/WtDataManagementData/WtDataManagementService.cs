﻿using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using WtSbAssistant.Core.Dto;
using WtSbAssistant.Core.Helpers;
using WtSbAssistant.Core.Logger;

namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData
{
    public class WtDataManagementService(WtSbAssistantLogger logger, ConfigHelper config)
    {

        public async Task UploadFileAsync(IBrowserFile file)
        {
            try
            {
                await using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                var data = await reader.ReadToEndAsync();
                var startTimeString = file.Name.Substring(11, 19);

                var strings = data.TrimEnd('\n').Split('\n');
                var logs = strings.Select(s =>
                {
                    try
                    {
                        var timeStrings = new Regex(@"\d{1,2}:\d{1,2}").Match(s).Value.Split(':');
                        return new WtLogItem
                        {
                            Time = (int)new TimeSpan(0, int.Parse(timeStrings[0]), int.Parse(timeStrings[1]))
                                .TotalSeconds,
                            Message = new Regex(@"(?<=\d{1,2}:\d{1,2} ).*").Match(s).Value
                        };
                    }
                    catch (Exception ex)
                    {
                        return new WtLogItem();
                    }
                }).ToList();

                var log = new WtLog
                {
                    Logs = logs,
                    Time = DateTime.ParseExact(startTimeString, "yyyy_MM_dd_HH_mm_ss", CultureInfo.InstalledUICulture).Subtract(TimeSpan.FromSeconds(logs.First().Time))
                };
                using var http = new HttpClient();
                http.BaseAddress = new Uri(config.GetConfig("WebAPI", "Address") ?? string.Empty);

                var response = await http.PostAsJsonAsync("WtLog", log);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
    }
}