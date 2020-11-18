using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Seagate.AAS.Parsel.Hw.Aerotech.A3200;
using Seagate.AAS.Parsel.Hw;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Utils;
using System.Threading;

namespace Seagate.AAS.HGA.HST.Process
{
    public class MonitorIOState
    {
    
        private IDigitalInput _di;
        private IDigitalOutput _do;
        private IAnalogInput _ai;
        private string text;

        private IOState[,] _InputState;
        private IOState[,] _AnalogueState;
        private IOState[,] _OutputState;

        
        private string _directory;

        public MonitorIOState(HSTSettings settings)
        {
           
            _InputState = new IOState[8, 8];
            _AnalogueState = new IOState[1, 2];
            _OutputState = new IOState[8, 8];

            _directory = settings.Directory.IOChangeStatePath;

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
            // ********************************input*************************************************
            int iloop = 0;
            for (int icard = 0; icard < 3; icard++, iloop++)
            {
                for (int ibit = 0; ibit < 8; ibit++)
                {
                    _InputState[icard, ibit] = new IOState(settings.Directory.IOChangeStatePath, settings, true, HSTMachine.Workcell, 0, ibit + (8 * iloop));
                    
                }
            }

            for (int icard = 3; icard < 8; icard++)
            {
                for (int ibit = 0; ibit < 8; ibit++)
                {
                    _InputState[icard, ibit] = new IOState(settings.Directory.IOChangeStatePath, settings, true, HSTMachine.Workcell, icard-2, ibit);

                }
            }
            // ********************************input*************************************************


            // ********************************Analogue*************************************************
            for (int analogueInput = 0; analogueInput < 2; analogueInput++)
            {
                _AnalogueState[0, analogueInput] = new IOState(settings.Directory.IOChangeStatePath, settings, false, HSTMachine.Workcell, 0, analogueInput);                
            }

            // ********************************Analogue*************************************************

            // ********************************Output*************************************************

            iloop = 0;
            for (int icard = 0; icard < 3; icard++, iloop++)
            {
                for (int ibit = 0; ibit < 8; ibit++)//each card got 8 bits
                {
                    _OutputState[icard, ibit] = new IOState(settings.Directory.IOChangeStatePath, settings, true, HSTMachine.Workcell, 0, ibit + (8*iloop));
                   
                }
            }

            for (int icard = 3; icard < 8; icard++)
            {
                for (int ibit = 0; ibit < 8; ibit++)//each card got 8 bits
                {
                    _OutputState[icard, ibit] = new IOState(settings.Directory.IOChangeStatePath, settings, true, HSTMachine.Workcell, icard - 2, ibit);

                }
            }

            // ********************************Output*************************************************
        }

