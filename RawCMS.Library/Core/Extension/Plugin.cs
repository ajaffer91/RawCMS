﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Mina'</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawCMS.Library.UI;
using SlugGenerator;

namespace RawCMS.Library.Core.Extension
{
    /// <summary>
    /// RawCMS plugin definitio
    /// </summary>
    public abstract class Plugin
    {
        public virtual int Priority { get; internal set; } = 1;
        public abstract string Name { get; }
        public abstract string Description { get; }
        public ILogger Logger { get => logger; }
        public AppEngine Engine => engine;

        private readonly AppEngine engine;
        private readonly ILogger logger;

        public  string PluginPath { get; internal set; }

        public virtual string Slug { get { return this.Name.Replace("RawCMS.Plugins.","").GenerateSlug(); } }

        public string GetUIFolder()
        {
            var baseFolder = Path.GetDirectoryName(this.PluginPath);
            baseFolder = Path.GetDirectoryName(baseFolder);
            baseFolder = Path.GetDirectoryName(baseFolder);
            baseFolder = Path.GetDirectoryName(baseFolder);
            return Path.Combine(baseFolder, "UI");
        }

        public Plugin(AppEngine engine, ILogger logger)
        {
            this.engine = engine;
            this.logger = logger;
        }

        /// <summary>
        /// startup application event
        /// </summary>
        public virtual void OnApplicationStart()
        {
            Logger.LogInformation($"Plugin {Name} is notified about app starts");
        }

        /// <summary>
        /// this allow plugin to register its own services
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// this allow the plugin to interact with appengine and application builder
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appEngine"></param>
        public abstract void Configure(IApplicationBuilder app);

        /// <summary>
        /// this metod receive configuration to allow plugin configure itself
        /// </summary>
        /// <param name="configuration"></param>
        public abstract void Setup(IConfigurationRoot configuration);

        /// <summary>
        /// this method allow mvc configuration
        /// </summary>
        /// <param name="builder"></param>
        public abstract void ConfigureMvc(IMvcBuilder builder);


        public virtual UIMetadata GetUIMetadata()
        {
            return new UIMetadata();
        }
    }
}