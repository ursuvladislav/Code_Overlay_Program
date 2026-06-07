using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeOverlayRunner.Services
{
    public enum BuildMode
    {
        Raw,
        IncludesOnly,
        IncludesAndMain
    }

    public static class CompilerService
    {
        private const string CommonIncludes =
            @"#include <iostream>
            #include <vector>
            #include <string>
            #include <deque>
            #include <list>
            #include <algorithm>
            #include <queue>
            #include <stack>

            using namespace std;

            ";

        private static string FindVsDevCmd()
        {
            string vswhere = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe";

            if (!File.Exists(vswhere))
                throw new FileNotFoundException("vswhere.exe не найден");

            var psi = new ProcessStartInfo
            {
                FileName = vswhere,
                Arguments = "-latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi)!;
            string installPath = proc.StandardOutput.ReadToEnd().Trim();
            proc.WaitForExit();

            if (string.IsNullOrWhiteSpace(installPath))
                throw new Exception("Visual Studio / Build Tools с C++ не найдены");

            string vsDevCmd = Path.Combine(installPath, @"Common7\Tools\VsDevCmd.bat");

            if (!File.Exists(vsDevCmd))
                throw new FileNotFoundException("VsDevCmd.bat не найден по пути: " + vsDevCmd);

            return vsDevCmd;
        }

        public static string BuildFinalCode(string clipboardCode, BuildMode mode)
        {
            if (string.IsNullOrWhiteSpace(clipboardCode))
                return string.Empty;

            switch (mode)
            {
                case BuildMode.IncludesOnly:
                    return CommonIncludes + clipboardCode;

                case BuildMode.IncludesAndMain:
                    return BuildCodeWithMain(clipboardCode);

                default:
                    return clipboardCode;
            }
        }

        private static string BuildCodeWithMain(string clipboardCode)
        {
            if (ContainsMainFunction(clipboardCode))
                return CommonIncludes + clipboardCode;

            return CommonIncludes +
@"int main()
{
" + IndentCode(clipboardCode) + @"

    return 0;
}
";
        }

        private static bool ContainsMainFunction(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return code.Contains("int main(") ||
                   code.Contains("int main (") ||
                   code.Contains("signed main(") ||
                   code.Contains("signed main (") ||
                   code.Contains("void main(") ||
                   code.Contains("void main (");
        }

        private static string IndentCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return string.Empty;

            var lines = code.Replace("\r\n", "\n").Split('\n');
            return string.Join(Environment.NewLine, lines.Select(line => "    " + line));
        }

        public static async Task<string> CompileAndRunCppAsync(string clipboardCode, BuildMode mode)
        {
            string finalCode = BuildFinalCode(clipboardCode, mode);

            if (string.IsNullOrWhiteSpace(finalCode))
                return "Буфер обмена пуст.";

            string workDir = Path.Combine(Path.GetTempPath(), "CodeOverlayRunner", Guid.NewGuid().ToString());
            Directory.CreateDirectory(workDir);

            string cppFile = Path.Combine(workDir, "main.cpp");
            string exeFile = Path.Combine(workDir, "a.exe");

            await File.WriteAllTextAsync(cppFile, finalCode, Encoding.UTF8);

            string vsDevCmd = FindVsDevCmd();

            var compilePsi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{vsDevCmd}\" -arch=amd64 && clang++ \"{cppFile}\" -std=c++17 -O2 -o \"{exeFile}\"\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workDir
            };

            using var compileProc = Process.Start(compilePsi)!;
            string compileOut = await compileProc.StandardOutput.ReadToEndAsync();
            string compileErr = await compileProc.StandardError.ReadToEndAsync();
            await compileProc.WaitForExitAsync();

            if (compileProc.ExitCode != 0)
            {
                string compileMessage = !string.IsNullOrWhiteSpace(compileErr)
                    ? compileErr.Trim()
                    : compileOut.Trim();

                return "❌ Ошибка компиляции:\n" + compileMessage;
            }

            var runPsi = new ProcessStartInfo
            {
                FileName = exeFile,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workDir
            };

            using var runProc = Process.Start(runPsi)!;

            if (!runProc.WaitForExit(2000))
            {
                try { runProc.Kill(true); } catch { }
                return "⏱️ Превышено время выполнения (2 сек).";
            }

            string runOut = await runProc.StandardOutput.ReadToEndAsync();
            string runErr = await runProc.StandardError.ReadToEndAsync();

            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(runOut))
                sb.AppendLine(runOut.Trim());

            if (!string.IsNullOrWhiteSpace(runErr))
                sb.AppendLine("stderr:\n" + runErr.Trim());

            if (sb.Length == 0)
                sb.AppendLine("✔ Выполнено. Вывода нет.");

            return sb.ToString();
        }
    }
}