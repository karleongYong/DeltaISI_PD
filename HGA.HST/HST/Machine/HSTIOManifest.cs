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
using Seagate.AAS.Parsel.Hw;
using Aerotech.A3200.Information;

namespace Seagate.AAS.HGA.HST.Machine
{
    public class HSTIOManifest : IOManifest
    {
        public enum MechModules
        {
            Module1,
            Module2,
            Module3,
            Module4,
            Module5,
            Module6,
            Module7
        }
        public enum Cameras
        {
            InputCamera = 0,
            FiducialCamera = 1,
            OutputCamera = 2,
            

        }
        public enum VisionApp
        {
            InputInspection = 0,
            FiducialInspection = 1,
            OutputInspection = 2,
            

        }
        public enum DigitalInputs
        {
            //(Set 1)
            //X 
            Electronics_Output_1,//0
            Electronics_Output_2,
            Electronics_Output_3,
            Electronics_Output_4,
            Output_Turn_Station_Exit_Clear,
            Output_Turn_Station_At_90_Deg,
            Output_Turn_Station_At_0_Deg,
            Input_Conveyor_Position_On,//7
            //Set (2)
            //X 
            Ground_Master,//8
            Ventilation_Fan_1,
            Ventilation_Fan_2,
            Ventilation_Fan_3,
            NOT_USED_1,//skip by aerotech itself
            NOT_USED_2,//skip by aerotech itself
            NOT_USED_3,//skip by aerotech itself
            NOT_USED_4,//skip by aerotech itself//15

            //Set (3)
            //X 
            NOT_USED_5,//16
            NOT_USED_6,
            NOT_USED_7,
            NOT_USED_8,
            Input_Carrier_Clamp_Sensor,
            Output_Carrier_Clamp_Sensor,
            NOT_USED_9,
            NOT_USED_12,//23

            //(Set 4)
            //Y 
            BIS_Position_On,//24
            BOS_Position_On,
            BBZ_Position_On,
            NOT_USED_13,
            Input_Turn_Station_At_90_Deg,
            Input_Turn_Station_At_0_Deg,
            Input_Turn_Station_In_Position,
            Output_Turn_Station_In_Position,//31
            //EMO_Sense_Input,

            //(Set 5)
            //Theta
            Input_Stopper_Up,//32
            Input_Stopper_Down,
            Input_Lifter_Up,//34
            Input_Lifter_Down,
            Output_Stopper_Up,
            Output_Stopper_Down,
            Output_Lifter_Up,
            Output_Lifter_Down,

            //(Set 6)
            //Z1
            Input_CS_Deploy,
            Input_CS_Retract,
            Input_CS_Lock,
            Input_CS_Unlock,
            Output_CS_Deploy,
            Output_CS_Retract,
            Output_CS_Lock,
            Output_CS_Unlock,

            //(Set 7)
            //Z2
            PN_VS_1,
            PN_VS_2,
            PN_VS_3,
            PN_VS_4,
            PN_VS_5,
            PN_VS_6,
            PN_VS_7,
            EMO_Sense_Input,

            //(Set 8)
            //Z3
            Input_EE_VS1,
            Input_EE_VS2,
            Input_EE_Flattener_Up,
            Input_EE_Flattener_Down,
            Output_EE_VS1,
            Output_EE_VS2,
            Input_Lifter_Carrier_Sense,
            Output_Lifter_Carrier_Sense,
        }

        public enum AnalogInputs
        {
            FRL_Switch,
            FRL_Switch_Minus
        }

        public enum DigitalOutputs
        {
            //(Set 1)
            //X
            Input_EE_VCH,
            Input_EE_PCH,
            Output_EE_VCH,
            Output_EE_PCH,
            NOT_USED_1,
            NOT_USED_2,
            NOT_USED_3,
            NOT_USED_4,

            //(Set 2)
            Output_CS_Deploy,
            Output_CS_Rotate,
            Input_EE_Flattener,
            Input_Conveyor_Inhibit,
            NOT_USED_5,//skip by aerotech itself
            NOT_USED_6,//skip by aerotech itself
            NOT_USED_7,//skip by aerotech itself
            NOT_USED_8,//skip by aerotech itself

            //(Set 3)
            DC_Servicing_Light,
            Camera_1,
            Camera_3,
            Camera_2,
            Electronics_Input_1,
            Electronics_Input_2,
            Electronics_Input_3,
            Electronics_Input_4,

