using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LadderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LadderController : ControllerBase
    {
        IConfiguration configuration;

        public LadderController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        const int WaitForExitSpan = 10 * 1000;

        [HttpPost("start")]
        public async Task<IActionResult> StartLadder()
        {
            try
            {
                var ladderName = this.configuration.GetValue<string>("Ladder:ProcessName");
                if (this.IsProcessRunning(ladderName))
                {
                    return BadRequest($"{ladderName} is already running.");
                }
                
                var result = await StartProcess(this.configuration.GetValue<string>("Ladder:StartCmd"));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("Stop")]
        public async Task<IActionResult> StopLadder()
        {
            try
            {
                var result = await StartProcess(this.configuration.GetValue<string>("Ladder:StopCmd"));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        bool IsProcessRunning(string name)
        {
            var processes = Process.GetProcessesByName(name);

            return processes?.Count() > 0;
        }

        Task<string> StartProcess(string filename)
        {
            return Task.Run(() =>
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = filename;

                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);

                    process.Start();

                    string output = null, error = null;
                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();

                    var exited = process.WaitForExit(WaitForExitSpan);     // (optional) wait up to 10 seconds

                    if (string.IsNullOrWhiteSpace(error))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"Start {process.StartInfo.FileName} successfully.");
                        sb.AppendLine();
                        sb.AppendLine($"Output: {output}");
                        return sb.ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException(error);
                    }
                }
            });
        }
    }
}