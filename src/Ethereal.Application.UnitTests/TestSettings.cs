﻿using System.IO;

namespace Ethereal.Application.UnitTests
{
    public class TestSettings : IEtherealSettings
    {
        public string TempPath => GetTempDirectory();
        public string ExecutablesPath { get; set; }
        
        private static string GetTempDirectory()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}