using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace VoltRpc.Benchmarks.Core
{
    public class VoltRpcConfig : Attribute, IConfigSource
    {
        public IConfig Config { get; }

        public VoltRpcConfig()
        {
            ManualConfig config = ManualConfig.CreateEmpty().AddJob(new Job(Job.Default)
            {
                Environment =
                {
                    Jit = Jit.Default, 
                    Platform = Platform.AnyCpu
                }
            });
            IConfig defaultConfig = DefaultConfig.Instance;
            
            config.AddColumnProvider(DefaultColumnProviders.Instance);
            config.AddLogger(defaultConfig.GetLoggers().ToArray());
            config.AddAnalyser(defaultConfig.GetAnalysers().ToArray());
            config.AddValidator(defaultConfig.GetValidators().ToArray());
            config.AddExporter(new CsvExporter(CsvSeparator.Comma, new SummaryStyle(CultureInfo.CurrentCulture, true, SizeUnit.KB, TimeUnit.Microsecond, false)));
            config.AddExporter(MarkdownExporter.Default);
            
            Config = config;
        }
    }
}