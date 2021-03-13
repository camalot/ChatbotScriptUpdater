using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatbotScriptUpdater.Exceptions;
using Newtonsoft.Json;
using Semver;

namespace ChatbotScriptUpdater {
	public class ApplicationUpdater {

		public class Configuration {
			[JsonProperty ( "path" )]
			public string Path { get; set; }
			[JsonProperty ( "folderName" )]
			public string FolderName { get; set; }
			[JsonProperty ( "version" )]
			public string Version { get; set; }
			[JsonProperty ( "name" )]
			public string Name { get; set; }
			public string ProcessName { get; set; } = "Streamlabs Chatbot";
			[JsonProperty ( "application" )]
			public string Application { get; set; }
			[JsonProperty ( "moveZipRoot" )]

			public bool MoveZipRoot { get; set; } = false;
			[JsonProperty("autoRestartAfterUpdate")]
			public bool AutoRestartAfterUpdate { get; set; } = false;
			[JsonProperty ( "requiresRestart" )]
			public bool RequiresRestart { get; set; } = false;
			[JsonProperty ( "website" )]
			public string Website { get; set; }
			[JsonProperty ( "repository" )]
			public ConfigurationRepo Repository { get; set; } = new ConfigurationRepo ( );
			[JsonProperty ( "kill" )]
			public List<string> Kill { get; set; } = new List<string> ( );
			[JsonProperty ( "execute" )]
			public ConfigurationExecute Execute { get; set; } = new ConfigurationExecute ( );

			public ConfigurationInterface Interface { get; set; } = new ConfigurationInterface ( );
		}

		public class ConfigurationInterface {
			[JsonProperty("closeButton")]
			public string CloseButton { get; set; } = "&Close";
			[JsonProperty ( "downloadButton" )]
			public string DownloadButton { get; set; } = "&Dowload && Update";
			[JsonProperty("repsitoryLink")]
			public string OpenRepositoryLink { get; set; } = "Open Repository";
		}

		public class ConfigurationExecute {

			[JsonProperty ( "before" )]
			public List<ConfigurationExecuteCommand> Before { get; set; } = new List<ConfigurationExecuteCommand> ( );
			[JsonProperty ( "after" )]
			public List<ConfigurationExecuteCommand> After { get; set; } = new List<ConfigurationExecuteCommand> ( );
		}

		public class ConfigurationExecuteCommand {
			[JsonProperty ( "command" )]
			public string Command { get; set; }
			[JsonProperty ( "arguments" )]
			public List<string> Arguments { get; set; } = new List<string> ( );
			[JsonProperty ( "workingDirectory" )]
			public string WorkingDirectory { get; set; }
			[JsonProperty ( "ignoreExitCode" )]
			public bool IgnoreExitCode { get; set; } = true;
			[JsonProperty ( "validExitCodes" )]
			public List<int> ValidExitCodes { get; set; } = new List<int> ( ) { 0 };

			public override string ToString ( ) {
				return $"{Command} {string.Join ( " ", Arguments )}";
			}
		}

		public class ConfigurationRepo {
			[JsonProperty ( "name" )]
			public string Name { get; set; }
			[JsonProperty ( "owner" )]
			public string Owner { get; set; }
			[JsonProperty ( "assetMatch" )]
			public string AssetMatch { get; set; } = null;
		}

		public class UpdateStatusEventArgs {
			public Github.UpdateCheck Status { get; set; }
		}
		public event EventHandler BeginUpdateCheck;
		public event EventHandler<UpdateStatusEventArgs> EndUpdateCheck;
		public event EventHandler<ErrorEventArgs> Error;

		public ApplicationUpdater ( ) {

		}

		public bool HasError { get; set; }

		public Configuration GetConfiguration ( string file = "update.manifest" ) {
			var path = Path.GetDirectoryName ( Assembly.GetExecutingAssembly ( ).Location );
			var fullPath = Path.Combine ( path, file );
			Console.WriteLine ( fullPath );
			if ( File.Exists ( fullPath ) ) {
				using ( var fr = new StreamReader ( fullPath ) ) {
					using ( var jr = new JsonTextReader ( fr ) ) {
						var ser = new JsonSerializer ( );
						return ser.Deserialize<Configuration> ( jr );
					}
				}
			} else {
				HasError = true;
				Error?.Invoke ( this, new ErrorEventArgs ( new FileNotFoundException ( "Unable to locate required manifest file" ) ) );

				return new Configuration {
					Version = "0.0.0",
					Repository = null
				};
			}
		}