            //(Set 4)
            //Y
            CIS_Inhibit,
            COS_Inhibit,
            Input_Turn_Station_Inhibit,
            Input_Turn_Station_Turn_To_90_Deg,
            Input_Turn_Station_Turn_To_0_Deg,
            Output_Turn_Station_Inhibit,
            Output_Turn_Station_Turn_To_90_Deg,
            Output_Turn_Station_Turn_To_0_Deg,

            //(Set 5)
            //Theta
            Input_Stopper,
            Input_Lifter_Up,
            Input_Lifter_Down,
            Output_Stopper,
            Output_Lifter_Up,
            Output_Lifter_Down,
            Input_CS_Deploy,
            Input_CS_Rotate,

            //(Set 6)
            //Z1
            PN_VTS_1,
            PN_VTS_2,
            PN_VTS_3,
            PN_VTS_4,
            PN_VTS_5,
            PN_VTS_6,
            PN_VTS_7,
            Soft_Start_Up,

            //(Set 7)
            //Z2
            PN_VCH_1,
            PN_PCH_1,
            PN_VCH_2,
            PN_PCH_2,
            PN_VCH_3,
            PN_PCH_3,
            PN_VCH_4,
            PN_PCH_4,

            //(Set 8)
            //Z3
            PN_VCH_5,
            PN_PCH_5,
            PN_VCH_6,
            PN_PCH_6,
            PN_VCH_7,
            PN_PCH_7,
            BBZ_Inhibit,
            NOT_USED_10,
        }


        public enum Axes
        {
            X,
            Y,
            Theta,
            Z1,
            Z2,
            Z3,
        }

        public enum ControllerAxesLabel
        {
            //order must be match with Axes enum
            X,
            Y,
            Z,
            U,
            A,
            B
        }

        private HSTWorkcell workcell;

        public HSTIOManifest(HSTWorkcell workcell)
            : base(HSTMachine.HwSystem)
        {
            this.workcell = workcell;
        }

        public override void RegisterIOs()
        {
            RegisterWorkcellHardware();
            FillWorkcellManifest();
        }
        // Event handlers ------------------------------------------------------
        // Internal methods ----------------------------------------------------

        /// <summary>
        /// Register hardware components
        /// </summary>
        private void RegisterWorkcellHardware()
        {

        }

