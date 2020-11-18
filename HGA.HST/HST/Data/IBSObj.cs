using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    public class IBSObj
    {
        public enum IbsPattern
        {
            Unknown,
            A,
            W1,
            W2
        }

        public enum ChoiceFlag
        {
            Unknown,
            ID,
            OD
        }

        private string _rdPattern;
        private string _wrPattern;
        private bool _isWrAll;
        private string _choiceFlag;
        private HGAProductTabType _tab;
        private const char PatternAll = 'A';
        private const char Pattern1 = '1';
        private const char Pattern2 = '2';

        public IBSObj()
        {
            Default();
        }

        public IBSObj(string rd_pattern, string wr_pattern, HGAProductTabType hgaTab)
        {
            Default();
            _rdPattern = rd_pattern;
            _wrPattern = wr_pattern;
            _tab = hgaTab;
            CalculateChoiceFlage();
        }
        public IBSObj(IbsPattern rd_pattern, IbsPattern wr_pattern, HGAProductTabType hgaTab)
        {
            Default();
            AssignRDPattern(rd_pattern);
            AssignWRPattern(wr_pattern);
            _tab = hgaTab;
            CalculateChoiceFlage();
        }

        public string Current_RD_Pattern
        {
            get { return _rdPattern; }
        }

        public string Current_WR_Pattern
        {
            get { return _wrPattern; }
        }

        public string CurentChoiceFlag
        {
            get { return _choiceFlag; }
        }

        public bool IsWriterAllPattern
        {
            get { return _isWrAll; }
        }

        private void CalculateChoiceFlage()
        {
            try
            {
                _isWrAll = _wrPattern.Contains("?") || _wrPattern.Contains(IbsPattern.A.ToString()) || _wrPattern == string.Empty ? true : false;

                if (!_isWrAll)
                {
                    if (string.Equals(_rdPattern, PatternAll.ToString()))    //RD = A
                    {
                        if (_tab == HGAProductTabType.Up)
                        {
                            if (string.Equals(_wrPattern, Pattern1.ToString()))      //WR=1 && Up
                                _choiceFlag = ChoiceFlag.ID.ToString();
                            else if (string.Equals(_wrPattern, Pattern2.ToString())) //WR=2 && Up
                                _choiceFlag = ChoiceFlag.OD.ToString();
                        }
                        else if (_tab == HGAProductTabType.Down)
                        {
                            if (string.Equals(_wrPattern, Pattern1.ToString()))      //WR=1 && Dn
                                _choiceFlag = ChoiceFlag.OD.ToString();
                            else if (string.Equals(_wrPattern, Pattern2.ToString())) //WR=2 && Dn
                                _choiceFlag = ChoiceFlag.ID.ToString();
                        }
                    }
                    else if (string.Equals(_rdPattern, Pattern1.ToString()))  //RD = 1
                    {
                        if (string.Equals(_wrPattern, Pattern1.ToString()))      
                        {
                            if (_tab == HGAProductTabType.Up)
                                _choiceFlag = ChoiceFlag.ID.ToString();
                            else if (_tab == HGAProductTabType.Down)
                                _choiceFlag = ChoiceFlag.OD.ToString();
                        }
                        else if (string.Equals(_wrPattern, Pattern2.ToString()))
                        {
                            if (_tab == HGAProductTabType.Up)
                                _choiceFlag = ChoiceFlag.OD.ToString();
                            else if (_tab == HGAProductTabType.Down)
                                _choiceFlag = ChoiceFlag.ID.ToString();
                        }
                    }
                    else if (string.Equals(_rdPattern, Pattern2.ToString()))   //RD = 2
                    {
                        if (string.Equals(_wrPattern, Pattern1.ToString()))      
                        {
                            if (_tab == HGAProductTabType.Up)
                                _choiceFlag = ChoiceFlag.ID.ToString();
                            else if (_tab == HGAProductTabType.Down)
                                _choiceFlag = ChoiceFlag.OD.ToString();
                        }
                        else if (string.Equals(_wrPattern, Pattern2.ToString()))
                        {
                            if (_tab == HGAProductTabType.Up)
                                _choiceFlag = ChoiceFlag.OD.ToString();
                            else if (_tab == HGAProductTabType.Down)
                                _choiceFlag = ChoiceFlag.ID.ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                _choiceFlag = string.Empty;
            }
        }


        private void AssignRDPattern(IbsPattern pattern)
        {
            switch (pattern)
            {
                case IbsPattern.Unknown:
                    _rdPattern = string.Empty;
                    break;
                case IbsPattern.A:
                    _rdPattern = PatternAll.ToString();
                    break;
                case IbsPattern.W1:
                    _rdPattern = Pattern1.ToString();
                    break;
                case IbsPattern.W2:
                    _rdPattern = Pattern2.ToString();
                    break;
                default:
                    break;
            }
        }

        private void AssignWRPattern(IbsPattern pattern)
        {
            switch (pattern)
            {
                case IbsPattern.Unknown:
                    _wrPattern = string.Empty;
                    break;
                case IbsPattern.A:
                    _wrPattern = PatternAll.ToString();
                    break;
                case IbsPattern.W1:
                    _wrPattern = Pattern1.ToString();
                    break;
                case IbsPattern.W2:
                    _wrPattern = Pattern2.ToString();
                    break;
                default:
                    break;
            }
        }

        private void Default()
        {
            _rdPattern = string.Empty;
            _wrPattern = string.Empty;
            _isWrAll = false;
            _tab = HGAProductTabType.Unknown;
            _choiceFlag = string.Empty;
        }
    }
}
