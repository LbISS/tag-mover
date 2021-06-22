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
		/// <summary>Defines the entry point of the application.</summary>
		/// <param name="args">The arguments.</param>
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			Parser.Default.ParseArguments<CommandLineOptions>(args)
				  .WithParsed((opts) => RunWithOptions(opts, host))
				  .WithNotParsed(HandleOptionsError);
		}

		/// <summary>Runs main algorithm with the cmd options.</summary>
		/// <param name="opts">The cmd options.</param>
		/// <param name="host">The host.</param>
		protected static void RunWithOptions(CommandLineOptions opts, IHost host)
		{
			var copyService = ActivatorUtilities.GetServiceOrCreateInstance<ICopyService>(host.Services);
			var filterService = ActivatorUtilities.GetServiceOrCreateInstance<IFilterService>(host.Services);

			var filterFunction = filterService.GetFilterFunction(opts.Filter);

			copyService.Copy(opts.Src, opts.Dest, opts.IncludePattern, opts.ExcludePattern, filterFunction);
		}

		/// <summary>
		/// Handles the cmd options error.
		/// </summary>
		/// <param name="errs">The errors.</param>
		protected static void HandleOptionsError(IEnumerable<Error> errs)
		{
			//No specific handling for now
		}

		/// <summary>
		/// Creates the host builder.
		/// </summary>
		/// <param name="args">The cmd arguments.</param>
		/// <returns></returns>
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

	/// <summary>
	/// Class for commandLine options
	/// </summary>
	public class CommandLineOptions
	{
		[Option('s', "src", Required = true, HelpText = "Source folder.")]
		public string Src { get; set; }

		[Option('d', "dest", Required = true, HelpText = "Destination folder.")]
		public string Dest { get; set; }

		[Option('f', "filter", Required = true, HelpText = "Filter string. More info in readme.")]
		public string Filter { get; set; }

		[Option('i', "include", Required = false, HelpText = "Include pattern for filepath - regexp format. Files will be copied only if they are passing both filter and pattern.")]
		public string IncludePattern { get; set; }

		[Option('e', "exclude", Required = false, HelpText = "Exclude pattern for filepath - regexp format. Files will be excluded even they are passing filter.")]
		public string ExcludePattern { get; set; }
	}
}
