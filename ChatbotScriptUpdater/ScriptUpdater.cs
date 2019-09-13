﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChatbotScriptUpdater.Exceptions;
using Newtonsoft.Json;
using Semver;

namespace ChatbotScriptUpdater {
	public class ScriptUpdater {

		public class Configuration {
			public Configuration ( ) {
				Repository = new ConfigurationRepo ( );
			}
			[JsonProperty ( "path" )]
			public string Path { get; set; }
			[JsonProperty("script")]
			public string Script { get; set; }
			[JsonProperty ( "version" )]
			public string Version { get; set; }
			[JsonProperty ( "chatbot" )]
			public string Chatbot { get; set; }
			[JsonProperty("repository")]
			public ConfigurationRepo Repository { get; set; }
		}

		public class ConfigurationRepo {
			[JsonProperty("name")]
			public string Name { get; set; }
			[JsonProperty( "owner" )]
			public string Owner { get; set; }
		}

		public class UpdateStatusEventArgs {
			public Github.UpdateCheck Status { get; set; }
		}
		public event EventHandler BeginUpdateCheck;
		public event EventHandler<UpdateStatusEventArgs> EndUpdateCheck;
		public event EventHandler<ErrorEventArgs> Error;

		public ScriptUpdater (  ) {

		}

		public bool HasError { get; set; }

		public Configuration GetConfiguration ( ) {
			var path = Path.GetDirectoryName ( Assembly.GetExecutingAssembly ( ).Location );
			var file = "chatbot.json";
			var fullPath = Path.Combine ( path, file );
			if ( File.Exists ( fullPath ) ) {
				using ( var fr = new StreamReader ( fullPath ) ) {
					using ( var jr = new JsonTextReader ( fr ) ) {
						var ser = new JsonSerializer ( );
						return ser.Deserialize<Configuration> ( jr );
					}
				}
			} else {
				HasError = true;
				Error?.Invoke ( this, new ErrorEventArgs ( new FileNotFoundException ( "Unable to locate required chatbot.json config file" ) ) );

				return new Configuration {
					Version = "0.0.0",
					Repository = null
				};
			}
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
					result = new Github.UpdateCheck ( ) {
						HasUpdate = userVersion < releaseVersion,
						LatestVersion = releaseVersion,
						UserVersion = userVersion,
						Asset = release.Assets.First ( )
					};
				} else {
					HasError = true;
					Error?.Invoke ( this, new ErrorEventArgs ( new UnableToLoadReleaseException ( ) ) );
					return result;
				}

				EndUpdateCheck?.Invoke ( this, new UpdateStatusEventArgs ( ) { Status = result } );
				return result;
			} catch (Exception ex) {
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
			if (config == null || config.Repository == null) {
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