using Godot;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public partial class glove : Node
{
	private Process gloveProcess;
	private bool calibrated = false;

	[Export]
	public bool IsFist { get; private set; } = false;
	public bool openCalibrated { get; private set; } = false;


	private List<List<IMUValue>> idleFrames = new List<List<IMUValue>>();
	private List<List<IMUValue>> fistFrames = new List<List<IMUValue>>();
	private List<IMUValue> lastFrame = null;

	public override void _Ready()
	{
		StartGloveProcess();
		StartCalibrationAsync();
	}

	private async void StartCalibrationAsync()
	{
		GD.Print("Hold idle for 5 seconds...");
		idleFrames = await CollectFramesAsync(5);
		openCalibrated = true;

		GD.Print("Now make a fist for 5 seconds...");
		fistFrames = await CollectFramesAsync(5);

		calibrated = true;
		GD.Print($"Calibration complete. Idle frames: {idleFrames.Count}, Fist frames: {fistFrames.Count}");
	}

	private async Task<List<List<IMUValue>>> CollectFramesAsync(float seconds)
	{
		List<List<IMUValue>> frames = new List<List<IMUValue>>();
		float interval = 0.05f;
		int count = (int)(seconds / interval);

		for (int i = 0; i < count; i++)
		{
			if (lastFrame != null)
			{
				var copyFrame = lastFrame.Select(imu => new IMUValue
				{
					x = imu.x,
					y = imu.y,
					z = imu.z,
					w = imu.w
				}).ToList();

				frames.Add(copyFrame);
			}
			await Task.Delay((int)(interval * 1000));
		}

		return frames;
	}

	private void StartGloveProcess()
	{
		gloveProcess = new Process();
		gloveProcess.StartInfo.FileName = @"Connector_v3.1.2-beta.1.exe";
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
						lastFrame = data.imuValues.Select(imu => new IMUValue
						{
							x = imu.x,
							y = imu.y,
							z = imu.z,
							w = imu.w
						}).ToList();

						if (calibrated)
						{
							float distIdle = DistanceToCalibration(lastFrame, idleFrames);
							float distFist = DistanceToCalibration(lastFrame, fistFrames);

							IsFist = distFist < distIdle;
							GD.Print(IsFist ? "Fist" : "Open Hand");
						}
					}
				}
				catch (Exception ex)
				{
					GD.PrintErr("JSON parse error: " + ex.Message);
				}
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

	private float DistanceToCalibration(List<IMUValue> currentFrame, List<List<IMUValue>> calibrationFrames)
	{
		if (calibrationFrames.Count == 0) return float.MaxValue;

		int imuCount = currentFrame.Count;
		var avgFrame = new List<IMUValue>();

		for (int i = 0; i < imuCount; i++)
		{
			avgFrame.Add(new IMUValue
			{
				x = calibrationFrames.Average(f => f[i].x),
				y = calibrationFrames.Average(f => f[i].y),
				z = calibrationFrames.Average(f => f[i].z),
				w = calibrationFrames.Average(f => f[i].w)
			});
		}

		float sum = 0f;
		for (int i = 0; i < imuCount; i++)
		{
			var c = currentFrame[i];
			var a = avgFrame[i];

			sum += (c.x - a.x) * (c.x - a.x);
			sum += (c.y - a.y) * (c.y - a.y);
			sum += (c.z - a.z) * (c.z - a.z);
			sum += (c.w - a.w) * (c.w - a.w);
		}

		return (float)Math.Sqrt(sum);
	}

	public override void _ExitTree()
	{
		if (gloveProcess != null && !gloveProcess.HasExited)
			gloveProcess.Kill();
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
