using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XyratexOSC.Hardware;
using XyratexOSC.IO.Devices;
using XyratexOSC.Logging;
using XyratexOSC.Settings;

namespace XyratexOSC.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class IOComponent : HardwareComponent<IIODevice, IIOPart>
    {
        private enum IOPartType
        {
            DIBit,
            DOBit,
            DIWord,
            DOWord,
            AIBit,
            AOBit
        }

        private int _INBIT_DOOR_STAT = -1;
        private int _INBIT_ESTOP_STAT = -1;
        private int _INBIT_AMP_ENABLED = -1;
        private int _OUTBIT_AMP_ENABLE = -1;
        private int _OUTBIT_SAFETY_OVERRIDE = -1;
        private int _OUTBIT_AMP_LATCH = -1;
        private int _OUTBIT_LIGHT_RED = -1;
        private int _OUTBIT_LIGHT_YELLOW = -1;
        private int _OUTBIT_LIGHT_GREEN = -1;
        private int _OUTBIT_LIGHT_BLUE = -1;

        #region Singleton Implementation

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<IOComponent> _instance
             = new Lazy<IOComponent>(() => new IOComponent());

        /// <summary>
        /// Gets the IOComponent instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static IOComponent Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IOComponent"/> class.
        /// </summary>
        private IOComponent()
        {
            ConfigFilePath = "IO.config";
            PartsNodeNames = new string[] {"DInBits", "DOutBits", "DInWords", "DOutWords", "AInBits", "AOutBits" };
            DefaultDeviceName = "PmacDevice";
        }

        #endregion Singleton Implementation

        #region IDisposable Members

        /// <summary>
        /// Finalizes an instance of the <see cref="IOComponent"/> class.
        /// </summary>
        ~IOComponent()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool managed)
        {
            if (managed)
            {
                for (int i = 0; i < _devMgr.Devices.Count; i++)
                {
                    IIODevice device = _devMgr.Devices[i] as IIODevice;

                    try
                    {
                        if (device == null)
                            continue;

                        if (device.Initialized)
                            device.Uninitialize();

                        if (device.Connected)
                            device.Close();

                        device.Dispose();
                        device = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this, "Failed to dispose of IO device {0}. {1}", device.Name, ex);
                    }
                }

                Log.Trace().Flush();
            }
            else
            {
                //TODO
            }
        }

        #endregion IDisposable Members

        /// <summary>
        /// The component name, which is used for logging and can be used for identifying a component.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get 
            {
                return "IO";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IOComponent"/> is simulated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulated; otherwise, <c>false</c>.
        /// </value>
        public bool Simulated
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a <see cref="NamedList{T}"/> of digital input bits.
        /// </summary>
        /// <value>
        /// The DI bits.
        /// </value>
        public INamedList<DIBit> DIBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a <see cref="NamedList{T}"/> of digital output bits.
        /// </summary>
        /// <value>
        /// The DO bits.
        /// </value>
        public INamedList<DOBit> DOBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a <see cref="NamedList{T}"/> of digital input words.
        /// </summary>
        /// <value>
        /// The DI words.
        /// </value>
        public INamedList<DIWord> DIWords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a <see cref="NamedList{T}"/> of digital output words.
        /// </summary>
        /// <value>
        /// The DO words.
        /// </value>
        public INamedList<DOWord> DOWords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a <see cref="NamedList{T}"/> of analog input bits.
        /// </summary>
        /// <value>
        /// The AI bits.
        /// </value>
        public INamedList<AIBit> AIBits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a <see cref="NamedList{T}"/> of analog output bits.
        /// </summary>
        /// <value>
        /// The AO bits.
        /// </value>
        public INamedList<AOBit> AOBits
        {
            get;
            private set;
        }

        /// <summary>
        /// The index for the door status input-bit. Use this bit to monitor the machine door.
        /// </summary>
        /// <value>The door status input bit.</value>
        public int INBIT_DOOR_STAT
        {
            get { return _INBIT_DOOR_STAT; }
        }

        /// <summary>
        /// The index for the estop-pressed status input-bit. Use this bit to monitor the estop.
        /// </summary>
        /// <value>The estop status input bit.</value>
        public int INBIT_ESTOP_STAT
        {
            get { return _INBIT_ESTOP_STAT; }
        }

        /// <summary>
        /// The index for the amplifiers enabled input-bit. Use this bit to monitor if the amps are on.
        /// </summary>
        /// <value>The amp enabled input bit.</value>
        public int INBIT_AMP_ENABLED
        {
            get { return _INBIT_AMP_ENABLED; }
        }

        /// <summary>
        /// The index for the amplifiers enabled output-bit.
        /// </summary>
        /// <value>The amp enabled output bit.</value>
        public int OUTBIT_AMP_ENABLE
        {
            get { return _OUTBIT_AMP_ENABLE; }
        }

        /// <summary>
        ///The index for the safety override output-bit.
        /// </summary>
        /// <value>The safety override output bit.</value>
        internal int OUTBIT_SAFETY_OVERRIDE
        {
            get { return _OUTBIT_SAFETY_OVERRIDE; }
        }

        /// <summary>
        /// The index for the amplifiers latch output-bit.
        /// </summary>
        /// <value>The amp latch output bit.</value>
        public int OUTBIT_AMP_LATCH
        {
            get { return _OUTBIT_AMP_LATCH; }
        }

        /// <summary>Output Bit: Red LED Indicator</summary>
        public int OUTBIT_LIGHT_RED
        {
            get { return _OUTBIT_LIGHT_RED; }
        }

        /// <summary>Output Bit: Yellow LED Indicator</summary>
        public int OUTBIT_LIGHT_YELLOW
        {
            get { return _OUTBIT_LIGHT_YELLOW; }
        }

        /// <summary>Output Bit: Green LED Indicator</summary>
        public int OUTBIT_LIGHT_GREEN
        {
            get { return _OUTBIT_LIGHT_GREEN; }
        }

        /// <summary>Output Bit: Blue LED Indicator</summary>
        public int OUTBIT_LIGHT_BLUE
        {
            get { return _OUTBIT_LIGHT_BLUE; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Initializes the IOComponent using the old config files.
        /// </summary>
        public override IList<string> TrySpecificInit(SettingsNode settings, out IList<IIOPart> parts)
        {
            parts = new List<IIOPart>();
            return new List<string>();
        }

        /// <summary>
        /// Gets the type of a simulated IO device type.
        /// </summary>
        /// <returns></returns>
        protected override Type GetSimDeviceType()
        {
            return typeof(SimIO);
        }

        /// <summary>
        /// Creates IO parts from the specified settings.
        /// </summary>
        /// <param name="settings">The IO settings node.</param>
        /// <param name="parts">The constructed IO parts.</param>
        /// <returns>A list of parts forced to simulated.</returns>
        public override IList<string> CreateParts(SettingsNode settings, out IList<IIOPart> parts)
        {
            parts = new List<IIOPart>();

            SettingsNode ioNode = settings["IO"];
            if (ioNode == null)
                return new List<string>();

            List<string> forcedParts = new List<string>();

            DIBits = new NamedList<DIBit>();
            DOBits = new NamedList<DOBit>();
            AIBits = new NamedList<AIBit>();
            AOBits = new NamedList<AOBit>();
            DIWords = new NamedList<DIWord>();
            DOWords = new NamedList<DOWord>();

            // Create bits

            foreach (SettingsNode node in ioNode.Nodes)
            {
                if (String.Equals(node.Name, "DInBits", StringComparison.CurrentCultureIgnoreCase) ||
                    String.Equals(node.Name, "InBits", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        IList<IIOPart> inBits;
                        IList<string> forced = CreateIOParts(node.Nodes.Select(n => n.Name), IOPartType.DIBit, out inBits);

                        foreach (IIOPart bit in inBits)
                            parts.Add(bit);

                        forcedParts.AddRange(forced);
                        DIBits = inBits.Select(b => (DIBit)b).ToNamedList();
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(IOComponent.Instance.Name).Error("DI Bit Config", "Failed to configure DI bit '{0}'. {1}", node.Nodes, ex);
                    }
                }
                else if (String.Equals(node.Name, "DOutBits", StringComparison.CurrentCultureIgnoreCase) ||
                            String.Equals(node.Name, "OutBits", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        IList<IIOPart> outBits;
                        IList<string> forced = CreateIOParts(node.Nodes.Select(n => n.Name), IOPartType.DOBit, out outBits);

                        foreach (IIOPart bit in outBits)
                            parts.Add(bit);

                        forcedParts.AddRange(forced);
                        DOBits = outBits.Select(b => (DOBit)b).ToNamedList();
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(IOComponent.Instance.Name).Error("DO Bit Config", "Failed to configure DO bit '{0}'. {1}", node.Nodes, ex);
                    }
                }
                else if (String.Equals(node.Name, "AInBits", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                    IList<IIOPart> inBits;
                    IList<string> forced = CreateIOParts(node.Nodes.Select(n => n.Name), IOPartType.AIBit, out inBits);

                    foreach (IIOPart bit in inBits)
                        parts.Add(bit);

                    forcedParts.AddRange(forced);
                    AIBits = inBits.Select(b => (AIBit)b).ToNamedList();
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(IOComponent.Instance.Name).Error("AI Bit Config", "Failed to configure AI bit '{0}'. {1}", node.Nodes, ex);
                    }
                }
                else if (String.Equals(node.Name, "AOutBits", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        IList<IIOPart> outBits;
                        IList<string> forced = CreateIOParts(node.Nodes.Select(n => n.Name), IOPartType.AOBit, out outBits);

                        foreach (IIOPart bit in outBits)
                            parts.Add(bit);

                        forcedParts.AddRange(forced);
                        AOBits = outBits.Select(b => (AOBit)b).ToNamedList();
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(IOComponent.Instance.Name).Error("AO Bit Config", "Failed to configure AO bit '{0}'. {1}", node.Nodes, ex);
                    }
                }
            }

            foreach (SettingsNode node in ioNode.Nodes)
            {
                if (String.Equals(node.Name, "DInWords", StringComparison.CurrentCultureIgnoreCase) ||
                    String.Equals(node.Name, "InWords", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        IList<IIOPart> inWords;
                        IList<string> forced = CreateIOParts(node.Nodes.Select(n => n.Name), IOPartType.DIWord, out inWords);

                        foreach (IIOPart word in inWords)
                            parts.Add(word);

                        forcedParts.AddRange(forced);
                        DIWords = inWords.Select(b => (DIWord)b).ToNamedList();
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(IOComponent.Instance.Name).Error("DI Word Config", "Failed to configure DI word '{0}'. {1}", node.Nodes, ex);
                    }
                }
                else if (String.Equals(node.Name, "DOutWords", StringComparison.CurrentCultureIgnoreCase) ||
                         String.Equals(node.Name, "OutWords", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        IList<IIOPart> outWords;
                        IList<string> forced = CreateIOParts(node.Nodes.Select(n => n.Name), IOPartType.DOWord, out outWords);

                        foreach (IIOPart word in outWords)
                            parts.Add(word);

                        forcedParts.AddRange(forced);
                        DOWords = outWords.Select(b => (DOWord)b).ToNamedList();
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(IOComponent.Instance.Name).Error("DO Word Config", "Failed to configure DO word '{0}'. {1}", node.Nodes, ex);
                    }
                }
            }

            return forcedParts;
        }

        /// <summary>
        /// Creates the IO parts from comma separated strings.
        /// Uses the format "Name,Device,Channel,(ReadOnly | optional)"
        /// </summary>
        /// <param name="partLines">The string line describing a single IOPart.</param>
        /// <param name="type">The IO part type. ie. DIBit, DOBit, etc.</param>
        /// <param name="ioParts">The IO parts.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">IO part must be in the form {name},{device name},{channel}</exception>
        private IList<string> CreateIOParts(IEnumerable<string> partLines, IOPartType type, out IList<IIOPart> ioParts)
        {
            ioParts = new List<IIOPart>();
            List<string> forcedParts = new List<string>();
            
            foreach (string partLine in partLines)
            {
                IIOPart part = null;

                try
                {
                    string[] partInfo = partLine.Split(',');
                    string partName = partInfo[0];
                    string deviceName = partInfo[1];
                    int channel = -1;
                    bool readOnly = false;

                    if (partInfo.Length > 2)
                    {
                        int.TryParse(partInfo[2], out channel);
                    }

                    if (channel < 0)
                        throw new Exception("IO part must be in the form {name},{device name},{channel}");

                    if (String.Equals(partInfo.Last(), "ReadOnly", StringComparison.CurrentCultureIgnoreCase) ||
                        String.Equals(partInfo.Last(), "Reserved", StringComparison.CurrentCultureIgnoreCase))
                    {
                        readOnly = true;
                    }

                    if (string.IsNullOrEmpty(deviceName))
                    {
                        deviceName = GetSimDeviceType().Name;

                        Log.Error(this, "No device name specified for {0}. Switching to a Simulated {1} device instead.", partInfo[0], Name);
                    }

                    IIODevice device = GetDevice(deviceName);
                                        
                    if (device == null)
                    {
                        string simName = GetSimDeviceType().Name;

                        if (!String.Equals(deviceName, simName, StringComparison.CurrentCultureIgnoreCase))
                            Log.Error(this, "No device found with the name '{0}'. Switching to a Simulated {1} device instead.", deviceName, Name);

                        device = GetSimDevice(deviceName);
                    }

                    try
                    {
                        if (!device.Connected)
                            device.Open();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this, "{0} could not connect to {1} device '{2}' on port {3}. {4}", typeof(IIOPart).Name, device.GetType().Name, device.Name, device.Port, ex);
                        Log.Error(this, "Switching {0} '{1}' to simulated device instead.", typeof(IIOPart).Name, partName);
                        device = GetSimDevice(deviceName);
                    }

                    if (!_devMgr.Devices.Contains(deviceName))
                        _devMgr.Devices.Add(device);

                    switch (type)
                    {
                        case IOPartType.DIBit:
                            part = new DIBit(device);
                            break;
                        case IOPartType.DOBit:
                            part = new DOBit(device);
                            break;
                        case IOPartType.AIBit:
                            part = new AIBit(device);
                            break;
                        case IOPartType.AOBit:
                            part = new AOBit(device);
                            break;
                        case IOPartType.DIWord:
                            DIWord inWord = new DIWord(device);
                            InitializeWord(inWord, partInfo);
                            part = inWord;                            
                            break;
                        case IOPartType.DOWord:
                            DOWord outWord = new DOWord(device);
                            InitializeWord(outWord, partInfo);
                            part = outWord;
                            break;
                    }
                    
                    part.Name = partName;
                    part.Channel = channel;
                    part.ReadOnly = readOnly;

                    ioParts.Add(part);

                    if (!String.Equals(device.Name, deviceName, StringComparison.CurrentCultureIgnoreCase))
                        forcedParts.Add(part.Name);
                }
                catch (Exception ex)
                {
                    string error = "";

                    if (type == IOPartType.DIWord || type == IOPartType.DOWord)
                        error += "Word ";
                    else
                        error += "Bit ";

                    if (part != null && !String.IsNullOrEmpty(part.Name))
                        error += part.Name + " ";

                    error += String.Format("could not be parsed from string {0}.\n{1}", partLine, ex);

                    Log.Error(this, error);
                }
            }

            return forcedParts;
        }

        private void InitializeWord(DIOWord word, string[] parameters)
        {
            int size = -1;
            int start = -1;

            if (parameters.Length > 4)
            {
                int.TryParse(parameters[3], out size);
                int.TryParse(parameters[4], out start);
            }
            else if (parameters.Length > 3)
            {
                int.TryParse(parameters[3], out start);
            }

            if (size < 0)
                size = ((IIODevice)word.Device).GetDIOWordLength();

            if (size < 0)
                throw new Exception("Invalid word size.");

            if (start < 0)
                throw new Exception("Invalid starting bit channel.");

            word.WordLength = size;
            word.StartBitChannel = start;
        }

        /// <summary>
        /// Registers all parts to their parent device. This is called after <see cref="IPart"/> creation.
        /// </summary>
        public override void RegisterParts(IEnumerable<IIOPart> parts)
        {
            foreach (IIOPart part in parts)
            {
                IIODevice device = part.Device as IIODevice;

                if (part is DIBit)
                    device.RegisterDIBit((DIBit)part);
                else if (part is DOBit)
                    device.RegisterDOBit((DOBit)part);
                else if (part is AIBit)
                    device.RegisterAIBit((AIBit)part);
                else if (part is AOBit)
                    device.RegisterAOBit((AOBit)part);
                if (part is DIWord)
                    device.RegisterDIWord((DIWord)part);
                else if (part is DOWord)
                    device.RegisterDOWord((DOWord)part);
            }
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="partNode">The part node.</param>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override IIOPart CreatePart(SettingsNode partNode, IIODevice device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a generic settings template.
        /// </summary>
        /// <returns></returns>
        public override SettingsDocument GenerateSettingsTemplate()
        {
            SettingsDocument doc = new SettingsDocument();

            //TODO - create an example IO config, instead of a generic settings document.

            return doc;
        }

        //
        // Digital IO
        //

        /// <summary>
        /// Gets the current state of the <see cref="DIBit"/> at the specified index of all <see cref="DIBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="DIBit"/> current state.</returns>
        public bool GetDIBit(int index)
        {
            return DIBits[index].Get();
        }

        /// <summary>
        /// Gets the current value of the <see cref="DIBit"/> at the specified index of all <see cref="DIBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="DIBit"/> current value.</returns>
        public int GetDIBitValue(int index)
        {
            return DIBits[index].GetValue();
        }


        /// <summary>
        /// Gets the current state of the <see cref="DOBit"/> at the specified index of all <see cref="DOBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="DOBit"/> current state.</returns>
        public bool GetDOBit(int index)
        {
            return DOBits[index].Get();
        }

        /// <summary>
        /// Gets the current value of the <see cref="DOBit"/> at the specified index of all <see cref="DOBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="DOBit"/> current value.</returns>
        public int GetDOBitValue(int index)
        {
            return DOBits[index].GetValue();
        }

        /// <summary>
        /// Gets the current values of the <see cref="DIBit" />s at the specified indices of all <see cref="DIBits" />.
        /// </summary>
        /// <param name="indices">The indices of each <see cref="DIBit"/>.</param>
        /// <returns>
        /// An array of the <see cref="DIBit"/>s current values.
        /// </returns>
        public int[] GetDIBits(params int[] indices)
        {
            List<DIBit> bits = new List<DIBit>();

            for (int i = 0; i < indices.Length; i++)
                bits.Add(DIBits[indices[i]]);

            return bits.GetValues().ToArray();
        }

        /// <summary>
        /// Gets the current values of the <see cref="DOBit"/>s at the specified indices of all <see cref="DOBits" />.
        /// </summary>
        /// <param name="indices">The indices of each <see cref="DOBit"/>.</param>
        /// <returns>
        /// An array of the <see cref="DOBit"/>s current values.
        /// </returns>
        public int[] GetDOBits(params int[] indices)
        {
            List<DOBit> bits = new List<DOBit>();

            for (int i = 0; i < indices.Length; i++)
                bits.Add(DOBits[indices[i]]);

            return bits.GetValues().ToArray();
        }

        /// <summary>
        /// Sets the current value of the <see cref="DIBit" /> at the specified index of all <see cref="DIBits" />.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value (0 or 1).</param>
        public void SetDIBit(int index, int value)
        {
            DIBits[index].Set(value);
        }

        /// <summary>
        /// Sets the current value of the <see cref="DOBit" /> at the specified index of all <see cref="DOBits" />.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value (0 or 1).</param>
        public void SetDOBit(int index, int value)
        {
            DOBits[index].Set(value);
        }

        /// <summary>
        /// Sets the current values of the <see cref="DIBit" />s at the specified indices of all <see cref="DIBits" />.
        /// </summary>
        /// <param name="indices">The indices of each <see cref="DIBit" />.</param>
        /// <param name="values">An array of values that must match the length of the indices array.</param>
        public void SetDIBits(int[] indices, int[] values)
        {
            List<DIBit> bits = new List<DIBit>();

            for (int i = 0; i < indices.Length; i++)
                bits.Add(DIBits[indices[i]]);

            bits.SetValues(values);
        }

        /// <summary>
        /// Sets the current values of the <see cref="DOBit" />s at the specified indices of all <see cref="DOBits" />.
        /// </summary>
        /// <param name="indices">The indices of each <see cref="DOBit" />.</param>
        /// <param name="values">An array of values that must match the length of the indices array.</param>
        public void SetDOBits(int[] indices, int[] values)
        {
            List<DOBit> bits = new List<DOBit>();

            for (int i = 0; i < indices.Length; i++)
                bits.Add(DOBits[indices[i]]);

            bits.SetValues(values);
        }

        /// <summary>
        /// Gets the current state of the <see cref="DIWord"/> at the specified index of all <see cref="DIWords"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="DIWord"/> current value.</returns>
        public ulong GetDIWord(int index)
        {
            return DIWords[index].Get();
        }

        /// <summary>
        /// Gets the current state of the <see cref="DOWord"/> at the specified index of all <see cref="DOWords"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="DOWord"/> current value.</returns>
        public ulong GetDOWord(int index)
        {
            return DOWords[index].Get();
        }
        
        //
        // Analog IO
        //

        /// <summary>
        /// Gets the current value of the <see cref="AIBit"/> at the specified index of all <see cref="AIBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="AIBit"/> current value.</returns>
        public double GetAIBit(int index)
        {
            return AIBits[index].Get();
        }

        /// <summary>
        /// Gets the current value of the <see cref="AOBit"/> at the specified index of all <see cref="AOBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="AOBit"/> current value.</returns>
        public double GetAOBit(int index)
        {
            return AOBits[index].Get();
        }

        /// <summary>
        /// Sets the current value of the <see cref="AIBit"/> at the specified index of all <see cref="AIBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="AIBit"/> current value.</returns>
        public void SetAIBit(int index, double value)
        {
            AIBits[index].Set(value);
        }

        /// <summary>
        /// Gets the current value of the <see cref="AOBit"/> at the specified index of all <see cref="AOBits"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="AOBit"/> current value.</returns>
        public void SetAOBit(int index, double value)
        {
            AOBits[index].Set(value);
        }
    }
}
