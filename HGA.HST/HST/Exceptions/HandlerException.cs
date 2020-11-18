using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.HGA.HST.Controllers;
using System.IO;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using Seagate.AAS.Parsel.Services;

namespace Seagate.AAS.HGA.HST.Exceptions
{
    /// <summary>
    /// Represent handler exception.
    /// 
    /// Provides static methods to create HandlerException instance and open chm help
    /// file according to error code.
    /// 
    /// This class contains static CustomizedHandlerErrMsgList and HandlerErrorList that can be used to provide
    /// customized handler error message. During creation of handler error message,
    /// if the error code matches any key defined in the list, the error message 
    /// in the list will be retrieved and become the handler error message. The priority is 
    /// given to CustomizedHandlerErrMsgList.
    /// 
    /// CustomizedHandlerErrMsgList's data is loaded from text document defined by constant CustomizedHandlerErrorMessageSource.
    /// The error message in the list is used to overwrite default handler error message.
    /// 
    /// HandlerErrorList's data is loaded within the static constructor of this type. It is used for 
    /// customization of handler error message in design time. The error emssage in the list is used to overwrite
    /// default error message. The priority is given to CustomizedHandlerErrList.
    /// </summary>
    public class HandlerException : HandlerExceptionBase
    {
        // Nested declarations -------------------------------------------------    

        // Member variables ----------------------------------------------------
        public const string CustomizedHandlerErrorMessageSource = @"C:\Seagate\HGA.HST\Setup\CustomizedHandlerErrMsg.txt";
        private static Dictionary<string, int> _helpLinkList = new Dictionary<string, int>();

        // Constructors & Finalizers -------------------------------------------
        static HandlerException()
        {            
            GetCustomizedHandlerErrorMessage();

            // Add handler specific and unique error code and its ascociated help message here.
            _handlerErrorList = new Dictionary<string, string>();
            //_handlerErrorList.Add("A002-A002", "Conveyor was trying to clear workzone when it failed to down Boat Stopper.");
            //_handlerErrorList.Add("A005-A003", "Conveyor failed to loacate boat because it failed to extend Boat Pusher.");
        }

        public HandlerException()
        {
        }

        public HandlerException(string message)
            : base(message)
        {
        }

        public HandlerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Properties ----------------------------------------------------------

        private static Dictionary<string, string> _handlerErrorList;
        /// <summary>
        /// A list of error code keys and error messages that were pre-configured to be used to overwrite default
        /// handler error emssage.
        /// </summary>
        public static Dictionary<string, string> HandlerErrorList
        {
            get { return _handlerErrorList; }
        }

        private static Dictionary<string, string> _customizedHandlerErrMsgList = new Dictionary<string, string>();
        /// <summary>
        /// A list of error code and error messages pair loaded from text document defined by constant CustomizedHandlerErrorMessageSource
        /// used to overwrite default handler error message.
        /// </summary>
        public static Dictionary<string, string> CustomizedHandlerErrMsgList
        {
            get { return _customizedHandlerErrMsgList; }
        }

        // Methods -------------------------------------------------------------
        /// <summary>
        /// Create a HandlerException instance.
        /// </summary>
        /// <param name="innerException"></param>
        /// <returns></returns>
        public static HandlerException CreateHandlerException(Exception innerException)
        {
            //HandlerException he2 = new HandlerException(innerException.Message, innerException);
            //return he2;

            ControllerHST controller = null;
            string message = "";
            string proMes = "";
            string ctrMes = "";
            string devMes = "";
            string proEc = "Z000"; // Z for process error code not found.
            string ctrEc = "Z000"; // Z for controller error code not found.
            string devEc = "000";// 000 for device error code not found.
            // Get process error code.
            if (innerException is ProcessException)
            {
                proEc = ((ProcessException)innerException).ErrorCode;
                proMes = ((ProcessException)innerException).Message;
            }
            // Get controller error code.
            if (innerException.InnerException != null && innerException.InnerException is ControllerException)
            {
                ctrEc = ((ControllerException)innerException.InnerException).ErrorCode;
                ctrMes = ((ControllerException)innerException.InnerException).Message;
                controller = ((ControllerException)innerException.InnerException).Controller;

                // Get device error code
                if (innerException.InnerException.InnerException != null && false)
                {
                    // Not implemented. Unless base library for device support Error Code.
                    devEc = "??";
                    devMes = "??";
                }
            }

            // Construct error code.
            string errCode = GetHandlerErrorCode(proEc, ctrEc);

            // Construct error message.
            if (innerException.InnerException != null)
            {
                message = GetHandlerErrorMessage(errCode, proMes, ctrMes);
            }
            else
            {
                message = proMes; // Monitor process doesn't have controller message.
            }

            string errCodeDetail = errCode + "-" + devEc;

            string mesExtra = message + Environment.NewLine + Environment.NewLine;
            mesExtra = mesExtra + "[Exceptions hierarchy]";            

            HandlerException he = new HandlerException(mesExtra, innerException);
            he.ErrorCode = errCodeDetail;
            return he;
        }

