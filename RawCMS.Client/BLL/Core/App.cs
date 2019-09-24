﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Minà</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using CommandLine;
using RawCMS.Client.BLL.CommandLineParser;
using RawCMS.Client.BLL.Interfaces;
using RawCMS.Client.BLL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace RawCMS.Client.BLL.Core
{
    public class App
    {
        private readonly IConfigService _configService;

        private readonly ILoggerService _loggerService;

        private readonly IRawCmsService _rawCmsService;

        private readonly ITokenService _tokenService;

        private readonly IClientConfigService _clientConfigService;

        #region fun

        public readonly string Message =

@"
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMWNMMMMMMMMMMMMMMMMMMMM ______               _____                M
MWNXWMMMMMMKc,dXMMMMMMMMMMMMMMMMMM | ___ \             /  __ \               M
MWXKXNMMMW0;...cKWMMMMMMMMMMMMMMMM | |_/ /__ ___      _| /  \/_ __ ___  ___  M
MWXKKXMMMNkoxd,.':o0WMMMMMMMWMWMWM |    // _` \ \ /\ / / |   | '_ ` _ \/ __| M
MMMMMMMMXkONN0;    .lXMMMMMMKMKMKM | |\ \ (_| |\ V  V /| \__/\ | | | | \__ \ M
MMMMMMMK:.:kd,.   . .dXNWMMMKMKMKM \_| \_\__,_| \_/\_/  \____/_| |_| |_|___/ M
MMMMMMMO' ... .   . .ll,dXMMKMKMKM                                           M
MMMMMMM0' ......    ,o' .xWMKMKMKM       _____  _ _            _             M
MMMMMMMX: .;:;,.   .l;. .xWMKMKMKM      /  __ \| (_)          | |            M
MMMMMMMWd. .dO, . .:;,' ,0WMKMKMKM      | /  \/| |_  ___ _ __ | |_           M
MMMMMMMMK; .dX: .,kl.;o'.lXMKMKMKM      | |   || | |/ _ \ '_ \| __|          M
MMMMMMMMWO,.c0c .cKd,cOc .xMKMKMKM      | \__/\| | |  __/ | | | |_           M
MMMMMMMMMWk,;kd,,l0XXNXdcl0WKWKWKW       \____/|_|_|\___|_| |_|\__|          M
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
";

        #endregion fun

        public App(
        IConfigService configService,
            ILoggerService loggerService,
        IRawCmsService rawCmsService,
            ITokenService tokenService,
        IClientConfigService clientConfigService)
        {
            _configService = configService;
            _loggerService = loggerService;
            _rawCmsService = rawCmsService;
            _tokenService = tokenService;
            _clientConfigService = clientConfigService;
        }

        public void Run(string[] args)
        {
            Console.WriteLine(Message);

            int ret = CommandLine.Parser.Default.ParseArguments<ClientOptions, LoginOptions, ListOptions, InsertOptions>(args)
                    .MapResult(
                      (ClientOptions opts) => RunClientOptionsCode(opts),
                      (LoginOptions opts) => RunLoginOptionsCode(opts),
                      (ListOptions opts) => RunListOptionsCode(opts),
                      (InsertOptions opts) => RunInsertOptionsCode(opts),
                      (ReplaceOptions opts) => RunReplacetOptionsCode(opts),
                      (DeleteOptions opts) => RunDeleteOptionsCode(opts),
                      (PatchOptions opts) => RunPatchOptionsCode(opts),

                      errs => RunErrorCode(errs));
        }

        private static int RunErrorCode(IEnumerable<Error> errs)
        {
            //_loggerService.Warn("Some parameters are missing:");
            //foreach (MissingRequiredOptionError item in errs)
            //{
            //    _loggerService.Warn($"Missing: {item.NameInfo.NameText}");

            //}
            return 1;
        }

        private int RunPatchOptionsCode(PatchOptions opts)
        {
            throw new NotImplementedException();
        }

        private int RunDeleteOptionsCode(DeleteOptions opts)
        {
            throw new NotImplementedException();
        }

        private int RunReplacetOptionsCode(ReplaceOptions opts)
        {
            throw new NotImplementedException();
        }

        private int RunInsertOptionsCode(InsertOptions opts)
        {
            bool Verbose = opts.Verbose;
            bool Recursive = opts.Recursive;
            bool DryRun = opts.DryRun;
            bool Pretty = opts.Pretty;
            string collection = opts.Collection;
            string filePath = opts.FilePath;
            string folderPath = opts.FolderPath;

            // setting log/console Output
            _loggerService.SetVerbose(Verbose);
            _loggerService.SetPretty(Pretty);

            // check token befare action..
            ConfigFile config = _configService.Load();

            if (config == null)
            {
                _loggerService.Warn("No configuratin file found. Please login before continue.");
                _loggerService.Warn("Program aborted.");
                return 0;
            }

            string token = config.Token;

            if (string.IsNullOrEmpty(token))
            {
                _loggerService.Warn("No token found. Please login before continue.");
                _loggerService.Warn("Program aborted.");
                return 0;
            };

            _loggerService.Debug($"Working into collection: {collection}");

            Dictionary<string, List<string>> listFile = new Dictionary<string, List<string>>();

            // pass a file to options
            if (!string.IsNullOrEmpty(filePath))
            {
                // check if file exists
                if (!File.Exists(filePath))
                {
                    _loggerService.Warn($"File not found: {filePath}");
                    return 0;
                }

                // check if file is valid json

                int check = _rawCmsService.CheckJSON(filePath);

                if (check != 0)
                {
                    _loggerService.Warn("Json is not well-formatted. Skip file.");
                    return 0;
                }
                List<string> filelist = new List<string>
                {
                    filePath
                };
                listFile.Add(collection, filelist);
            }
            else if (!string.IsNullOrEmpty(folderPath))
            {
                string cwd = Directory.GetCurrentDirectory();
                _loggerService.Info($"Current working directory: {cwd}");

                // get all file from folder
                if (!Directory.Exists(folderPath))
                {
                    _loggerService.Warn($"File not found: {filePath}");
                    return 0;
                }

                // This path is a directory
                // get first level path,
                // folder => collection
                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectoryInfo[] subdirs = dInfo.GetDirectories();

                foreach (DirectoryInfo subDir in subdirs)
                {
                    _rawCmsService.ProcessDirectory(Recursive, listFile, subDir.FullName, subDir.Name);
                }
            }
            else
            {
                _loggerService.Warn("At least one of the two options -f (file) or -d (folder) is mandatory.");
                return 0;
            }

            _rawCmsService.ElaborateQueue(listFile, config, Pretty);

            _loggerService.Info($"Processing file complete.");
            return 0;
        }

        private int RunListOptionsCode(ListOptions opts)
        {
            bool Verbose = opts.Verbose;
            _loggerService.SetVerbose(Verbose);

            bool Pretty = opts.Pretty;
            _loggerService.SetPretty(Pretty);

            int PageSize = opts.PageSize;
            int PageNumber = opts.PageNumber;
            string RawQuery = opts.RawQuery;

            string id = opts.Id;
            string collection = opts.Collection;

            //// check token befare action..
            ConfigFile config = _configService.Load();

            if (config == null)
            {
                _loggerService.Warn("No configuratin file found. Please login before continue.");
                _loggerService.Warn("Program aborted.");
                return 0;
            }
            string token = config.Token;

            if (string.IsNullOrEmpty(token))
            {
                _loggerService.Warn("No token found. Please login.");
                return 0;
            };

            _loggerService.Debug($"Perform action in collection: {collection}");

            ListRequest req = new ListRequest
            {
                Collection = collection,
                Token = token,
                PageNumber = PageNumber < 1 ? 1 : PageNumber,
                PageSize = PageSize < 1 ? 10 : PageSize,
                RawQuery = RawQuery,
                Url = config.ServerUrl
            };

            if (!string.IsNullOrEmpty(id))
            {
                req.Id = id;
            }

            RestSharp.IRestResponse responseRawCMS = _rawCmsService.GetData(req);

            _loggerService.Debug($"RawCMS response code: {responseRawCMS.StatusCode}");

            if (!responseRawCMS.IsSuccessful)
            {
                //log.Error($"Error occurred: \n{responseRawCMS.Content}");
                _loggerService.Error($"Error: {responseRawCMS.ErrorMessage}");
            }
            else
            {
                _loggerService.Response(responseRawCMS.Content);
            }

            //switch (responseRawCMS.ResponseStatus)
            //{
            //    case RestSharp.ResponseStatus.Completed:

            //        break;

            //    case RestSharp.ResponseStatus.None:
            //    case RestSharp.ResponseStatus.Error:
            //    case RestSharp.ResponseStatus.TimedOut:
            //    case RestSharp.ResponseStatus.Aborted:

            //    default:
            //        log.Error($"Error response: {responseRawCMS.ErrorMessage}");
            //        break;
            //}

            return 0;
        }

        private int RunLoginOptionsCode(LoginOptions opts)
        {
            bool Verbose = false;
            _loggerService.SetVerbose(Verbose);

            string token = string.Empty;

            opts.ServerUrl = _rawCmsService.FixUrl(opts.ServerUrl);

            try
            {
                token = _tokenService.GetToken(opts);
                _loggerService.Debug($"\n---- TOKEN ------\n{token}\n-----------------");
            }
            catch (ExceptionToken e)
            {
                _loggerService.Error($"token error:");
                _loggerService.Error($"\t{e.Code}, {e.Message}");
                return 2;
            }
            catch (Exception e)
            {
                _loggerService.Error("token error", e);
                return 2;
            }

            if (string.IsNullOrEmpty(token))
            {
                _loggerService.Warn("Unable to get token. check if data are correct and retry.");
                return 2;
            }

            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileconfigname = _clientConfigService.GetValue<string>("ConfigFile");
            fileconfigname = string.Format(fileconfigname, opts.Username);
            string filePath = System.IO.Path.Combine(mydocpath, fileconfigname);

            ConfigFile cf = new ConfigFile
            {
                Token = token,
                CreatedTime = DateTime.Now.ToShortDateString(),
                ServerUrl = opts.ServerUrl,
                User = opts.Username
            };

            _configService.Save(filePath);

            var win = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var lin = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var osx = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            string SetEnv = "SET";
            if (lin || osx)
            {
                SetEnv = "export";
            }

            var os = System.Environment.OSVersion.Platform;
            _loggerService.Info($"os: {os.ToString()}");

            _loggerService.Info($"set enviroinment configuration: (copy, paste and hit return in console):\n\n{SetEnv} RAWCMSCONFIG={filePath}\n");

            return 0;
        }

        private int RunClientOptionsCode(ClientOptions opts)
        {
            throw new NotImplementedException();
        }
    }
}