        public void GetIOState()
        {

            //Input IO
            for (int icard = 0; icard < 5; icard++)//card 1 to card 5
            {
                for (int ibit = 0; ibit < 8; ibit++)//each card got 8 bits
                {
                    _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(ibit + (icard * 8));
                    text = "B" + (icard + 1).ToString() + "I" + (ibit + 1).ToString() + " - " + _di.ToString().Replace("_", " ");

                    if (_di.Name.Contains("NOT USED"))//don't display for those not used/spare
                    {
                        text = "B" + (icard + 1).ToString() + "I" + (ibit + 1).ToString() + " - " + "NOT USED";
                    }
                    
                    _InputState[icard, ibit].IOName = text;
                    _InputState[icard, ibit].AssignAxis(HSTMachine.Workcell._a3200HC, icard, ibit);
                }
            }

            //card 6 have 20 input, but still display in 8 input per set
            int iloop = 0;
            for (int icard = 5; icard < 8; icard++, iloop++)//current only have 8 set
            {
                for (int ibit = 0; ibit < 8; ibit++)//each card got 8 bits
                {
                    if (icard == 7 && ibit > 3)//we have only 60 input, skip for 61 to 64
                        continue;

                    _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(ibit + (icard * 8));
                    text = "B6I" + (ibit + 1 + (8 * iloop)).ToString() + " - " + _di.ToString().Replace("_", " ");

                    if (_di.Name.Contains("NOT USED"))//don't display for those not used/spare
                    {
                        text = "B6I" + String.Format("{0}", ibit + 1 + (8 * iloop)) + " - " + "NOT USED";
                    }

                    _InputState[icard, ibit].IOName = text;
                    _InputState[icard, ibit].AssignAxis(HSTMachine.Workcell._a3200HC, 5, ibit + (8 * iloop));
                }
            }

            _ai = HSTMachine.Workcell._ioManifest.GetAnalogInput(0);
            text = _ai.ToString().Replace("_", " ");
            _AnalogueState[0, 0].IOName = text;
            _AnalogueState[0, 0].AssignAxis(HSTMachine.Workcell._a3200HC, _ai);
            
            _ai = HSTMachine.Workcell._ioManifest.GetAnalogInput(1);
            text = _ai.ToString().Replace("_", " ");

            _AnalogueState[0, 1].IOName = text;
            _AnalogueState[0, 1].AssignAxis(HSTMachine.Workcell._a3200HC, _ai);

            ///////////////////////////////////////////////////////////////////////////////////////
            //Analog Input Session : End
            ///////////////////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////////////////////////////
            //Output Session : Start
            ///////////////////////////////////////////////////////////////////////////////////////

            //have 8 set to display, 3 column and 3 row (1 place for spare)
            for (int icard = 0; icard < 5; icard++)
            {
                for (int ibit = 0; ibit < 8; ibit++)//each card got 8 bits
                {
                   
                    _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(ibit + (icard * 8));
                    text = "B" + (icard + 1).ToString() + "O" + (ibit + 1).ToString() + " - " + _do.ToString().Replace("_", " ");

                    if (_do.Name.Contains("NOT USED"))//don't display for those not used/spare
                    {
                        //continue;
                        text = "B" + (icard + 1).ToString() + "O" + (ibit + 1).ToString() + " - " + "NOT USED";
                    }

                    _OutputState[icard, ibit].IOName = text;
                    _OutputState[icard, ibit].AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)icard), icard, ibit);
                }
            }

            

            //card 6 have 20 output, but still display in 8 output per set
            iloop = 0;
            for (int icard = 5; icard < 8; icard++, iloop++)//current only have 8 set
            {
                for (int ibit = 0; ibit < 8; ibit++)//each card got 8 bits
                {
                    if (icard == 7 && ibit > 3)//we have only 60 input, skip for 61 to 64
                        continue;

                    _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(ibit + (icard * 8));
                    text = "B6O" + (ibit + 1 + (8 * iloop)).ToString() + " - " + _do.ToString().Replace("_", " ");

                    if (_do.Name.Contains("NOT USED"))//don't display for those not used/spare
                    {
                        //continue;
                        text = "B6O" + String.Format("{0}", ibit + 1 + (8 * iloop)) + " - " + "NOT USED";
                    }

                    _OutputState[icard, ibit].IOName = text;
                    _OutputState[icard, ibit].AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, ibit + (8 * iloop));
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            //Output Session : End
            ///////////////////////////////////////////////////////////////////////////////////////
        }     

        public void AssignSettings(HSTSettings settings)
        {
            for (int icard = 0; icard < 8; icard++)
            {
                for (int ibit = 0; ibit < 8; ibit++)
                {
                    if (_InputState[icard, ibit] == null || (icard == 7 && ibit > 3))
                        continue;
                    settings.OnSettingsChanged -= new EventHandler(_InputState[icard, ibit].SettingsChanged);
                    settings.OnSettingsChanged += new EventHandler(_InputState[icard, ibit].SettingsChanged);
                    
                    settings.OnSettingsChanged -= new EventHandler(_OutputState[icard, ibit].SettingsChanged);
                    settings.OnSettingsChanged += new EventHandler(_OutputState[icard, ibit].SettingsChanged);
                }
            }
            for (int analogueInput = 0; analogueInput < 2; analogueInput++)
            {
                settings.OnSettingsChanged -= new EventHandler(_AnalogueState[0, analogueInput].SettingsChanged);
                settings.OnSettingsChanged += new EventHandler(_AnalogueState[0, analogueInput].SettingsChanged);
            }
        }

        public void StartLogIOState()
        {
            for (int icard = 0; icard < 8; icard++)
            {
                for (int ibit = 0; ibit < 8; ibit++)
                {
                    if (_InputState[icard, ibit] == null || (icard == 7 && ibit > 3))
                        continue;
                    _InputState[icard, ibit].StartLogIOState();
                    _OutputState[icard, ibit].StartLogIOState();

                }
            }
            for (int analogueInput = 0; analogueInput < 2; analogueInput++)
            {
                if (_AnalogueState[0, analogueInput] == null)
                    continue;
                _AnalogueState[0, analogueInput].StartLogIOState();

            } 
        }
        public void StopLogIOState()
        {

            for (int icard = 0; icard < 8; icard++)
            {
                for (int ibit = 0; ibit < 8; ibit++)
                {
                    if (_InputState[icard, ibit] == null || (icard == 7 && ibit > 3))
                        continue;
                    _InputState[icard, ibit].stop();
                    _OutputState[icard, ibit].stop();

                }
            }
            for (int analogueInput = 0; analogueInput < 2; analogueInput++)
            {
                if (_AnalogueState[0, analogueInput] == null)
                    continue;
                _AnalogueState[0, analogueInput].stop();
                
            } 
       
       }


        public class IOState
        {
            public enum IOMode
            {
                Input,
                Output,
                AnalogInput
            }

            private System.Windows.Forms.Timer tmrUpdate;

            private bool _iostate;
            private string _IOname;
            private string _directory;
            private bool _digitalstate;
            private string fn;
            private string _analogValue;
            private int _icard;
            private int _ibit;
            
            HSTSettings _settings;


            private ReaderWriterLockSlim _filelock = new ReaderWriterLockSlim();
            HSTWorkcell _workcell;

            private A3200HC _a3200;
            private IDigitalInput _iDigitalInput;
            private IDigitalOutput _iDigitalOutput;
            private int _card;
            private int _bit;
            private IAxis _axis;
            private IAnalogInput _iAnalogInput;
            private IOMode _ioMode;
            private bool _log;
            public IOState(string directory, HSTSettings settings, bool digitalstate, HSTWorkcell workcell, int card, int bit)
            {
                _iostate = false;
                _analogValue = "UNKNOWN";
                _log = false;
                _directory = directory;
                _digitalstate = digitalstate;
                _workcell = workcell;
                _icard = card;
                _ibit = bit;
                _settings = settings;

                this.tmrUpdate = new System.Windows.Forms.Timer();
                this.tmrUpdate.Interval = 100;
                this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);

                if (settings.Install.LogIOChangeState)
                    _log = true;

            }
            public string IOName
            {
                set
                {
                    _IOname = value;
                }

                get
                {
                    return _IOname;
                }

            }


            public void SettingsChanged(object sender, EventArgs e)
            {
                try
                {
                    HSTSettings settings = sender as HSTSettings;

                    if (settings != null)
                        if (settings.Install.LogIOChangeState)
                        {
                            _log = true;
                        }
                        else
                        {
                            _log = false;
                        }

                }
                catch (Exception ex)
                { }
            }


            public void StartLogIOState()
            {
                try
                {
                    if (_log)
                    {

                        this.tmrUpdate.Enabled = true;
                        this.tmrUpdate.Start();
                    } 

                }
                catch (Exception ex)
                { }
            }
            //Output
            public void AssignAxis(A3200HC a3200, IAxis axis, int card, int bit)
            {
                _a3200 = a3200;
                _axis = axis;
                _card = card;
                _bit = bit;
                _ioMode = IOMode.Output;
                IDigitalOutput digitalOutput = HSTMachine.Workcell._ioManifest.GetDigitalOutput(_bit + (_card * 8));
                if (_a3200 != null)
                {
                    try
                    {
                        _iostate = _a3200.GetState(digitalOutput) == DigitalIOState.On ? true : false;
                    }
                    catch (Exception ex)
                    { }
                }
            }
            //Analog Input
            public void AssignAxis(A3200HC a3200, IAnalogInput iAnalogInput)
            {
                _a3200 = a3200;
                _iAnalogInput = iAnalogInput;
                _ioMode = IOMode.AnalogInput;
                if (_a3200 != null)
                {
                    try
                    {
                        _analogValue = _a3200.GetRawValue(iAnalogInput).ToString();
                    }
                    catch (Exception ex)
                    { }
                }
            }

            //Input
            public void AssignAxis(A3200HC a3200, int card, int bit)
            {
                _a3200 = a3200;
                _card = card;
                _bit = bit;
                _ioMode = IOMode.Input;
                IDigitalInput digitalInput = HSTMachine.Workcell._ioManifest.GetDigitalInput(_bit + (_card * 8));
                if (_a3200 != null)
                {

                    try
                    {
                        _iostate = _a3200.GetState(digitalInput) == DigitalIOState.On ? true : false;
                    }
                    catch (Exception ex)
                    { }

                }

            }
           

            private void tmrUpdate_Tick(object sender, EventArgs e)
            {

                if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
                {
                    if (_a3200 != null)
                    {

                        _filelock.EnterWriteLock();
                        try
                        {
                            fn = _directory + "\\" + "IOSTATE" + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";

                            using (StreamWriter outputFile = new StreamWriter(fn, true))
                            {
                                switch ((int)_ioMode)
                                {
                                    case 0://input
                                        IDigitalInput digitalInput = HSTMachine.Workcell._ioManifest.GetDigitalInput(_bit + (_card * 8)); ;
                                        bool outputInput = _a3200.GetState(digitalInput) == DigitalIOState.On ? true : false;
                                        if (outputInput != _iostate)
                                        {
                                            {
                                                _iostate = outputInput;
                                                outputFile.WriteLine(String.Format("{0}, Input Digital,{1}, {2}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff"), _IOname, outputInput.ToString()));

                                            }
                                        }
                                        break;
                                    case 1://output
                                        IDigitalOutput digitalOutput = HSTMachine.Workcell._ioManifest.GetDigitalOutput(_bit + (_card * 8)); ;
                                        bool outputOutput = _a3200.GetState(digitalOutput) == DigitalIOState.On ? true : false;
                                        if (outputOutput != _iostate)
                                        {
                                            _iostate = outputOutput;
                                        }
                                        break;
                                    case 2://analog input
                                        string outputAnalog = _a3200.GetRawValue(_iAnalogInput).ToString();
                                        if (!_analogValue.Equals(outputAnalog))
                                        {
                                            _analogValue = outputAnalog;
                                        }
                                        break;
                                    default:

                                        break;
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            _filelock.ExitWriteLock();

                        }

                    }
                }
            }


            public void stop()
            {
                if (tmrUpdate != null)
                {
                    this.tmrUpdate.Enabled = false;
                    this.tmrUpdate.Stop();
                }
            }
        }

        
    }



}
