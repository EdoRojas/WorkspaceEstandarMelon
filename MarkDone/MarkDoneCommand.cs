using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.LogicaAPI;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones;
using Genesyslab.Desktop.Modules.Outbound.Model.Campaign;
using Genesyslab.Platform.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.MarkDone
{
    class MarkDoneCommand : IElementOfCommand
    {
        readonly IObjectContainer container;
        ILogger log;
        private static LogicaNegocio _metodos = new LogicaNegocio();
        private ICase _caseIdentifier;
        public object Context { get; set; }
        MetodoApi _metodoApi = new MetodoApi();
        TerminoInteraccion termino = new TerminoInteraccion();
        public string DialMode = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeReleaseCallCommand"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public MarkDoneCommand(IObjectContainer container)
        {
            this.container = container;

            // Initialize the trace system
            this.log = container.Resolve<ILogger>();

            // Create a child trace section
            this.log = log.CreateChildLogger("MarkDoneLog");


            var contextDictionary = Context as IDictionary<string, object>;

            object caseObject;

            if (contextDictionary != null && contextDictionary.TryGetValue("Case", out caseObject))
            {
                var theCase = caseObject as ICase;
                _caseIdentifier = theCase;
                
            }

        }

        /// <summary>
        /// Gets the name of the command. This is optional.
        /// </summary>
        /// <value>The command name.</value>
        public string Name { get { return "MarkDoneTipificado"; } set { } }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        public bool Execute(IDictionary<string, object> parameters, IProgressUpdater progress)
        {
            // To go to the main thread
           

            if (Application.Current.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
            {
                object result = Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new ExecuteDelegate(Execute), parameters, progress);
                return (bool)result;
            }
            else
            {
                // Ok, we are in the main thread
                log.Info("Execute");
                IInteraction interactionVoice = parameters["InteractionMarkDone0"] as IInteraction;
                var inx = InteraccionGenesys.GetAttachedData(interactionVoice);

                // Get the parameter                

                
                if (interactionVoice != null)
                {
                    var tipoInteraccion = interactionVoice.Type;
                    switch (tipoInteraccion)
                    {
                        case "InteractionVoice":

                            termino.CaseId = interactionVoice.CaseId;
                            termino.Agentdbid = InteractionExtensionCrossnetModule.AgenteConfig != null ?
                                                InteractionExtensionCrossnetModule.AgenteConfig.DBID : 0;
                            
                            inx.genEventoMarkdone = "InteractionVoice";

                            inx.IdMarkdone = interactionVoice.EntrepriseInteractionCurrent.Id;
                            break;
                        case "InteractionPullPreview":
                            var item = interactionVoice.OutboundChainRecord.Records.FirstOrDefault();
                            inx.genEventoMarkdone = "PreviewRecord";
                            inx.IdMarkdone = item.CallAttemptGUID;

                            termino.CaseId = interactionVoice.CaseId;
                            termino.Agentdbid = InteractionExtensionCrossnetModule.AgenteConfig != null ?
                                                InteractionExtensionCrossnetModule.AgenteConfig.DBID : 0;
                            
                            inx.genEventoMarkdone = "PreviewRecord";

                            break;
                        case "InteractionCallback":
                            termino.CaseId = interactionVoice.CaseId;
                            termino.Agentdbid = InteractionExtensionCrossnetModule.AgenteConfig != null ?
                                                InteractionExtensionCrossnetModule.AgenteConfig.DBID : 0;

                            inx.genEventoMarkdone = "InteractionCallback";

                            inx.IdMarkdone = interactionVoice.EntrepriseInteractionCurrent.Id;

                            break;
                    }
                    if (InteractionExtensionCrossnetModule.TablaTipificaciones == null)
                    {
                        return false;
                    }

                    EResultado resultado = null;

                    _metodoApi.TerminoInteraccion(out resultado, termino, InteractionExtensionCrossnetModule.token);

                    DataRow resultMarkDone = null;
                    switch (tipoInteraccion)
                    {
                        case "InteractionVoice":
                            inx.genEventoMarkdone = "InteractionVoice";
                            inx.IdMarkdone = interactionVoice.EntrepriseInteractionCurrent.Id;
                            resultMarkDone = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + inx.IdMarkdone +
                                                                                              "' and Agente ='" + inx.GenAgente + "'").FirstOrDefault();

                            break;
                        case "InteractionPullPreview":
                            var item = interactionVoice.OutboundChainRecord.Records.FirstOrDefault();
                            inx.genEventoMarkdone = "PreviewRecord";
                            inx.IdMarkdone = item.CallAttemptGUID;
                            resultMarkDone = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + inx.IdMarkdone +
                                                                                              "' and Agente ='" + inx.GenAgente + "'").FirstOrDefault();

                            break;
                        case "InteractionCallback":
                            inx.genEventoMarkdone = "InteractionCallback";
                            inx.IdMarkdone = interactionVoice.EntrepriseInteractionCurrent.Id;
                            resultMarkDone = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + inx.IdMarkdone +
                                                                                              "' and Agente ='" + inx.GenAgente + "'").FirstOrDefault();

                            break;
                    }
                    string Estado = resultMarkDone["Estado"] != null ? resultMarkDone["Estado"].ToString() : "";
                    if (!Estado.Equals("TIPIFICADO"))
                    {
                        GenesysAlert.SendMessage("Favor tipificar", "Public", SeverityType.Warning);
                        return true;
                    }
                    else
                    {
                        switch (tipoInteraccion)
                        {
                            case "InteractionVoice":
                                InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + inx.IdMarkdone +
                                                                                            "' and Agente ='" + inx.GenAgente + "'").FirstOrDefault().Delete();
                                break;
                            case "InteractionPullPreview":
                                var item = interactionVoice.OutboundChainRecord.Records.FirstOrDefault();
                                InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + inx.IdMarkdone +
                                                                                            "' and Agente ='" + inx.GenAgente + "'").FirstOrDefault().Delete();

                                break;
                            case "InteractionCallback":
                                InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + inx.IdMarkdone +
                                                                                            "' and Agente ='" + inx.GenAgente + "'").FirstOrDefault().Delete();
                                break;
                        }
                    }
                    
                }
           
                return false;
            }
        }
        /// <summary>
        /// This delegate allows to go to the main thread.
        /// </summary>
        delegate bool ExecuteDelegate(IDictionary<string, object> parameters, IProgressUpdater progressUpdater);
    }
}

