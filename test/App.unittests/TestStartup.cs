﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace App.unittests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(
           IApplicationBuilder app,
           IHostingEnvironment env)
        {
        }
    }
}
