using System.Threading.Tasks;

namespace Xamarin.Forms.Utils.Controls.Rotator
{
	public interface IRotatorController
	{
		Task MoveCenterToLeftAnimated( View view, double percentCompleted = 0 );
		Task MoveCenterToRightAnimated( View view, double percentCompleted = 0 );
		Task MoveLeftToCenterAnimated( View view, double percentCompleted = 0 );
		Task MoveRightToCenterAnimated( View view, double percentCompleted = 0 );
		void OnTapped( double XTapPercent, double YTapPercent );
		void StepCenterToLeft( View view, double percent );
		void StepCenterToRight( View view, double percent );
		void StepLeftToCenter( View view, double percent );
		void StepRightToCenter( View view, double percent );
	}
}