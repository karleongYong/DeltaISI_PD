using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A notification banner that provides a series of prompts and associated buttons/actions.
    /// </summary>
    public class WizardBanner : NotifyBanner
    {
        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>
        /// The steps.
        /// </value>
        public Queue<WizardStep> Steps
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current step.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        public WizardStep CurrentStep
        {
            get
            {
                if (Steps.Count == 0)
                    return null;

                return Steps.Peek();
            }
        }

        /// <summary>
        /// Gets the previous step.
        /// </summary>
        /// <value>
        /// The previous step.
        /// </value>
        public WizardStep PreviousStep
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardBanner"/> class.
        /// </summary>
        /// <param name="steps">The steps.</param>
        public WizardBanner(IList<WizardStep> steps)
        {
            if (steps == null)
                steps = new List<WizardStep>();

            Steps = new Queue<WizardStep>(steps);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardBanner"/> class.
        /// </summary>
        /// <param name="steps">The steps.</param>
        public WizardBanner(params WizardStep[] steps)
        {
            Steps = new Queue<WizardStep>();

            foreach (WizardStep step in steps)
                Steps.Enqueue(step);

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();

            this.VisibleChanged += WizardBanner_VisibleChanged;
            this.Closing += WizardBanner_Closing;
            this.CloseText = "Cancel";
        }

        private void WizardBanner_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                UpdateStep();

                if (CurrentStep != null)
                    DoStepAction(CurrentStep.Setup);

                this.VisibleChanged -= WizardBanner_VisibleChanged;
            }
        }

        private void WizardBanner_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= WizardBanner_Closing;

            if (CurrentStep != null && CurrentStep.Cleanup != null)
                CurrentStep.Cleanup();
        }

        /// <summary>
        /// Performs the default step action and progresses to the next wizard step.
        /// </summary>
        public void ActionAndProgress()
        {
            Action action = null;
            if (CurrentStep != null && CurrentStep.Actions.Count > 0)
                action = CurrentStep.Actions.First().Item2;

            ActionAndProgress(action);
        }

        /// <summary>
        /// Performs the step action of the specified button name and progresses to the next wizard step.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        public void ActionAndProgress(string actionName)
        {
            Action action = null;
            if (CurrentStep != null && CurrentStep.Actions.Count > 0)
            {
                foreach (Tuple<string, Action> actionItem in CurrentStep.Actions)
                {
                    if (String.Equals(actionName, actionItem.Item1, StringComparison.CurrentCultureIgnoreCase))
                    {
                        action = actionItem.Item2;
                        break;
                    }
                }
            }

            ActionAndProgress(action);
        }

        private void ActionAndProgress(Action action)
        {
            if (Steps.Count == 0)
            {
                UIUtility.Invoke(this, Close);
                return;
            }

            if (CurrentStep != null)
            {
                Action cleanup = CurrentStep.Cleanup;
                Action nextSetup = null;

                PreviousStep = Steps.Dequeue();
                PreviousStep.PropertyChanged -= CurrentStep_PropertyChanged;

                if (CurrentStep != null)
                {
                    CurrentStep.PropertyChanged += CurrentStep_PropertyChanged;
                    nextSetup = CurrentStep.Setup;
                }

                DoStepAction(() =>
                {
                    if (action != null)
                        action();

                    if (cleanup != null)
                        cleanup();

                    if (CurrentStep != null)
                    {
                        UpdateStep();
                        
                        if (nextSetup != null)
                            nextSetup();
                    }
                });
            }
        }

        private void DoStepAction(Action action)
        {
            if (action == null)
                return;

            UIUtility.Invoke(this, () => this.Enabled = false);              

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                UIUtility.Invoke(this, () =>
                {
                    this.Enabled = true;

                    if (task.IsFaulted)
                    {
                        Exception ex = task.Exception;

                        while (ex is AggregateException)
                            ex = ex.InnerException;

                        this.Close();
                        Notify.BannerError("Wizard Failed", ex.ToString());
                    }
                    else if (CurrentStep == null)
                    {
                        this.Close();
                    }
                });
            });
        }

        private void CurrentStep_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateStep();
        }

        private void UpdateStep()
        {
            if (Steps.Count == 0)
                return;

            UIUtility.Invoke(this, () =>
            {
                if (CurrentStep != null)
                    this.Message = CurrentStep.Instructions;

                this.ClearActions();

                if (CurrentStep != Steps.Last())
                    this.CloseText = "Cancel";

                if (CurrentStep.Actions.Count <= 1)
                {
                    if (CurrentStep.Actions.Count == 0)
                    {
                        if (CurrentStep != Steps.Last())
                            this.AddAction("Next", null);
                        else
                            this.CloseText = "Done";
                    }
                    else
                    {
                        Tuple<string, Action> actionAndName = CurrentStep.Actions[0];

                        if (actionAndName.Item2 == null && CurrentStep == Steps.Last())
                        {
                            this.CloseText = "Done";
                        }
                        else
                        {
                            string actionText = "Next";

                            if (!String.IsNullOrEmpty(actionAndName.Item1))
                                actionText = actionAndName.Item1;
                            if (CurrentStep == Steps.Last())
                                actionText = "OK";

                            this.AddAction(actionText, () => ActionAndProgress(actionAndName.Item2));
                        }
                    }
                }
                else
                {
                    foreach (Tuple<string, Action> actionAndName in CurrentStep.Actions)
                    {
                        string actionText = "Next";

                        if (!String.IsNullOrEmpty(actionAndName.Item1))
                            actionText = actionAndName.Item1;

                        this.AddAction(actionText, () => ActionAndProgress(actionAndName.Item2));
                    }
                }


            });
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // WizardBanner
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.Name = "WizardBanner";
            this.ResumeLayout(false);
        }
    }
}