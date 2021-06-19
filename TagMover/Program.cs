using System.Collections.Generic;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TagMover.Copy;
using TagMover.Filesystem;
using TagMover.Filter;
using TagMover.Filter.OperatorProcessors;
using TagMover.Tag;

namespace TagMover
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			Parser.Default.ParseArguments<CommandLineOptions>(args)
				  .WithParsed((opts) => RunWithOptions(opts, host))
				  .WithNotParsed(HandleOptionsError);
		}

		protected static void RunWithOptions(CommandLineOptions opts, IHost host)
		{
			var copyService = ActivatorUtilities.GetServiceOrCreateInstance<ICopyService>(host.Services);
			var filterService = ActivatorUtilities.GetServiceOrCreateInstance<IFilterService>(host.Services);

			var filterFunction = filterService.GetFilterFunction(opts.Filter, opts.IncludePattern, opts.ExcludePattern);

			copyService.Copy(opts.Src, opts.Dest, filterFunction);
		}

		protected static void HandleOptionsError(IEnumerable<Error> errs)
		{
			//No specific handling for now
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((_, services) =>
						services
						.AddTransient<IFilesystemService, FilesystemService>()
						.AddTransient<IFilterService, FilterService>()
						.AddTransient<ICopyService, CopyService>()
						.AddTransient<ITagsService, TagsService>()
						.AddTransient<ITagProcessor, ID3v2Processor>()
						.AddTransient<IOperatorProcessor, MissingOperatorProcessor>()
						.AddTransient<IOperatorProcessor, PresentOperatorProcessor>()
			);
	}

	public class CommandLineOptions
	{
		[Option('s', "src", Required = false, HelpText = "Source folder.")]
		public string Src { get; set; }

		[Option('d', "dest", Required = false, HelpText = "Destination folder.")]
		public string Dest { get; set; }

		[Option('f', "filter", Required = false, HelpText = "Filter string. More info in readme.")]
		public string Filter { get; set; }

		[Option('i', "include", Required = false, HelpText = "Include pattern for filepath - regexp format. Files will be copied only if they are passing both filter and pattern.")]
		public string IncludePattern { get; set; }

		[Option('e', "exclude", Required = false, HelpText = "Exclude pattern for filepath - regexp format. Files will be excluded even they are passing filter.")]
		public string ExcludePattern { get; set; }
	}
}
