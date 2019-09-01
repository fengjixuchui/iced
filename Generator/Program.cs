/*
Copyright (C) 2018-2019 de4dot@gmail.com

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace Generator {
	enum Command {
		Decoder,
		Formatter,
		CpuidFeature,
	}

	sealed class CommandLineOptions {
		public readonly List<Command> Commands = new List<Command>();
		public string? IcedProjectDir = null;
	}

	static class Program {
		static int Main(string[] args) {
			try {
				if (!ParseCommandLine(args, out var options)) {
					Usage();
					return 1;
				}

				if (options.IcedProjectDir is null)
					options.IcedProjectDir = GetIcedProjectDir();

				foreach (var command in options.Commands) {
					switch (command) {
#if !NO_DECODER
					case Command.Decoder:
						new Decoder.DecoderTableGenerator(options.IcedProjectDir).Generate();
						break;
#endif

#if (!NO_GAS_FORMATTER || !NO_INTEL_FORMATTER || !NO_MASM_FORMATTER || !NO_NASM_FORMATTER) && !NO_FORMATTER
					case Command.Formatter:
						new Formatters.FormatterTableGenerator(options.IcedProjectDir).Generate();
						break;
#endif

#if !NO_INSTR_INFO
					case Command.CpuidFeature:
						new InstructionInfo.CpuidFeatureTableGenerator(options.IcedProjectDir).Generate();
						break;
#endif

					default:
						throw new InvalidOperationException();
					}
				}
				return 0;
			}
			catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				return 1;
			}
		}

		static string GetIcedProjectDir() {
			var dir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(typeof(Program).Assembly.Location)))));
			if (dir is null || !File.Exists(Path.Combine(dir, "Iced.sln")))
				throw new InvalidOperationException();
			dir = Path.Combine(dir, "Iced");
			if (!File.Exists(Path.Combine(dir, "Iced.csproj")))
				throw new InvalidOperationException();
			return dir;
		}

		static void Usage() {
			Console.WriteLine(@"Generator <command(s)>
command:
    --decoder           Generate decoder tables
    --formatter         Generate formatter tables
    --cpuidfeature      Generate cpuid features table
");
		}

		static bool ParseCommandLine(string[] args, out CommandLineOptions options) {
			options = new CommandLineOptions();
			foreach (var arg in args) {
				switch (arg) {
				case "-?":
				case "--help":
					return false;

				case "--decoder":
					if (!TryAddCommand(options, Command.Decoder))
						return false;
					break;

				case "--formatter":
					if (!TryAddCommand(options, Command.Formatter))
						return false;
					break;

				case "--cpuidfeature":
					if (!TryAddCommand(options, Command.CpuidFeature))
						return false;
					break;

				default:
					return false;
				}
			}
			return options.Commands.Count != 0;
		}

		static bool TryAddCommand(CommandLineOptions options, Command command) {
			if (options.Commands.Contains(command))
				return false;
			options.Commands.Add(command);
			return true;
		}
	}
}