        private void FillWorkcellManifest()
        {
            HSTMachine.DisplayStatus("Registering IO List");
            HSTMachine.Workcell._a3200HC.IOStore.RegisterDriveNode(Axes.X.ToString(), ComponentType.NdriveCP, 0, 24, 24, 0, 0);
            HSTMachine.Workcell._a3200HC.IOStore.RegisterDriveNode(Axes.Y.ToString(), ComponentType.Npaq, 1, 8, 8, 2, 0);
            HSTMachine.Workcell._a3200HC.IOStore.RegisterDriveNode(Axes.Theta.ToString(), ComponentType.Npaq, 2, 8, 8, 0, 0);
            HSTMachine.Workcell._a3200HC.IOStore.RegisterDriveNode(Axes.Z1.ToString(), ComponentType.Npaq, 3, 8, 8, 0, 0);
            HSTMachine.Workcell._a3200HC.IOStore.RegisterDriveNode(Axes.Z2.ToString(), ComponentType.Npaq, 4, 8, 8, 0, 0);
            HSTMachine.Workcell._a3200HC.IOStore.RegisterDriveNode(Axes.Z3.ToString(), ComponentType.Npaq, 5, 8, 8, 0, 0);

            ///////////////////////////////////////////////////////////////////////////////////////
            //Input Session : Start
            ///////////////////////////////////////////////////////////////////////////////////////
            //X-axis TB204 (Ndrive CP)
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 0), (int)DigitalInputs.Electronics_Output_1, DigitalInputs.Electronics_Output_1.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 1), (int)DigitalInputs.Electronics_Output_2, DigitalInputs.Electronics_Output_2.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 2), (int)DigitalInputs.Electronics_Output_3, DigitalInputs.Electronics_Output_3.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 3), (int)DigitalInputs.Electronics_Output_4, DigitalInputs.Electronics_Output_4.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 4), (int)DigitalInputs.Output_Turn_Station_Exit_Clear, DigitalInputs.Output_Turn_Station_Exit_Clear.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 5), (int)DigitalInputs.Output_Turn_Station_At_90_Deg, DigitalInputs.Output_Turn_Station_At_90_Deg.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 6), (int)DigitalInputs.Output_Turn_Station_At_0_Deg, DigitalInputs.Output_Turn_Station_At_0_Deg.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 7), (int)DigitalInputs.Input_Conveyor_Position_On, DigitalInputs.Input_Conveyor_Position_On.ToString().Replace("_", " "));

            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 8), (int)DigitalInputs.Ground_Master, DigitalInputs.Ground_Master.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 9), (int)DigitalInputs.Ventilation_Fan_1, DigitalInputs.Ventilation_Fan_1.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 10), (int)DigitalInputs.Ventilation_Fan_2, DigitalInputs.Ventilation_Fan_2.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 11), (int)DigitalInputs.Ventilation_Fan_3, DigitalInputs.Ventilation_Fan_3.ToString().Replace("_", " "));

            //bit 12 to 15 (zero based) is skipped at aerotech itself.....due to unknown reason. "It is designed that way" is the answer from aerotech ppl. 
            //so.... because we are using enum, progreammingly it have to treat as a bit, so it should display to "not looks weird" (user may ask where is 12 and so). so we display as "not used". but it is not a spare. it soly for displaying nicely only
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 12), (int)DigitalInputs.NOT_USED_1, DigitalInputs.NOT_USED_1.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 13), (int)DigitalInputs.NOT_USED_2, DigitalInputs.NOT_USED_2.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 14), (int)DigitalInputs.NOT_USED_3, DigitalInputs.NOT_USED_3.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 15), (int)DigitalInputs.NOT_USED_4, DigitalInputs.NOT_USED_4.ToString().Replace("_", " "));

            //X-axis TB205 (Ndrive CP)
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 16), (int)DigitalInputs.NOT_USED_5, DigitalInputs.NOT_USED_5.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 17), (int)DigitalInputs.NOT_USED_6, DigitalInputs.NOT_USED_6.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 18), (int)DigitalInputs.NOT_USED_7, DigitalInputs.NOT_USED_7.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 19), (int)DigitalInputs.NOT_USED_8, DigitalInputs.NOT_USED_8.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 20), (int)DigitalInputs.Input_Carrier_Clamp_Sensor, DigitalInputs.Input_Carrier_Clamp_Sensor.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 21), (int)DigitalInputs.Output_Carrier_Clamp_Sensor, DigitalInputs.Output_Carrier_Clamp_Sensor.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 22), (int)DigitalInputs.NOT_USED_9, DigitalInputs.NOT_USED_9.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.X, 23), (int)DigitalInputs.NOT_USED_12, DigitalInputs.NOT_USED_12.ToString().Replace("_", " "));

            //Npaq-MR
            //Y-axis 
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 0), (int)DigitalInputs.BIS_Position_On, DigitalInputs.BIS_Position_On.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 1), (int)DigitalInputs.BOS_Position_On, DigitalInputs.BOS_Position_On.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 2), (int)DigitalInputs.BBZ_Position_On, DigitalInputs.BBZ_Position_On.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 3), (int)DigitalInputs.NOT_USED_13, DigitalInputs.NOT_USED_13.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 4), (int)DigitalInputs.Input_Turn_Station_At_90_Deg, DigitalInputs.Input_Turn_Station_At_90_Deg.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 5), (int)DigitalInputs.Input_Turn_Station_At_0_Deg, DigitalInputs.Input_Turn_Station_At_0_Deg.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 6), (int)DigitalInputs.Input_Turn_Station_In_Position, DigitalInputs.Input_Turn_Station_In_Position.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Y, 7), (int)DigitalInputs.Output_Turn_Station_In_Position, DigitalInputs.Output_Turn_Station_In_Position.ToString().Replace("_", " "));

            RegisterAnalogInput(HSTMachine.Workcell._a3200HC.IOStore.GetAnalogInput((uint)Axes.Y, 0), (int)AnalogInputs.FRL_Switch, AnalogInputs.FRL_Switch.ToString().Replace("_", " "));
            RegisterAnalogInput(HSTMachine.Workcell._a3200HC.IOStore.GetAnalogInput((uint)Axes.Y, 1), (int)AnalogInputs.FRL_Switch_Minus, AnalogInputs.FRL_Switch_Minus.ToString().Replace("_", " "));

            //Theta-axis
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 0), (int)DigitalInputs.Input_Stopper_Up, DigitalInputs.Input_Stopper_Up.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 1), (int)DigitalInputs.Input_Stopper_Down, DigitalInputs.Input_Stopper_Down.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 2), (int)DigitalInputs.Input_Lifter_Up, DigitalInputs.Input_Lifter_Up.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 3), (int)DigitalInputs.Input_Lifter_Down, DigitalInputs.Input_Lifter_Down.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 4), (int)DigitalInputs.Output_Stopper_Up, DigitalInputs.Output_Stopper_Up.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 5), (int)DigitalInputs.Output_Stopper_Down, DigitalInputs.Output_Stopper_Down.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 6), (int)DigitalInputs.Output_Lifter_Up, DigitalInputs.Output_Lifter_Up.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Theta, 7), (int)DigitalInputs.Output_Lifter_Down, DigitalInputs.Output_Lifter_Down.ToString().Replace("_", " "));

            //Z1-axis
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 0), (int)DigitalInputs.Input_CS_Deploy, DigitalInputs.Input_CS_Deploy.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 1), (int)DigitalInputs.Input_CS_Retract, DigitalInputs.Input_CS_Retract.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 2), (int)DigitalInputs.Input_CS_Lock, DigitalInputs.Input_CS_Lock.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 3), (int)DigitalInputs.Input_CS_Unlock, DigitalInputs.Input_CS_Unlock.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 4), (int)DigitalInputs.Output_CS_Deploy, DigitalInputs.Output_CS_Deploy.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 5), (int)DigitalInputs.Output_CS_Retract, DigitalInputs.Output_CS_Retract.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 6), (int)DigitalInputs.Output_CS_Lock, DigitalInputs.Output_CS_Lock.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z1, 7), (int)DigitalInputs.Output_CS_Unlock, DigitalInputs.Output_CS_Unlock.ToString().Replace("_", " "));

            //Z2-axis
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 0), (int)DigitalInputs.PN_VS_1, DigitalInputs.PN_VS_1.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 1), (int)DigitalInputs.PN_VS_2, DigitalInputs.PN_VS_2.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 2), (int)DigitalInputs.PN_VS_3, DigitalInputs.PN_VS_3.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 3), (int)DigitalInputs.PN_VS_4, DigitalInputs.PN_VS_4.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 4), (int)DigitalInputs.PN_VS_5, DigitalInputs.PN_VS_5.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 5), (int)DigitalInputs.PN_VS_6, DigitalInputs.PN_VS_6.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 6), (int)DigitalInputs.PN_VS_7, DigitalInputs.PN_VS_7.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z2, 7), (int)DigitalInputs.EMO_Sense_Input, DigitalInputs.EMO_Sense_Input.ToString().Replace("_", " "));

            //Z3-axis
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 0), (int)DigitalInputs.Input_EE_VS1, DigitalInputs.Input_EE_VS1.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 1), (int)DigitalInputs.Input_EE_VS2, DigitalInputs.Input_EE_VS2.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 2), (int)DigitalInputs.Input_EE_Flattener_Up, DigitalInputs.Input_EE_Flattener_Up.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 3), (int)DigitalInputs.Input_EE_Flattener_Down, DigitalInputs.Input_EE_Flattener_Down.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 4), (int)DigitalInputs.Output_EE_VS1, DigitalInputs.Output_EE_VS1.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 5), (int)DigitalInputs.Output_EE_VS2, DigitalInputs.Output_EE_VS2.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 6), (int)DigitalInputs.Input_Lifter_Carrier_Sense, DigitalInputs.Input_Lifter_Carrier_Sense.ToString().Replace("_", " "));
            RegisterDigitalInput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalInput((uint)Axes.Z3, 7), (int)DigitalInputs.Output_Lifter_Carrier_Sense, DigitalInputs.Output_Lifter_Carrier_Sense.ToString().Replace("_", " "));
                 
            ///////////////////////////////////////////////////////////////////////////////////////
            //Input Session : End
            ///////////////////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////////////////////
            //Output Session : Start
            ///////////////////////////////////////////////////////////////////////////////////////
            //X-axis TB202 (Ndrive CP)
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 0), (int)DigitalOutputs.Input_EE_VCH, DigitalOutputs.Input_EE_VCH.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 1), (int)DigitalOutputs.Input_EE_PCH, DigitalOutputs.Input_EE_PCH.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 2), (int)DigitalOutputs.Output_EE_VCH, DigitalOutputs.Output_EE_VCH.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 3), (int)DigitalOutputs.Output_EE_PCH, DigitalOutputs.Output_EE_PCH.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 4), (int)DigitalOutputs.NOT_USED_1, DigitalOutputs.NOT_USED_1.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 5), (int)DigitalOutputs.NOT_USED_2, DigitalOutputs.NOT_USED_2.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 6), (int)DigitalOutputs.NOT_USED_3, DigitalOutputs.NOT_USED_3.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 7), (int)DigitalOutputs.NOT_USED_4, DigitalOutputs.NOT_USED_4.ToString().Replace("_", " "));

            //X-axis J104 (Ndrive CP)
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 8), (int)DigitalOutputs.Output_CS_Deploy, DigitalOutputs.Output_CS_Deploy.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 9), (int)DigitalOutputs.Output_CS_Rotate, DigitalOutputs.Output_CS_Rotate.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 10), (int)DigitalOutputs.Input_EE_Flattener, DigitalOutputs.Input_EE_Flattener.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 11), (int)DigitalOutputs.Input_Conveyor_Inhibit, DigitalOutputs.Input_Conveyor_Inhibit.ToString().Replace("_", " "));

            //bit 12 to 15 (zero based) is skipped at aerotech itself.....due to unknown reason. "It is designed that way" is the answer from aerotech ppl. 
            //so.... because we are using enum, progreammingly it have to treat as a bit, so it should display to "not looks weird" (user may ask where is 12 and so). so we display as "not used". but it is not a spare. it soly for displaying nicely only
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 12), (int)DigitalOutputs.NOT_USED_5, DigitalInputs.NOT_USED_5.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 13), (int)DigitalOutputs.NOT_USED_6, DigitalInputs.NOT_USED_6.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 14), (int)DigitalOutputs.NOT_USED_7, DigitalInputs.NOT_USED_7.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 15), (int)DigitalOutputs.NOT_USED_8, DigitalInputs.NOT_USED_8.ToString().Replace("_", " "));


            //X-axis TB203 (Ndrive CP)
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 16), (int)DigitalOutputs.DC_Servicing_Light, DigitalOutputs.DC_Servicing_Light.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 17), (int)DigitalOutputs.Camera_1, DigitalOutputs.Camera_1.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 18), (int)DigitalOutputs.Camera_3, DigitalOutputs.Camera_3.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 19), (int)DigitalOutputs.Camera_2, DigitalOutputs.Camera_2.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 20), (int)DigitalOutputs.Electronics_Input_1, DigitalOutputs.Electronics_Input_1.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 21), (int)DigitalOutputs.Electronics_Input_2, DigitalOutputs.Electronics_Input_2.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 22), (int)DigitalOutputs.Electronics_Input_3, DigitalOutputs.Electronics_Input_3.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.X, 23), (int)DigitalOutputs.Electronics_Input_4, DigitalOutputs.Electronics_Input_4.ToString().Replace("_", " "));

            //Y-axis 
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 0), (int)DigitalOutputs.CIS_Inhibit, DigitalOutputs.CIS_Inhibit.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 1), (int)DigitalOutputs.COS_Inhibit, DigitalOutputs.COS_Inhibit.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 2), (int)DigitalOutputs.Input_Turn_Station_Inhibit, DigitalOutputs.Input_Turn_Station_Inhibit.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 3), (int)DigitalOutputs.Input_Turn_Station_Turn_To_90_Deg, DigitalOutputs.Input_Turn_Station_Turn_To_90_Deg.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 4), (int)DigitalOutputs.Input_Turn_Station_Turn_To_0_Deg, DigitalOutputs.Input_Turn_Station_Turn_To_0_Deg.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 5), (int)DigitalOutputs.Output_Turn_Station_Inhibit, DigitalOutputs.Output_Turn_Station_Inhibit.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 6), (int)DigitalOutputs.Output_Turn_Station_Turn_To_90_Deg, DigitalOutputs.Output_Turn_Station_Turn_To_90_Deg.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Y, 7), (int)DigitalOutputs.Output_Turn_Station_Turn_To_0_Deg, DigitalOutputs.Output_Turn_Station_Turn_To_0_Deg.ToString().Replace("_", " "));

            //Theta-axis
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 0), (int)DigitalOutputs.Input_Stopper, DigitalOutputs.Input_Stopper.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 1), (int)DigitalOutputs.Input_Lifter_Up, DigitalOutputs.Input_Lifter_Up.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 2), (int)DigitalOutputs.Input_Lifter_Down, DigitalOutputs.Input_Lifter_Down.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 3), (int)DigitalOutputs.Output_Stopper, DigitalOutputs.Output_Stopper.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 4), (int)DigitalOutputs.Output_Lifter_Up, DigitalOutputs.Output_Lifter_Up.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 5), (int)DigitalOutputs.Output_Lifter_Down, DigitalOutputs.Output_Lifter_Down.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 6), (int)DigitalOutputs.Input_CS_Deploy, DigitalOutputs.Input_CS_Deploy.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Theta, 7), (int)DigitalOutputs.Input_CS_Rotate, DigitalOutputs.Input_CS_Rotate.ToString().Replace("_", " "));

            //Z1-axis
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 0), (int)DigitalOutputs.PN_VTS_1, DigitalOutputs.PN_VTS_1.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 1), (int)DigitalOutputs.PN_VTS_2, DigitalOutputs.PN_VTS_2.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 2), (int)DigitalOutputs.PN_VTS_3, DigitalOutputs.PN_VTS_3.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 3), (int)DigitalOutputs.PN_VTS_4, DigitalOutputs.PN_VTS_4.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 4), (int)DigitalOutputs.PN_VTS_5, DigitalOutputs.PN_VTS_5.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 5), (int)DigitalOutputs.PN_VTS_6, DigitalOutputs.PN_VTS_6.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 6), (int)DigitalOutputs.PN_VTS_7, DigitalOutputs.PN_VTS_7.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z1, 7), (int)DigitalOutputs.Soft_Start_Up, DigitalOutputs.Soft_Start_Up.ToString().Replace("_", " "));
            
            //Z2-axis
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 0), (int)DigitalOutputs.PN_VCH_1, DigitalOutputs.PN_VCH_1.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 1), (int)DigitalOutputs.PN_PCH_1, DigitalOutputs.PN_PCH_1.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 2), (int)DigitalOutputs.PN_VCH_2, DigitalOutputs.PN_VCH_2.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 3), (int)DigitalOutputs.PN_PCH_2, DigitalOutputs.PN_PCH_2.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 4), (int)DigitalOutputs.PN_VCH_3, DigitalOutputs.PN_VCH_3.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 5), (int)DigitalOutputs.PN_PCH_3, DigitalOutputs.PN_PCH_3.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 6), (int)DigitalOutputs.PN_VCH_4, DigitalOutputs.PN_VCH_4.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z2, 7), (int)DigitalOutputs.PN_PCH_4, DigitalOutputs.PN_PCH_4.ToString().Replace("_", " "));
            
            //Z3-axis
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 0), (int)DigitalOutputs.PN_VCH_5, DigitalOutputs.PN_VCH_5.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 1), (int)DigitalOutputs.PN_PCH_5, DigitalOutputs.PN_PCH_5.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 2), (int)DigitalOutputs.PN_VCH_6, DigitalOutputs.PN_VCH_6.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 3), (int)DigitalOutputs.PN_PCH_6, DigitalOutputs.PN_PCH_6.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 4), (int)DigitalOutputs.PN_VCH_7, DigitalOutputs.PN_VCH_7.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 5), (int)DigitalOutputs.PN_PCH_7, DigitalOutputs.PN_PCH_7.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 6), (int)DigitalOutputs.BBZ_Inhibit, DigitalOutputs.BBZ_Inhibit.ToString().Replace("_", " "));
            RegisterDigitalOutput(HSTMachine.Workcell._a3200HC.IOStore.GetDigitalOutput((uint)Axes.Z3, 7), (int)DigitalOutputs.NOT_USED_10, DigitalOutputs.NOT_USED_10.ToString().Replace("_", " "));

            ///////////////////////////////////////////////////////////////////////////////////////
            //Output Session : End
            ///////////////////////////////////////////////////////////////////////////////////////
            
            HSTMachine.DisplayStatus("Registering Motor Axis");
            RegisterAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)Axes.X), (int)Axes.X, Axes.X.ToString().Replace("_", " "));
            RegisterAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)Axes.Y), (int)Axes.Y, Axes.Y.ToString().Replace("_", " "));
            RegisterAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)Axes.Theta), (int)Axes.Theta, Axes.Theta.ToString().Replace("_", " ")); 
            RegisterAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)Axes.Z1), (int)Axes.Z1, Axes.Z1.ToString().Replace("_", " "));            
            RegisterAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)Axes.Z2), (int)Axes.Z2, Axes.Z2.ToString().Replace("_", " "));            
            RegisterAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)Axes.Z3), (int)Axes.Z3, Axes.Z3.ToString().Replace("_", " "));                             
        }
    }
}
