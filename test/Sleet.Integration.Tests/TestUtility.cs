﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Sleet.Test
{
    /// <summary>
    /// Duplicated from SleetLib.Tests due to a VS 2017 RC2 Issue.
    /// </summary>
    public static class TestUtility
    {
        public static Stream GetResource(string name)
        {
            var path = $"Sleet.Test.compiler.resources.{name}";
            return typeof(TestUtility).GetTypeInfo().Assembly.GetManifestResourceStream(path);
        }

        public static JObject CreateConfigWithLocal(string sourceName, string sourcePath, string baseUri)
        {
            // Create the config template
            var json = new JObject
            {
                { "username", "test" },
                { "useremail", "test@tempuri.org" }
            };
            var sourcesArray = new JArray();
            json.Add("sources", sourcesArray);

            var folderJson = new JObject
            {
                { "name", sourceName },
                { "type", "local" },
                { "path", sourcePath }
            };
            if (!string.IsNullOrEmpty(baseUri))
            {
                folderJson.Add("baseURI", baseUri);
            }

            sourcesArray.Add(folderJson);

            return json;
        }

        public static IEnumerable<FileInfo> GetJsonFiles(string root)
        {
            var dir = new DirectoryInfo(root);

            foreach (var file in dir.EnumerateFiles("*.json", SearchOption.AllDirectories))
            {
                yield return file;
            }
        }

        public static void WalkJson(string root, Action<FileInfo, JObject, string> walker)
        {
            var valid = false;

            foreach (var file in GetJsonFiles(root))
            {
                var json = JsonUtility.LoadJson(file);

                var tokens = json.Descendants().ToArray();

                foreach (var token in tokens)
                {
                    if (token.Type == JTokenType.String
                        || token.Type == JTokenType.Uri)
                    {
                        valid = true;
                        walker(file, json, token.Value<string>());
                    }
                }
            }

            // Ensure that the input was valid
            Assert.True(valid);
        }
    }
}