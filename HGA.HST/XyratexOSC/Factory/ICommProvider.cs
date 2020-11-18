using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.Factory
{
    public interface ICommProvider
    {
        /// <summary>
        /// Initializes the communication provider.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Un-initializes the communication provider.
        /// </summary>
        void Uninitialize();

        /// <summary>
        /// Opens connection to Continuum system.
        /// </summary>
        /// <returns></returns>
        void Connect();

        /// <summary>
        /// Checks connection to Continuum system.
        /// </summary>
        /// <returns></returns>
        void CheckConnection();

        /// <summary>
        /// Closes connection to Continuum system.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Attempts to establish communication with the factory host.
        /// </summary>
        /// <returns></returns>
        void AttemptOnline();

        /// <summary>
        /// Disables communication with the factory host.
        /// </summary>
        /// <returns></returns>
        void Offline();

        /// <summary>
        /// Gets the tool's communication/control state.
        /// </summary>
        /// <returns>The tool's communication/control state.</returns>
        ToolState GetToolState();

        /// <summary>
        /// Gets the Host's communication state as determined from the last GEM box heartbeat.
        /// </summary>
        /// <returns>The Host's communication state.</returns>
        HostState GetHostState();

        /// <summary>
        /// Sends the specified result file to the facility server.
        /// </summary>
        /// <param name="filepath">The file path of the result file.</param>
        void SendResultFile(string filepath);

        /// <summary>
        /// Occurs when a message is sent to the facility server.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageSent;

        /// <summary>
        /// Occurs when a message is received from the facility server.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when the recipe (process program) is downloaded and ready on the Equipment computer.
        /// </summary>
        event EventHandler<RecipeEventArgs> RecipeReady;

        event EventHandler<ProcessRunEventArgs> RemoteCommandRun;

        /// <summary>
        /// Occurs when the Host has changed any variable (status, equipment constant, etc)
        /// </summary>
        event EventHandler<GemVariableEventArgs> VariableChanged;

        /// <summary>
        /// Raises the collection event specified by the CEID.
        /// </summary>
        /// <param name="ceid">The CEID.</param>
        /// <param name="instanceId">The instance identifier associated with the event.</param>
        void RaiseEvent(int ceid, int instanceId);

        /// <summary>
        /// Adds an alarm GEM item.
        /// </summary>
        /// <param name="alarm">The alarm.</param>
        void AddAlarm(GemAlarm alarm);

        /// <summary>
        /// Sets the alarm state and raises the associated alarm collection event.
        /// </summary>
        /// <param name="aid">The alarm ID.</param>
        /// <param name="ceid">The collection event ID.</param>
        void RaiseAlarm(int aid, int ceid);

        /// <summary>
        /// Sets the alarm state and raises the associated alarm collection event.
        /// </summary>
        /// <param name="aid">The alarm ID.</param>
        /// <param name="ceid">The collection event ID.</param>
        void ClearAlarm(int aid, int ceid);

        /// <summary>
        /// Adds a variable GEM item.
        /// </summary>
        /// <param name="var">The variable.</param>
        void AddVariable(GemVariable var);

        /// <summary>
        /// Sets the value of the variable with the specified variable ID.
        /// </summary>
        /// <param name="vid">The variable ID.</param>
        /// <param name="instanceId">The instance identifier used to associate with an event.</param>
        /// <param name="value">The value.</param>
        void SetVariable(int vid, int instanceId, object value);
    }
}