		public void RunCommand(ConfigurationExecuteCommand command) {
			try {
				var processInfo = new System.Diagnostics.ProcessStartInfo( command.Command, MakeArgumentSafe ( command.Arguments) );
				processInfo.UseShellExecute = false;
				Console.WriteLine ( command.ToString ( ) );
				if(string.IsNullOrWhiteSpace(command.WorkingDirectory)) {
					throw new ArgumentException ( "Working Directory MUST Be Set." );
				}
				processInfo.WorkingDirectory = command.WorkingDirectory;
				var process = new System.Diagnostics.Process ( );
				process.StartInfo = processInfo;
				process.Start ( );
				process.WaitForExit ( );
				if(!command.IgnoreExitCode && command.ValidExitCodes.Count > 0 && !command.ValidExitCodes.Distinct().Contains(process.ExitCode)) {
					var pluraled = command.ValidExitCodes.Count == 1 ? "" : "s";
					throw new Exception ( $"Expected exit code{pluraled} ({string.Join ( ", ", command.ValidExitCodes.Distinct() )}) but got {process.ExitCode} for {command}" );
				}
			} catch (Exception ex) {
				if(!command.IgnoreExitCode) {
					throw ex;
				}
			}
		}

		private string MakeArgumentSafe(IEnumerable<string> args) {
			var sb = new StringBuilder ( );
			foreach ( var s in args ) {
				if(s.Contains(" ")) {
					sb.Append ( $"\"{s}\" " );
				} else {
					sb.Append ( $"{s} " );
				}

			}
			return sb.ToString ( ).Trim ( );
		}
		public async Task<Github.UpdateCheck> CheckUpdateStatus ( Configuration config ) {
			try {
				BeginUpdateCheck?.Invoke ( this, new EventArgs ( ) );
				var release = await GetLatestRelease ( config );
				var userVersion = SemVersion.Parse ( config.Version );
				
				var result = new Github.UpdateCheck ( ) {
					HasUpdate = false,
					UserVersion = userVersion,
					LatestVersion = SemVersion.Parse ( "0.0.0" )
				};
				if ( release != null && release.Assets?.Count ( ) > 0 ) {
					var releaseVersion = SemVersion.Parse ( release.TagName );

					var asset = release.Assets.First ( );
					if (!string.IsNullOrWhiteSpace(config.Repository.AssetMatch)) {
						var regex = new Regex ( config.Repository.AssetMatch, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
						asset = release.Assets.Where ( a => regex.IsMatch ( a.Name ) ).FirstOrDefault ( );
					}
					if ( asset == null ) {
						// no asset matches
						HasError = true;
						Error?.Invoke ( this, new ErrorEventArgs ( new NoMatchingAssetsException ( ) ) );
						return result;
					} else {
						result = new Github.UpdateCheck ( ) {
							HasUpdate = userVersion < releaseVersion,
							LatestVersion = releaseVersion,
							UserVersion = userVersion,
							Asset = asset
						};
					}
				} else {
					HasError = true;
					Error?.Invoke ( this, new ErrorEventArgs ( new UnableToLoadReleaseException ( ) ) );
					return result;
				}

				EndUpdateCheck?.Invoke ( this, new UpdateStatusEventArgs ( ) { Status = result } );
				return result;
			} catch ( Exception ex ) {
				HasError = true;
				Error?.Invoke ( this, new ErrorEventArgs ( ex ) );
				return new Github.UpdateCheck ( ) {
					HasUpdate = false,
					UserVersion = SemVersion.Parse ( "0.0.0" ),
					LatestVersion = SemVersion.Parse ( "0.0.0" )
				};
			}
		}



		private async Task<Github.Release> GetLatestRelease ( Configuration config ) {
			if ( config == null || config.Repository == null ) {
				throw new NoConfigurationException ( );
			}
			var api = $"https://api.github.com/repos/{config.Repository.Owner}/{config.Repository.Name}/releases/latest";

			using ( var httpClient = new HttpClient ( ) ) {
				httpClient.DefaultRequestHeaders.Add ( "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36" );
				var json = await httpClient.GetStringAsync ( api );
				using ( var sr = new StringReader ( json ) ) {
					using ( var jr = new JsonTextReader ( sr ) ) {
						var ser = new JsonSerializer ( ) {
							DateFormatHandling = DateFormatHandling.IsoDateFormat
						};
						return ser.Deserialize<Github.Release> ( jr );
					}
				}
			}
		}


	}
}
