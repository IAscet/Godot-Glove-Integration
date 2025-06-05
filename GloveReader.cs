using Godot;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class GloveReader : Node2D
{
	private Process gloveProcess;
	private float? baselineY = null;
	private bool calibrated = false;

	[Export]
	public float Tolerance = 0.11f;

	[Export]
	public bool IsAboveBaselineState { get; private set; } = false;

	public override void _Ready()
	{
		StartGloveProcess();
		StartCalibrationAsync();
	}

	private async void StartCalibrationAsync()
	{
		await Task.Delay(3000);
		if (baselineY.HasValue)
			calibrated = true;
	}

	private void StartGloveProcess()
	{
		gloveProcess = new Process();
		gloveProcess.StartInfo.FileName = @"Q:\Connector.exe";
		gloveProcess.StartInfo.UseShellExecute = false;
		gloveProcess.StartInfo.RedirectStandardOutput = true;
		gloveProcess.StartInfo.RedirectStandardError = true;
		gloveProcess.StartInfo.CreateNoWindow = true;

		gloveProcess.OutputDataReceived += (sender, args) =>
		{
			if (!string.IsNullOrEmpty(args.Data))
			{
				try
				{
					var data = JsonSerializer.Deserialize<GloveData>(args.Data);
					if (data?.type == "dataframe" && data.imuValues?.Count > 0)
					{
						var imu = data.imuValues[0];
						float imuY = imu.y;

						if (!calibrated && !baselineY.HasValue)
							baselineY = imuY;

						if (calibrated && baselineY.HasValue)
						{
							float difference = imuY - baselineY.Value;
							IsAboveBaselineState = difference > Tolerance;
							GD.Print(IsAboveBaselineState ? "Jumping" : "On_Ground");
						}
					}
				}
				catch { }
			}
		};

		gloveProcess.ErrorDataReceived += (sender, args) =>
		{
			if (!string.IsNullOrEmpty(args.Data))
				GD.PrintErr("Error: " + args.Data);
		};

		gloveProcess.Start();
		gloveProcess.BeginOutputReadLine();
		gloveProcess.BeginErrorReadLine();
	}

	public override void _ExitTree()
	{
		if (gloveProcess != null && !gloveProcess.HasExited)
		{
			gloveProcess.Kill();
		}
	}
}

public class IMUValue
{
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float w { get; set; }
}

public class GloveData
{
	public string deviceId { get; set; }
	public string type { get; set; }
	public List<int> forceValues { get; set; }
	public List<IMUValue> imuValues { get; set; }
	public List<int> imuStates { get; set; }
	public List<int> vibrationStates { get; set; }
}
