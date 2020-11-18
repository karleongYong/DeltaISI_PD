using System;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for IProviderCamera.
	/// </summary>
	public interface IProviderCamera
	{
		double GetExposure(ICamera camera);
		double GetBrightness(ICamera camera);
		double GetContrast(ICamera camera);
		void SetExposure(ICamera camera, double exposure);
		void SetBrightness(ICamera camera, double brightness);
		void SetContrast(ICamera camera, double contrast);

		void RequestImage(ICamera camera, int timeOut);
		object GetLastImage(ICamera camera);
//		object WaitOnAcquisition(ICamera camera);
		void SaveLastImage(ICamera camera, string pathFileName);
		void AppendLastToImageDB(ICamera camera, string pathFileName);
		void AppendLastToImageDB(ICamera camera, string pathFileName, string comment);
		void SetStrobeParameters(ICamera camera, int outputNumber, double millisecPulseDuration, double millisecPulseDelay, bool activeHigh);
	}
}
