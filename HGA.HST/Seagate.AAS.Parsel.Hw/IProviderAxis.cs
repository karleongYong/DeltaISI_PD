using System;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for IProviderAxis.
	/// </summary>
    public interface IProviderAxis
    {
        bool IsHomed(IAxis axis);
        bool IsEnabled(IAxis axis);
        bool IsMoveDone(IAxis axis);
		
        double GetActualPosition(IAxis axis);
        double GetCommandPosition(IAxis axis);
        
        void Move(IAxis axis, double acceleration, double velocity, double position);
        void MoveRel(IAxis axis, double acceleration, double velocity, double positionRelative);
		void Home(IAxis axis);
        void Enable(IAxis axis, bool bEnable);		
        void Stop(IAxis axis);

        void StartWaitMoveDone(IAxis axis, uint timeOut);  // timeout in msec
    }
}
