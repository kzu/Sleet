﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Common;

namespace Sleet
{
    internal static class Util
    {
        internal static ISleetFileSystem CreateFileSystemOrThrow(LocalSettings settings, string sourceName, LocalCache cache)
        {
            var fileSystem = FileSystemFactory.CreateFileSystem(settings, cache, sourceName);

            if (fileSystem == null)
            {
                throw new InvalidOperationException("Unable to find source. Verify that the --source parameter is correct and that sleet.json contains the named source.");
            }

            return fileSystem;
        }

        internal static void ValidateRequiredOptions(IEnumerable<CommandOption> required)
        {
            // Validate parameters
            foreach (var requiredOption in required)
            {
                if (!requiredOption.HasValue())
                {
                    throw new ArgumentException($"Missing required parameter --{requiredOption.LongName}.");
                }
            }
        }

        internal static void SetVerbosity(ILogger log, bool verbose)
        {
            if (log is ConsoleLogger consoleLogger)
            {
                if (verbose)
                {
                    consoleLogger.VerbosityLevel = LogLevel.Verbose;
                }
                else
                {
                    consoleLogger.VerbosityLevel = LogLevel.Information;
                    consoleLogger.CollapseMessages = true;
                }
            }
        }
    }
}
