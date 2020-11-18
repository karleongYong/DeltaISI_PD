//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/6/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace Seagate.AAS.Parsel.Services
{

    // Mediator pattern using .NET event delegates

    // -------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    public class Message : EventArgs
    {
        private string _msg;

        public Message(string msg)
        {
            this._msg = msg;
        }

        public string Text
        { 
            get{ return _msg; } 
            set{ _msg = value; } 
        }
    }
    
    // -------------------------------------------------------------------------


    /// <summary>
    /// Usage:       
    /// msgCh.MessageSentEvent += new MessageChannel.ReceiveMessageHandler(ReceiveMessage));
    /// Fire the event.     msgCh.SendMessage(null, new Message("here you go")); 
    /// </summary>
    public class MessageChannel
    {
        [Serializable]
        public delegate void ReceiveMessageHandler(object source, Message msg); 
        
        public event ReceiveMessageHandler MessageSentEvent; 
 
        // This is called to fire the event. 
        public void SendMessage(object source, Message msg) 
        { 
            if(MessageSentEvent != null) 
                MessageSentEvent(source, msg); 
        } 

        public void RegisterReceiveMessageHandler(ReceiveMessageHandler handler)
        {
            this.MessageSentEvent += handler;
        }
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Summary description for Messaging.
    /// </summary>
    public class Messaging : IService
    {
        private Hashtable _msgChannelMap = new Hashtable();

        internal Messaging()
        {
        }

        public MessageChannel GetMessageChannel(string channelID)
        {
            return (MessageChannel)_msgChannelMap[channelID];
        }

        public void InitializeService()
        {
        }
		
        public void UnloadService()
        {
        }

        public MessageChannel CreateMessageChannel(string channelID)
        {
            MessageChannel msgCh = null;

            if( _msgChannelMap.ContainsKey(channelID))
            {
                throw new ArgumentException("The channel is already created.");
            }
            else
            {
                 msgCh = new MessageChannel();
                _msgChannelMap.Add(channelID, msgCh);        
            }

            return msgCh;
        }
    }
}

  