        /// <summary>
        /// Get handler exception error code. It is a combination of 
        /// process error code and controller error code.
        /// </summary>
        /// <param name="processErrorCode"></param>
        /// <param name="controllerErrorCode"></param>
        /// <returns></returns>
        public static string GetHandlerErrorCode(string processErrorCode, string controllerErrorCode)
        {
            return processErrorCode + "-" + controllerErrorCode;
        }

        /// <summary>
        /// Get handler error message. 
        /// 
        /// If specified error code is exist in 
        /// CustomizedHandlerErrMsgList, then the error message 
        /// contained within the list will be used.
        /// 
        /// If specified error code is exist in
        /// HandlerErrorList, the error message contained 
        /// in that list will be used. 
        /// 
        /// If both above condition is not fulfilled, the handler error message is 
        /// constructed automatically by specified process error message and
        /// controller error message.
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="processErrMessage"></param>
        /// <param name="controllerErrMessage"></param>
        /// <returns></returns>
        public static string GetHandlerErrorMessage(string errCode, string processErrMessage, string controllerErrMessage)
        {
            string message = "";
            // Customized handler error list contains the code, then use the key to retrieve the error message.
            if (_customizedHandlerErrMsgList.ContainsKey(errCode))
            {
                message = _customizedHandlerErrMsgList[errCode];
                return message;
            }
            // If Handler Error List contain the code, then use the key to retrieve the error message.
            if (_handlerErrorList.ContainsKey(errCode))
            {
                message = _handlerErrorList[errCode];
                return message;
            }
            else
            {
                string removedFullStop = "";
                if (controllerErrMessage != "")
                {
                    if (processErrMessage.IndexOf(".") > 0)
                    {
                        removedFullStop = processErrMessage.Remove(processErrMessage.Length - 1);
                    }
                    else
                    {
                        removedFullStop = processErrMessage;
                    }
                    message = removedFullStop + " because " + controllerErrMessage;
                }
                else
                {
                    message = processErrMessage; // If no controller error message, just use process error message.
                }
                return message;
            }
        }

        /// <summary>
        /// Show chm help file and directly goto specific topic according to specified error code
        /// in brief format. Error code in brief format means error code without device error code, 
        /// for example: D002-B012-001 becomes D002-B012.
        /// </summary>
        /// <param name="helpFilePath"></param>
        /// <param name="errorCodeBrief"></param>
        public static void ShowErrorMessageHelp(string helpFilePath, string errorCodeBrief)
        {
            if (File.Exists(helpFilePath) == false)
            {
                ParselMessageBox.Show("CHM help file does not exist.", helpFilePath, MessageBoxIcon.Error, ErrorButton.NoButton, ErrorButton.NoButton, ErrorButton.OK);
                return;
            }
            int indexID = 0;
            if (_helpLinkList.ContainsKey(errorCodeBrief))
            {
                indexID = _helpLinkList[errorCodeBrief]; // look for topic id in the collection with respect to supplied brief error code.
                Help.ShowHelp(null, helpFilePath, HelpNavigator.TopicId, indexID.ToString());
            }
            else
            {
                Help.ShowHelp(null, helpFilePath);
            }
        }
        

        /// <summary>
        /// Get customized handler error message from specified text document and use to overwrite
        /// default handler error message to user defined handler error message.
        /// </summary>
        private static void GetCustomizedHandlerErrorMessage()
        {
            if (File.Exists(CustomizedHandlerErrorMessageSource))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(CustomizedHandlerErrorMessageSource))
                    {
                        while (!sr.EndOfStream)
                        {
                            string str = sr.ReadLine();
                            if (str.StartsWith("//") == false && str != "" && str.Contains(":") && str.Contains("-"))
                            {
                                string code = str.Remove(str.IndexOf(":"));
                                string errMes = str.Replace(code + ":", "").Trim();
                                if (_customizedHandlerErrMsgList.ContainsKey(code) == false)
                                {
                                    _customizedHandlerErrMsgList.Add(code, errMes);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ParselMessageBox.Show("Failed to load customized handler error message from file "
                        + CustomizedHandlerErrorMessageSource + "\r\n" +
                        ". Please ensure contents of the file is in correct format.",
                        ex, MessageBoxIcon.Error, ErrorButton.NoButton, ErrorButton.NoButton, ErrorButton.OK);
                }
            }
        }
    }
}
