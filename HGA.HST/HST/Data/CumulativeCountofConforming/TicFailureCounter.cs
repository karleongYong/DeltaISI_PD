using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming
{

    public class TicFailureCounter
    {
        private Dictionary<string, int> _failCode;

        public TicFailureCounter()
        {
            _failCode = new Dictionary<string, int>();
        }

        public void UpdateFailure(string errorcode)
        {
            var currentCounter = _failCode.Where(k => k.Key == errorcode).ToList();
            if (currentCounter.Count != 0)
            {
                var counter = currentCounter.FirstOrDefault().Value;
                counter++;
                _failCode[errorcode] = counter;
            }
            else
                _failCode.Add(errorcode.ToString(), 1);
        }

        public string GetTopCounterFailCode()
        {
            string returncode = string.Empty;
            int topnumber = 0;
            foreach (var item in _failCode)
            {
                var currentnumber = Convert.ToInt32(item.Value);
                if (currentnumber > topnumber)
                    returncode = item.Key;
            }

            return returncode;
        }

        public int GetCounterFailTotal()
        {
            int getcounter = 0;
            foreach (var item in _failCode)
            {
                var currentnumber = Convert.ToInt32(item.Value);
                getcounter += currentnumber;
            }

            return getcounter;
        }

        public int GetFailCodeCounter(string errorcode)
        {
            return _failCode[errorcode];
        }
    }

    public class MachineFailure
    {
        private Dictionary<string, TicFailureCounter> _failure;

        public MachineFailure()
        {
            _failure = new Dictionary<string, TicFailureCounter>();
        }
        public MachineFailure(string ticname)
        {
            _failure = new Dictionary<string, TicFailureCounter>();
        }

        public void UpdateFailure(string ticname, string errorcode)
        {
            var currentCounter = _failure.Where(k => k.Key == ticname).ToList();

            if (currentCounter.Count < 1)
            {
                var failure = new TicFailureCounter();
                _failure.Add(ticname, failure);
                failure.UpdateFailure(errorcode);
            }
            else
            {
                _failure[ticname].UpdateFailure(errorcode);
            }
        }

        public string GetTopFailMC()
        {
            string topnumberMC = string.Empty;
            int topnumber = 0;
            foreach (var item in _failure)
            {
                var code = GetTopCounterFailCodeByMC(item.Key);
                var counter = _failure[item.Key].GetFailCodeCounter(code);
                if (counter > topnumber)
                {
                    topnumberMC = item.Key;
                }
            }

            return topnumberMC;
        }

        public int GetCounterFailByMC(string ticname)
        {
            return _failure[ticname].GetCounterFailTotal();
        }

        public string GetTopCounterFailCodeByMC(string ticname)
        {
            if (_failure.Count() > 0)
            {
                return _failure[ticname].GetTopCounterFailCode();
            }
            else
            {
                return "Unknown";
            }

        }

        public void Reset()
        {
            _failure.Clear();
        }

        public void ResetByMC(string name)
        {
            _failure.Remove(name);
        }

    }

    public class CCCAlertInformation
    {
        //Mar 2020 
        public string LastFailureMessage = "";
        public string LastFailureStation = "";
        public string getLastFailureMessage { get { return LastFailureMessage; } set { LastFailureMessage = value; } }
        public string getLastFailureStation { get { return LastFailureStation; } set { LastFailureStation = value; } }


        public enum CCCMcDefect
        {
            Unknown,
            TICDefect,
            HSTDefect
        }

        public string CarrierId { get; set; }

        public string FailedMc { get; set; }

        public string FailedCode { get; set; }

        public string FailDockNumber { get; set; }
        public string UticFailedType { get; set; }

    }

    public class GraphWithMcMapping
    {
        public string Graph1McName { get; set; }
        public string Graph2McName { get; set; }
    }

    public class CCCDefectSelection : EventArgs
    {
        public CCCDefectSelection(CCCAlertInformation.CCCMcDefect defect)
        {
            Defect = defect;
        }

        public CCCAlertInformation.CCCMcDefect Defect { get; set; }

    }

}
