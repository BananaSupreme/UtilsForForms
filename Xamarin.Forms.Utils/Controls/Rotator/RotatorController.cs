using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Utils.Controls.Rotator
{
	public class RotatorController : IRotatorController
	{
		const int ANIMATION_TIME = 500;
		const double TAP_THRESHOLD = 0.1;
		const double MAX_ROTATION = 30;

		public void StepRightToCenter( View view, double percent )
		{
			double invPercent = 1 - percent;
			view.AnchorX = 1;
			view.RotationY = MAX_ROTATION * Easing.SinOut.Ease( invPercent );
			view.ScaleX = Easing.SinIn.Ease( percent );
			view.Opacity = Easing.SinIn.Ease( percent );
		}

		public void StepCenterToLeft( View view, double percent )
		{
			double invPercent = 1 - percent;
			view.AnchorX = 0;
			view.RotationY = MAX_ROTATION * Easing.SinIn.Ease( percent );
			view.ScaleX = Easing.SinOut.Ease( invPercent );
			view.Opacity = Easing.SinOut.Ease( invPercent );
		}

		public void StepLeftToCenter( View view, double percent )
		{
			double invPercent = 1 - percent;
			view.AnchorX = 0;
			view.RotationY = -MAX_ROTATION * Easing.SinIn.Ease( invPercent );
			view.ScaleX = Easing.SinOut.Ease( percent );
			view.Opacity = Easing.SinOut.Ease( percent );
		}

		public void StepCenterToRight( View view, double percent )
		{
			double invPercent = 1 - percent;
			view.AnchorX = 1;
			view.RotationY = -MAX_ROTATION * Easing.SinOut.Ease( percent );
			view.ScaleX = Easing.SinIn.Ease( invPercent );
			view.Opacity = Easing.SinIn.Ease( invPercent );
		}

		public async Task MoveCenterToRightAnimated( View view, double percentCompleted = 0 )
		{
			view.AnchorX = 1;
			await MoveAwayFromCenter( view, percentCompleted );
		}

		public async Task MoveRightToCenterAnimated( View view, double percentCompleted = 0 )
		{
			view.AnchorX = 1;
			await MoveToCenter( view, percentCompleted );
		}

		public async Task MoveCenterToLeftAnimated( View view, double percentCompleted = 0 )
		{
			view.AnchorX = 0;
			await MoveAwayFromCenter( view, percentCompleted );
		}

		public async Task MoveLeftToCenterAnimated( View view, double percentCompleted = 0 )
		{
			view.AnchorX = 0;
			await MoveToCenter( view, percentCompleted );
		}

		private static async Task MoveAwayFromCenter( View view, double percentCompleted )
		{
			uint adjustedDuration = (uint)( ( 1 - percentCompleted ) * ANIMATION_TIME );
			_ = await Task.WhenAll
				(
					view.RotateYTo( MAX_ROTATION, adjustedDuration, Easing.SinOut ),
					view.ScaleXTo( 0, adjustedDuration, Easing.SinIn ),
					view.FadeTo( 0, adjustedDuration, Easing.SinIn )
				);
		}

		private async Task MoveToCenter( View view, double percentCompleted = 0 )
		{
			view.RotationY = MAX_ROTATION * ( 1 - percentCompleted );

			uint adjustedDuration = (uint)( ( 1 - percentCompleted ) * ANIMATION_TIME );
			_ = await Task.WhenAll
				(
					view.RotateYTo( 0, adjustedDuration, Easing.SinIn ),
					view.ScaleXTo( 1, adjustedDuration, Easing.SinOut ),
					view.FadeTo( 1, adjustedDuration, Easing.SinOut )
				);
		}

		public void OnTapped( double XTapPercent, double YTapPercent )
		{
			int row = (int)Math.Floor( XTapPercent );
			int column = (int)Math.Floor( YTapPercent );
			double XTapPercentInRow = XTapPercent - row;
			double YTapPercentInColumn = YTapPercent - column;

			if ( XTapPercentInRow < TAP_THRESHOLD
				|| XTapPercentInRow > 1 - TAP_THRESHOLD
				|| YTapPercentInColumn < TAP_THRESHOLD
				|| YTapPercentInColumn > 1 - TAP_THRESHOLD )
				return;
		}
	}
}