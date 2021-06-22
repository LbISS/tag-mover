using System.Collections.Generic;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TagMover.Copy;
using TagMover.Filesystem;
using TagMover.Filter;
using TagMover.Tag;
using TagMover.Tag.TagProcessors;

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

			var filterFunction = filterService.GetFilterFunction(opts.Filter);

			copyService.Copy(opts.Src, opts.Dest, opts.IncludePattern, opts.ExcludePattern, filterFunction);
		}

		protected static void HandleOptionsError(IEnumerable<Error> errs)
		{
			//No specific handling for now
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((_, services) =>
						services
						.AddSingleton<IFilesystemService, FilesystemService>()
						.AddSingleton<IFilterService, AntlrFilterService>()
						.AddSingleton<AntlrFilterQueryVisitor, AntlrFilterQueryVisitor>()
						.AddSingleton<ICopyService, CopyService>()
						.AddSingleton<ITagsService, TagsService>()
						.AddSingleton<ISpecificTagProcessor, ID3v2Processor>()
						.AddSingleton<ISpecificTagProcessor, AsfProcessor>()
						.AddSingleton<ISpecificTagProcessor, XiphProcessor>()
						.AddSingleton<BaseTagProcessor, BaseTagProcessor>()
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
