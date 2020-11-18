using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.Hardware;

namespace XyratexOSC.IO.Devices
{
    /// <summary>
    /// Represents a controller device that has any digital or analog input/output channels.
    /// </summary>
    public interface IIODevice : IDevice
    {
        /// <summary>
        /// Registers the specified <see cref="DIBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="DIBit"/>.</param>
        void RegisterDIBit(DIBit bit);

        /// <summary>
        /// Registers the specified <see cref="DOBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="DOBit"/>.</param>
        void RegisterDOBit(DOBit bit);

        /// <summary>
        /// Registers the specified <see cref="AIBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="AIBit"/>.</param>
        void RegisterAIBit(AIBit bit);

        /// <summary>
        /// Registers the specified <see cref="AOBit"/> with this device.
        /// </summary>
        /// <param name="bit">The <see cref="AOBit"/>.</param>
        void RegisterAOBit(AOBit bit);

        /// <summary>
        /// Registers the specified <see cref="DIWord"/> with this device.
        /// </summary>
        /// <param name="word">The <see cref="DIWord"/>.</param>
        void RegisterDIWord(DIWord word);

        /// <summary>
        /// Registers the specified <see cref="DOWord"/> with this device.
        /// </summary>
        /// <param name="word">The <see cref="DOWord"/>.</param>
        void RegisterDOWord(DOWord word);

        /// <summary>
        /// Gets the number of bits that are represented in a single word.
        /// </summary>
        /// <returns>The number of bits.</returns>
        int GetDIOWordLength();

        /// <summary>
        /// Gets the current value of the digital input bit (0 or 1) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>The bit value.</returns>
        int GetDIBit(int channel);

        /// <summary>
        /// Sets the current value of the digital input bit (0 or 1) at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The DI value.</param>
        void SetDIBit(int channel, int value);

        /// <summary>
        /// Gets all of the values of the specified digital input bits. 
        /// This should utilize any time-saving techniques with multiple channel reads depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>An array of digital input values.</returns>
        int[] GetDIBits(int[] channels);

        /// <summary>
        /// Sets all of the values of the specified digital input bits. 
        /// This should utilize any time-saving techniques with multiple channel writes depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="values">The bit values.</param>
        void SetDIBits(int[] channels, int[] values);

        /// <summary>
        /// Gets the current value of the digital input word (see <see cref="DIWord"/>) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>The word value.</returns>
        ulong GetDIWord(int channel);

        /// <summary>
        /// Sets the current value of the digital input word (see <see cref="DIWord" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The word value.</param>
        void SetDIWord(int channel, ulong value);

        /// <summary>
        /// Gets the current value of the digital output bit (see <see cref="DOBit"/>) at the specified channel address.
        /// </summary>
        /// <param name="channel">The bit channel.</param>
        /// <returns>The bit value.</returns>
        int GetDOBit(int channel);

        /// <summary>
        /// Sets the current value of the digital output bit (0 or 1) at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The DO value.</param>
        void SetDOBit(int channel, int value);

        /// <summary>
        /// Gets all of the values of the specified digital output bits. 
        /// This should utilize any time-saving techniques with multiple channel reads depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>An array of digital output values.</returns>
        int[] GetDOBits(int[] channels);

        /// <summary>
        /// Sets all of the values of the specified digital output bits. 
        /// This should utilize any time-saving techniques with multiple channel writes depending on the controller and word configurations.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="values">The bit values.</param>
        void SetDOBits(int[] channels, int[] values);

        /// <summary>
        /// Gets the current value of the digital output word (see <see cref="DOWord"/>) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>The word value.</returns>
        ulong GetDOWord(int channel);

        /// <summary>
        /// Sets the current value of the digital output word (see <see cref="DOWord" />) at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The word value.</param>
        void SetDOWord(int channel, ulong value);

        /// <summary>
        /// Gets the current value of the analog input bit at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>The analog value.</returns>
        double GetAIBit(int channel);

        /// <summary>
        /// Sets the current value of the analog input bit at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The analog value.</param>
        void SetAIBit(int channel, double value);

        /// <summary>
        /// Gets the current value of the analog output bit at the specified channel address.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <returns>The analog value.</returns>
        double GetAOBit(int channel);

        /// <summary>
        /// Sets the current value of the analog output bit at the specified channel address to the specified value.
        /// </summary>
        /// <param name="channel">The channel address.</param>
        /// <param name="value">The analog value.</param>
        void SetAOBit(int channel, double value);
    }
}
