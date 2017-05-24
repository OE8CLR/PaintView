using System;
using CoreGraphics;

namespace XamarinPaint.Helpers
{
	public static class CGRectExtensions
	{
		public static CGRect CGRectNull()
		{
			return new CGRect (nfloat.PositiveInfinity, nfloat.PositiveInfinity, 0, 0);
		}
	}
}
