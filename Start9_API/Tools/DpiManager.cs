using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Start9.Api.Tools
{
	public static class DpiManager
	{
		public static float GetScalingFactor()
		{
			var g = Graphics.FromHwnd(IntPtr.Zero);
			return g.DpiY / 96;
		}

		public static int ConvertWpfUnitsToPixels(double wpfUnits) => (int) (wpfUnits * GetScalingFactor());

		public static int ConvertPixelsToWpfUnits(double pixels) => (int) (pixels / GetScalingFactor());
	}
}