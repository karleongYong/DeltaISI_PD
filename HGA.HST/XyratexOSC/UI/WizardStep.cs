using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A step of the <see cref="WizardBanner"/> that includes a prompt, and button/action options.
    /// </summary>
    public class WizardStep : INotifyPropertyChanged
    {
        private IList<Tuple<string, Action>> _actions = new List<Tuple<string, Action>>();
        private Action _setup;
        private Action _cleanup;
        private string _instructions;

        /// <summary>
        /// Gets or sets the action to be performed when this step is entered.
        /// </summary>
        /// <value>
        /// The setup action.
        /// </value>
        public Action Setup
        {
            get
            {
                return _setup;
            }
            set
            {
                if (_setup == value)
                    return;

                _setup = value;

                NotifyPropertyChanged("Setup");
            }
        }

        /// <summary>
        /// Gets the actions available button names and associated actions that are available to select during this step prompt.
        /// </summary>
        /// <value>
        /// The prompt actions.
        /// </value>
        public IList<Tuple<string, Action>> Actions
        {
            get
            {
                return _actions;
            }
        }

        /// <summary>
        /// Gets or sets the action executed when this step is exited (regardless of which action was selected).
        /// </summary>
        /// <value>
        /// The cleanup action.
        /// </value>
        public Action Cleanup
        {
            get
            {
                return _cleanup;
            }
            set
            {
                if (_cleanup == value)
                    return;

                _cleanup = value;

                NotifyPropertyChanged("Cleanup");
            }
        }

        /// <summary>
        /// Gets or sets the prompt instructions.
        /// </summary>
        /// <value>
        /// The instructions.
        /// </value>
        public string Instructions
        {
            get
            {
                return _instructions;
            }
            set
            {
                if (_instructions == value)
                    return;

                _instructions = value;

                NotifyPropertyChanged("Instructions");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStep"/> class.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        /// <param name="setup">The setup.</param>
        /// <param name="action">The action.</param>
        /// <param name="cleanup">The cleanup.</param>
        public WizardStep(string instructions, Action setup = null, Action action = null, Action cleanup = null)
        {
            Instructions = instructions;
            Setup = setup;
            Cleanup = cleanup;
            _actions.Add(new Tuple<string, Action>("", action));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStep"/> class.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        /// <param name="setup">The setup.</param>
        /// <param name="cleanup">The cleanup.</param>
        /// <param name="nameAndActions">The name and actions of multiple action options.</param>
        public WizardStep(string instructions, Action setup = null, Action cleanup = null, params Tuple<string, Action>[] nameAndActions)
        {
            Instructions = instructions;
            Setup = setup;
            Cleanup = cleanup;
            _actions = nameAndActions;
        }

        /// <summary>
        /// Adds an action option to this step.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="action">The action.</param>
        public void AddAction(string actionName, Action action)
        {
            Tuple<string, Action> actionAndName = new Tuple<string,Action>(actionName, action);

            if (_actions == null)
                _actions = new List<Tuple<string, Action>>();

            _actions.Add(actionAndName);

            NotifyPropertyChanged("Actions");
        }

        /// <summary>
        /// Removes an action by name from this wizard step.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        public void RemoveAction(string actionName)
        {
            if (_actions == null)
                return;

            Tuple<string, Action> actionAndName = null;

            foreach (Tuple<string, Action> actionItem in _actions)
                if (String.Equals(actionName, actionItem.Item1, StringComparison.CurrentCultureIgnoreCase))
                    actionAndName = actionItem;

            if (actionAndName != null)
                _actions.Remove(actionAndName);

            NotifyPropertyChanged("Actions");
        }

        /// <summary>
        /// Clears all actions of this wizard step.
        /// </summary>
        public void ClearActions()
        {
            if (_actions == null)
                return;

            _actions.Clear();

            NotifyPropertyChanged("Actions");
        }

        /// <summary>
        /// Occurs when a property value changes in this step so the Wizard Banner can update.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;

            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}