//
//  (c) Copyright 2013 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2013/08/01] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Settings;
using System.IO;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Utils;


namespace Seagate.AAS.HGA.HST.Controllers
{    
    public abstract class ControllerHST
    {
        // Nested declarations -------------------------------------------------
     
        // Member variables ----------------------------------------------------

        protected HSTWorkcell _workcell; 
        protected HSTIOManifest _ioManifest;
        protected int _currentProcessErrorNum;        
        protected ActiveProcessHST _currentProcessInstance;

        // Constructors & Finalizers -------------------------------------------

        public ControllerHST()
        {

        }

        public ControllerHST(HSTWorkcell workcell, string controllerID, string controllerName)
        {
            _workcell = workcell;            
            _ioManifest = (HSTIOManifest)workcell.IOManifest;

            _controllerID = controllerID;
            _controllerName = controllerName;

            _controllerErrorList = new Dictionary<int, string>();
            AddControllerError();

            // Generate a text file contains errors of this controller.
            OutputControllerErrorListToTextFile();
        }

        // Properties ----------------------------------------------------------

        protected string _controllerID = "";
        public string ControllerID
        {
            get { return _controllerID; }
        }

        protected string _controllerName = "";
        public string ControllerName
        {
            get { return _controllerName; }
        }
        

        protected Dictionary<int, string> _controllerErrorList;
        /// <summary>
        /// This list store exceptions that are capable be thrown by this type.
        /// </summary>
        public Dictionary<int, string> ControllerErrorList
        {
            get { return _controllerErrorList; }
        }
        // Methods -------------------------------------------------------------
        // Methods -------------------------------------------------------------
        public abstract void InitializeController();

        public virtual void OutputControllerErrorListToTextFile()
        {
            OutputControllerErrorListToTextFile("");
        }

        public virtual void OutputControllerErrorListToTextFile(string fileName)
        {
            string fn = "";
            if (fileName == "")
            {
                if (!Directory.Exists(HSTMachine.Workcell.HSTSettings.Directory.ErrorHandlingPath))
                {
                    Directory.CreateDirectory(HSTMachine.Workcell.HSTSettings.Directory.ErrorHandlingPath);
                }
                fn = HSTMachine.Workcell.HSTSettings.Directory.ErrorHandlingPath + "\\" + _controllerName + "[" + _controllerID + "]" + " Errors.txt";
            }
            else
                fn = fileName;
            //Seagate.AAS.EFI.Log.EfiLogger _efiLogger = new EFI.Log.EfiLogger(ConfigCLU.Directories.ErrorHandlingDir + "\\" + _controllerName + _controllerID);

            //_efiLogger.Log(

            StreamWriter sw = new StreamWriter(fn);
            sw.WriteLine("[" + _controllerName + "]");
            foreach (KeyValuePair<int, string> err in _controllerErrorList)
            {
                sw.WriteLine(_controllerID + err.Key.ToString("000000") + ": " + err.Value);
            }
            sw.Close();
        }

        //Process Instance and Process error code 
        public virtual void SetProcessCode(ActiveProcessHST processInstance, int procErrNum)
        {
            /*if (ExceptionDocumenter.EnableSelfDocumentation)
            {
                _currentProcessErrorNum = procErrNum;
                _currentProcessInstance = processInstance;
            }*/
        }

        public virtual string GetErrorCode(int errorNum)
        {
            return _controllerID + errorNum.ToString("000000");
        }

        //Child class call AddControllerError() to implement the error code list for this specific controller
        // Call AddError to add error code and error messages into ControllerErrorList.
        protected virtual void AddControllerError()
        { }
        protected void AddError(int errorNum, string errorMessage)
        {
            _controllerErrorList.Add(errorNum, errorMessage);
        }

        protected ControllerException CreateControllerException(Enum errorNum, Exception innerException)
        {
            // Convert Enum to appropriate Enum object and cast to integer
            int code = (int)Enum.Parse(errorNum.GetType(), errorNum.ToString());
            return CreateControllerException(code, innerException);
        }

        // Controller exception catch use this function to assign the error code 
        protected ControllerException CreateControllerException(int ctrErrNum, Exception innerException)
        {
            string errorMessage = _controllerErrorList[ctrErrNum];

            if (innerException != null && innerException is ControllerException)
            {
                // Do not wrap inner ControllerException. Just use the most inner ControllerException.
                ((ControllerException)innerException).Controller = this;
                return (ControllerException)innerException;
            }
            else
            {
                if (innerException != null)
                {
                    ControllerException conEx = new ControllerException(errorMessage, innerException);
                    conEx.ErrorCode = GetErrorCode(ctrErrNum);
                    conEx.Controller = this;
                    if (conEx.Source == null)
                        conEx.Source = conEx.Controller.ControllerName;
                    return conEx;
                }
                else
                {
                    ControllerException conEx = new ControllerException(errorMessage);
                    conEx.ErrorCode = GetErrorCode(ctrErrNum);
                    conEx.Controller = this;
                    if (conEx.Source == null)
                        conEx.Source = conEx.Controller.ControllerName;
                    return conEx;
                }
            }
        }        

        // Controller use for error documentattions (such as action 1, 2, 3.........)
        protected void SetAction(int currentActionIndex)
        {
                if (_currentProcessInstance != null)
                {
                    string errCode = _currentProcessInstance.ProcessID + _currentProcessErrorNum.ToString("0000") + "-" +
                        _controllerID + currentActionIndex.ToString("000000");
                    string proMes = _currentProcessInstance.ProcessErrorList[_currentProcessErrorNum];
                    string ctrMes = _controllerErrorList[currentActionIndex];
                    string errMes = HandlerException.GetHandlerErrorMessage(
                        errCode, proMes, ctrMes);
                    string errorCodeDetail = errCode + "-000";
                }
        }



        // Internal methods ----------------------------------------------------
       private void  AddProcessError()
        {
        }
    }
}
