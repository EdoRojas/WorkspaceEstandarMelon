using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.Windows.Event;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    /// <summary>
    ///     Interaction logic for TipificacionesButtonView.xaml
    /// </summary>
    public partial class TipificacionesButtonView : UserControl, ITipificacionesButtonView
    {
        private readonly IObjectContainer _container;
        private readonly IViewEventManager _viewEventManager;

        public TipificacionesButtonView(ITipificacionesViewModel viewModel, IObjectContainer container,
            IViewEventManager viewEventManager)
        {
            _container = container;
            _viewEventManager = viewEventManager;
            Model = viewModel;

            InitializeComponent();

            Width = double.NaN;
            Height = double.NaN;
        }

        public ITipificacionesViewModel Model
        {
            get { return DataContext as ITipificacionesViewModel; }
            set { DataContext = value; }
        }

        public object Context { get; set; }

        public void Create()
        {
            Model.Case = (Context as IDictionary<string, object>).TryGetValue("Case") as ICase;

            _viewEventManager.Subscribe(ActionEventHandler);
        }

        public void Destroy()
        {
            _viewEventManager.Unsubscribe(ActionEventHandler);

            Model.Case = null;
        }

        public void ActionEventHandler(object eventObject)
        {
            if ((Application.Current.Dispatcher != null) && !Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<object>(ActionEventHandler),
                    eventObject);
            else
            {
                var contactEvent = eventObject as GenericEvent;

                if ((contactEvent != null) && (contactEvent.Context == Model.Case.CaseId) &&
                    (contactEvent.Target == GenericContainerView.ContainerView))
                    foreach (var contactAction in contactEvent.Action)
                    {
                        var objectSimpleAction = contactAction.Action as string;

                        switch (objectSimpleAction)
                        {
                            case ActionGenericContainerView.UserControlLoaded:
                                SplitToggleButton.IsChecked = ((Visibility)contactAction.Parameters[0] ==
                                                               Visibility.Visible) &&
                                                              (contactAction.Parameters[1] as string ==
                                                               "InteractionTipificaciones");
                                break;

                            default:
                                break;
                        }
                    }
            }
        }

        private void splitToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Lock MinSize
            _viewEventManager.Publish(new GenericEvent
            {
                SourceId = null,
                Target = GenericContainerView.ContainerView,
                Context = Model.Case.CaseId,
                TargetId = null,
                Action = new[]
                {
                    new GenericAction
                    {
                        Action = ActionGenericContainerView.LockMinSize,
                        Parameters = new object[] {true, "InteractionContainerView"}
                    }
                }
            });

            _viewEventManager.Publish(new GenericEvent
            {
                Target = GenericContainerView.ContainerView,
                Context = Model.Case.CaseId,
                Action = new[]
                {
                    new GenericAction
                    {
                        Action = ActionGenericContainerView.ShowHidePanelRight,
                        Parameters =
                            new object[]
                            {
                                SplitToggleButton.IsChecked ?? false ? Visibility.Visible : Visibility.Collapsed,
                                "InteractionTipificaciones"
                            }
                    },
                    new GenericAction
                    {
                        Action = ActionGenericContainerView.ActivateThisPanel,
                        Parameters = new object[] {"InteractionTipificaciones"}
                    }
                }
            });

            _viewEventManager.Publish(new GenericEvent
            {
                SourceId = null,
                Target = GenericContainerView.ContainerView,
                Context = Model.Case.CaseId,
                TargetId = null,
                Action = new[]
                {
                    new GenericAction
                    {
                        Action = ActionGenericContainerView.LockMinSize,
                        Parameters = new object[] {false, "InteractionContainerView"}
                    }
                }
            });
        }
    }
}