﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MicroService.Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OneValuesController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public OneValuesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // GET api/values
        private static int _count = 0;
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _count++;
            Console.WriteLine($"Get...{_count}");
            if (_count <= 3)
            {
              //  System.Threading.Thread.Sleep(5000);
            }

            return new string[] { $"ClinetService: {DateTime.Now.ToString()}{Configuration["Service:Name"]} {Environment.MachineName} " +
                $"OS: {Environment.OSVersion.VersionString}" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
