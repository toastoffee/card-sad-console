using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public static class Log {
	public static Stack<string> logs = new();

	public static void Push(string log) {
		logs.Push(log);
	}

	public static void LogError(string error) {
		logs.Push($"[ERROR] {error}");
	}

	public static void Render() {
		foreach (var log in logs) {
			//Console.WriteLine(log);
		}
	}
}
