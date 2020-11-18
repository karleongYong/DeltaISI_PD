using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XyratexOSC.IO.Devices;

namespace XyratexOSC.IO
{
    public static class DIOBitExtensions
    {
        /// <summary>
        /// Sets the digital output values for the specified DO bits.
        /// </summary>
        /// <param name="bits">The DO bits.</param>
        /// <param name="values">The values.</param>
        public static void Set(this IEnumerable<DOBit> bits, params bool[] values)
        {
            bits.SetValues(values.Select(b => b ? 1 : 0).ToArray());
        }

        /// <summary>
        /// Sets the digital output values for the specified DO bits.
        /// </summary>
        /// <param name="bits">The DO bits.</param>
        /// <param name="values">The values.</param>
        public static void SetValues(this IEnumerable<DOBit> bits, params int[] values)
        {
            if (bits.Count() != values.Length)
                throw new ArgumentException("Mismatch between the DO bits to set and the number of values specified.");

            var devGroups = bits.GroupBy(b => b.Device);

            foreach (var devGroup in devGroups)
            {
                IIODevice dev = (IIODevice)devGroup.Key;
                List<DOBit> orderedBits = devGroup.OrderBy(b => b.Channel).ToList();
                int[] channels = orderedBits.Select(b => b.Channel).ToArray();

                dev.SetDOBits(channels, values);
            }
        }

        /// <summary>
        /// Sets the digital input values for the specified DI bits.
        /// </summary>
        /// <param name="bits">The DI bits.</param>
        /// <param name="values">The values.</param>
        public static void Set(this IEnumerable<DIBit> bits, params bool[] values)
        {
            bits.SetValues(values.Select(b => b ? 1 : 0).ToArray());
        }

        /// <summary>
        /// Sets the digital input values for the specified DI bits.
        /// </summary>
        /// <param name="bits">The DI bits.</param>
        /// <param name="values">The values.</param>
        public static void SetValues(this IEnumerable<DIBit> bits, params int[] values)
        {
            if (bits.Count() != values.Length)
                throw new ArgumentException("Mismatch between the DI bits to set and the number of values specified.");

            var devGroups = bits.GroupBy(b => b.Device);

            foreach (var devGroup in devGroups)
            {
                IIODevice dev = (IIODevice)devGroup.Key;
                List<DIBit> orderedBits = devGroup.OrderBy(b => b.Channel).ToList();
                int[] channels = orderedBits.Select(b => b.Channel).ToArray();

                dev.SetDIBits(channels, values);
            }
        }

        /// <summary>
        /// Gets the digital input values for the specified DI bits.
        /// </summary>
        /// <param name="bits">The DI bits.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<bool> Get(this IEnumerable<DIBit> bits)
        {
            return bits.GetValues().Select(v => (v == 0) ? false : true);
        }

        /// <summary>
        /// Gets the digital input values for the specified DI bits.
        /// </summary>
        /// <param name="bits">The DI bits.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<int> GetValues(this IEnumerable<DIBit> bits)
        {
            var devGroups = bits.GroupBy(b => b.Device);
            
            foreach (var devGroup in devGroups)
            {
                IIODevice dev = (IIODevice)devGroup.Key;
                List<DIBit> orderedBits = devGroup.OrderBy(b => b.Channel).ToList();

                int[] values = dev.GetDIBits(orderedBits.Select(b => b.Channel).ToArray());

                DateTime readTime = DateTime.Now;

                for (int i = 0; i < values.Length; i++)
                    orderedBits[i].Update(readTime, values[i]);
            }

            return bits.Select(b => b.GetLast());
        }

        /// <summary>
        /// Gets the digital output values for the specified DO bits.
        /// </summary>
        /// <param name="bits">The DO bits.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<bool> Get(this IEnumerable<DOBit> bits)
        {
            return bits.GetValues().Select(v => (v == 0) ? false : true);
        }

        /// <summary>
        /// Gets the digital output values for the specified DO bits.
        /// </summary>
        /// <param name="bits">The DO bits.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<int> GetValues(this IEnumerable<DOBit> bits)
        {
            var devGroups = bits.GroupBy(b => b.Device);

            foreach (var devGroup in devGroups)
            {
                IIODevice dev = (IIODevice)devGroup.Key;
                List<DOBit> orderedBits = devGroup.OrderBy(b => b.Channel).ToList();

                int[] values = dev.GetDOBits(orderedBits.Select(b => b.Channel).ToArray());

                DateTime readTime = DateTime.Now;

                for (int i = 0; i < values.Length; i++)
                    orderedBits[i].Update(readTime, values[i]);
            }

            return bits.Select(b => b.GetLast());
        }

        /// <summary>
        /// Gets the digital input values for the specified <see cref="DIBit"/>s. A read from the controller will only be performed if no previous
        /// read was performed within the specified timeout.
        /// </summary>
        /// <param name="bits">The DI bits.</param>
        /// <param name="minFresh">The minimum amount of time before data is considered stale and a re-read is required.</param>
        /// <returns>
        /// The values.
        /// </returns>
        public static IEnumerable<int> GetValues(this IEnumerable<DIBit> bits, TimeSpan minFresh)
        {
            var devGroups = bits.GroupBy(b => b.Device);

            foreach (var devGroup in devGroups)
            {
                IIODevice dev = (IIODevice)devGroup.Key;
                List<DIBit> orderedBits = devGroup.OrderBy(b => b.Channel).ToList();

                bool allFresh = true;
                foreach (DIBit bit in orderedBits)
                {
                    if (DateTime.Now - bit.LastRead > minFresh)
                    {
                        allFresh = false;
                        break;
                    }
                }

                if (!allFresh)
                {
                    int[] values = dev.GetDIBits(orderedBits.Select(b => b.Channel).ToArray());

                    DateTime readTime = DateTime.Now;

                    for (int i = 0; i < values.Length; i++)
                        orderedBits[i].Update(readTime, values[i]);
                }
            }

            return bits.Select(b => b.GetLast());
        }


        /// <summary>
        /// Gets the digital output values for the specified <see cref="DOBit"/>s. A read from the controller will only be performed if no previous
        /// read was performed within the specified timeout.
        /// </summary>
        /// <param name="bits">The DO bits.</param>
        /// <param name="minFresh">The minimum amount of time before data is considered stale and a re-read is required.</param>
        /// <returns>
        /// The values.
        /// </returns>
        public static IEnumerable<int> GetValues(this IEnumerable<DOBit> bits, TimeSpan minFresh)
        {
            var devGroups = bits.GroupBy(b => b.Device);

            foreach (var devGroup in devGroups)
            {
                IIODevice dev = (IIODevice)devGroup.Key;
                List<DOBit> orderedBits = devGroup.OrderBy(b => b.Channel).ToList();

                bool allFresh = true;
                foreach (DOBit bit in orderedBits)
                {
                    if (DateTime.Now - bit.LastRead > minFresh)
                    {
                        allFresh = false;
                        break;
                    }
                }

                if (!allFresh)
                {
                    int[] values = dev.GetDOBits(orderedBits.Select(b => b.Channel).ToArray());

                    DateTime readTime = DateTime.Now;

                    for (int i = 0; i < values.Length; i++)
                        orderedBits[i].Update(readTime, values[i]);
                }
            }

            return bits.Select(b => b.GetLast());
        }
    }
}
