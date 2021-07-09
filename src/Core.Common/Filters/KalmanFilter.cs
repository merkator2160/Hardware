using System;

namespace Core.Common.Filters
{
	public class KalmanFilter
	{
		private readonly Single _averageDeviation;
		private readonly Single _reactionSpeed;
		private Single _p = 1.0f;
		private Single _pc;
		private Single _g;
		private Single _xp;
		private Single _zp;
		private Single _xe;


		public KalmanFilter(Single averageDeviation = 0.25f, Single reactionSpeed = 0.05f)
		{
			_averageDeviation = averageDeviation;
			_reactionSpeed = reactionSpeed;
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public Single Calculate(Single val)
		{
			_pc = _p + _reactionSpeed;
			_g = _pc / (_pc + _averageDeviation);
			_p = (1 - _g) * _pc;
			_xp = _xe;
			_zp = _xp;
			_xe = _g * (val - _zp) + _xp;

			return _xe;
		}
	}
}