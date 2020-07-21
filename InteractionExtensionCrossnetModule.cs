using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Database;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.LogicaAPI;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions.Email;
using Genesyslab.Desktop.Modules.Outbound.Model.Campaign;
using Genesyslab.Desktop.Modules.Windows.Event;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet
{
    public class InteractionExtensionCrossnetModule : IModule
    {
        public static DataTable TablaTipificaciones;
        public static string AreaTipificacion;
        public static  IObjectContainer _container;
        private readonly IInteractionManager _interactionManager;
        private readonly IViewEventManager _viewEventManager;
        private readonly IViewManager _viewManager;
        private static LogicaNegocio _metodos;
        public static IAgent Agente;
        public static CfgPerson AgenteConfig;        
        public static DataTable TablaInteraccion;
        public static List<Atached> listaAtached = new List<Atached>();
        public static List<FiltroResult> listaFiltro = new List<FiltroResult>();
        public static DataTable _TablaInteraccion = new DataTable("MarkDone");
        private readonly ICommandManager _commandManager;
        public static int MarkDone = 0;
        public static string token = "";
        private ICase _caseIdentifier;
        public object Context { get; set; }
       public static MetodoApi _metodoApi = new MetodoApi();

        public InteractionExtensionCrossnetModule(IObjectContainer container, IViewManager viewManager, IInteractionManager interactionManager, ICommandManager commandManager, IViewEventManager viewEventManager, ICase caseIdentifier)
        {
            _container = container;
            _viewManager = viewManager;
            _interactionManager = interactionManager;
            _viewEventManager = viewEventManager;
            _metodos = new LogicaNegocio();
            _commandManager = commandManager;
            _caseIdentifier = caseIdentifier;
        }

        public void Initialize()
        {
            _container.RegisterType<ITipificacionesView, TipificacionesView>();
            _container.RegisterType<ITipificacionesViewModel, TipificacionesViewModel>();            

            _viewManager.ViewsByRegionName["InteractionWorksheetRegion"].Add(
                new ViewActivator
                {
                    ViewType = typeof(ITipificacionesView),
                    ViewName = "InteractionTipificaciones",
                    ActivateView = true
                }
            );

            _container.RegisterType<ITipificacionesButtonView, TipificacionesButtonView>();

           
            _viewManager.ViewsByRegionName["CaseViewSideButtonRegion"].Add(
                new ViewActivator
                {
                    ViewType = typeof(ITipificacionesButtonView),
                    ViewName = "TipificacionesButtonView",
                    ActivateView = true
                }
            );

            //_commandManager.InsertCommandToChainOfCommandBefore(
            //    "BundleClose",
            //    "IsPossibleToClose",
            //    new List<CommandActivator>()
            //    {
            //        new CommandActivator()
            //        {
            //            CommandType = typeof(MarkDone.MarkDoneCommand),
            //            Name = "CloseMarkDoneTipificado"
            //        }
            //    }
            //    );

            _interactionManager.InteractionEvent += interactionManager_InteractionEvent;

            _viewEventManager.Subscribe(CrossnetEventHandler);
        }

        private void CrossnetEventHandler(object eventObject)
        {
            var eventMessage = eventObject as string;
            if (eventMessage != null)
                switch (eventMessage)
                {
                    case "Loggin":
                        try
                        {
                            EResultado error;
                            Agente = _container.Resolve<IAgent>();
                            AgenteConfig = Agente.ConfPerson;
                            var lala= Agente.MonitoredCfgPersonAgents;
                            if (Agente != null)
                            {
                                Agente agent = new Agente
                                {
                                    userName = Agente.UserName != null ?
                                               Agente.UserName : "",
                                };
                                if (AgenteConfig != null)
                                {

                                    var lala2 = AgenteConfig.GetType();
                                   agent.firstName = AgenteConfig.FirstName != null ?
                                               AgenteConfig.FirstName : "";
                                   agent.lastName = AgenteConfig.LastName != null ?
                                               AgenteConfig.LastName : "";
                                    agent.tenantDBid =147;
                                   agent.agentDBid = AgenteConfig.DBID != null ?
                                               AgenteConfig.DBID : 0;
                                   agent.EmployeeId = AgenteConfig.EmployeeID != null ?
                                                       AgenteConfig.EmployeeID : "";
                                }
                                var resultado = _metodoApi.autentificacion(out error, agent);
                                if(resultado != null)
                                 token = resultado.token;
                            }
                            var itemsLista = _metodoApi.ArbolTipificacion(out error, token);
                            if (itemsLista != null)
                            {
                                if(itemsLista.listaArbol != null)
                                  TablaTipificaciones = Utility.ListToDataTable(itemsLista.listaArbol);
                            }
                            else
                            {
                                TablaTipificaciones = null;
                            }

                            var itemsFiltro = _metodoApi.ListaFiltro(out error, token);
                            if (itemsFiltro != null)
                            {
                                listaFiltro = itemsFiltro.listaFiltro;
                            }
                            listaAtached = _metodoApi.ListAtached(out error, token);
                            _TablaInteraccion.Columns.Add("IdMarkDone");
                            _TablaInteraccion.Columns.Add("EventoMarkdone");
                            _TablaInteraccion.Columns.Add("Estado");
                            _TablaInteraccion.Columns.Add("Agente");

                        }
                        catch (Exception error)
                        {
                            GenesysAlert.SendMessage(
                                "Se ha producido un error en el login: " +
                                error.Message, "Public",
                                SeverityType.Error);
                        }


                        break;
                }
        }

        public void interactionManager_InteractionEvent(object sender, EventArgs<IInteraction> e)
        {
            var datosInteraccion = new Interaccion();
            var interaccionActiva = e.Value;
            var ultimoEventoInteraccion = interaccionActiva.EntrepriseLastInteractionEvent;
            var tipoInteraccion = interaccionActiva.Type;
            
            switch (tipoInteraccion)
            {
                case "InteractionVoice":
                    ConvierteClase(ultimoEventoInteraccion.Id, ultimoEventoInteraccion, interaccionActiva);
                    _metodos.InsertaRegistroMarkDoneModule(interaccionActiva);
                    break;
                case "InteractionPullPreview": 
                case "InteractionPushPreview":
                    ConvierteClaseCampana(interaccionActiva);
                    _metodos.InsertaRegistroMarkDoneModule(interaccionActiva);
                    break;
                case "InteractionWorkItem":
                    break;
                case "InteractionChat":
                    break;
                case "InteractionCallback":
                    ConvierteClase(ultimoEventoInteraccion.Id, ultimoEventoInteraccion, interaccionActiva);
                    _metodos.InsertaRegistroMarkDoneModule(interaccionActiva);
                    break;
                case "InteractionInboundEmail":
                    break;
                case "InteractionOutboundEmail":
                    break;
                case "whatsappsession":
                    break;
            }
        }

        //private static void GuardarInteraccionEnPullPreview(Interaccion datosInx, IInteraction inx)
        //{
        //    try
        //    {
        //        var item = inx.OutboundChainRecord.Records.FirstOrDefault();
        //        if (item != null)
        //        {
        //            XDocument Documento = new XDocument();
        //            XElement xmlOut = null;
        //            datosInx.GenRecordId = item.UserData.GetAsInt("recordid");
        //            datosInx.GenChainIdCampana = item.ChainID;
        //            datosInx.GenCampaingName = inx.OutboundChainRecord.CampaignName;
        //            datosInx.GenGuid = item.CallAttemptGUID;
        //            datosInx.GenAttempt = item.Attempts;
        //            datosInx.UserName = Agente.UserName;
        //            datosInx.GenFirstName = AgenteConfig.FirstName.ToString();
        //            datosInx.GenLastName = AgenteConfig.LastName.ToString();
        //            datosInx.GenGroupName = item.CampaignInfo.GroupName;
        //            datosInx.GenTipoInteraccion = 7;
        //            datosInx.GenCanalEntrada = item.CampaignInfo.CampaignEvent;
        //            datosInx.GenEvento = item.CampaignInfo.CampaignEvent;
        //            datosInx.GenDestino = item.PhoneNumber;

        //            datosInx.GenOrigen = inx.Agent.Place.PlaceName;
        //            datosInx.GenFechaInicioCall = DateTime.Now;

        //            datosInx.GenEstadoTipificacion = "PENDIENTE DE TIPIFICACION";
        //            datosInx.GenEstadoInteraccion = "TRAE REGISTROS DE CAMPAÑA";

        //            try
        //            {
        //                 Documento = Utility.GeneraXML(inx);
        //            }
        //            catch (Exception)
        //            {

        //            }

        //            if (Documento != null)
        //            {
        //                 xmlOut = XElement.Parse(Documento.ToString());
        //            }

        //            datosInx.fields = xmlOut;
        //            _metodos.GuardaCampana(datosInx);
        //        }

        //    }
        //    catch (Exception error)
        //    {
        //        GenesysAlert.SendMessage(
        //            "Se ha producido un error al obtener los datos de la interacción en InteractionPullPreview -> PreviewRecord: " +
        //            error.Message, "Public", SeverityType.Error);
        //    }
        //}



        //private static void GuardarInteraccionEnPushPreview(Interaccion datosInx, IInteraction inx)
        //{
        //    try
        //    {
        //        var item = inx.OutboundChainRecord.Records.FirstOrDefault();
        //        if (item != null)
        //        {
        //            datosInx.GenRecordId = item.UserData.GetAsInt("recordid");
        //            datosInx.GenChainIdCampana = item.ChainID;
        //            datosInx.GenCampaingName = inx.OutboundChainRecord.CampaignName;
        //            datosInx.GenGuid = item.CallAttemptGUID;
        //            datosInx.GenAttempt = item.Attempts;
        //            datosInx.UserName = Agente.UserName;
        //            datosInx.GenFirstName = AgenteConfig.FirstName.ToString();
        //            datosInx.GenLastName = AgenteConfig.LastName.ToString(); 
        //            datosInx.GenGroupName = item.CampaignInfo.GroupName;
        //            datosInx.GenTipoInteraccion = 6;
        //            datosInx.GenCanalEntrada = "PreviewRecord";
        //            datosInx.GenEvento = "PreviewRecord";
        //            datosInx.GenDestino = item.PhoneNumber;

        //            datosInx.GenOrigen = inx.Agent.Place.PlaceName;
        //            datosInx.GenFechaInicioCall = DateTime.Now;

        //            datosInx.GenEstadoTipificacion = "PENDIENTE DE TIPIFICACION";
        //            datosInx.GenEstadoInteraccion = "TRAE REGISTROS DE CAMPAÑA";

        //            try
        //            {
        //                XDocument Documento = Utility.GeneraXML(inx);
        //            }
        //            catch (Exception)
        //            {

        //            }


        //            _metodos.GuardaCampana(datosInx);
        //        }

        //    }
        //    catch (Exception error)
        //    {
        //        GenesysAlert.SendMessage(
        //            "Se ha producido un error al obtener los datos de la interacción en InteractionPullPreview -> PreviewRecord: " +
        //            error.Message, "Public", SeverityType.Error);
        //    }
        //}

        //private static void GuardaInteraccionEmailOutboundEnAck(Interaccion datosInx, IInteraction inx)
        //{
        //    try
        //    {
        //        var email = inx as IInteractionOutboundEmail;

        //        if (email != null)
        //        { 
        //            if (email.EntrepriseEmailInteractionCurrent.Id != null)
        //                datosInx.GenConnid = email.EntrepriseEmailInteractionCurrent.Id;
        //            if (email.EntrepriseEmailInteractionCurrent.ParentID != null)
        //                datosInx.GenParentId = email.EntrepriseEmailInteractionCurrent.ParentID;
        //            datosInx.GenTipoInteraccion = 11;
        //            datosInx.GenFechaInicioCall = email.EntrepriseEmailInteractionCurrent.StartDate;
        //            datosInx.UserName = Agente.UserName;
        //            datosInx.GenFirstName = AgenteConfig.FirstName.ToString();
        //            datosInx.GenLastName = AgenteConfig.LastName.ToString();
        //            datosInx.GenArea = AreaTipificacion;

        //            if (email.EntrepriseEmailInteractionCurrent.From != null)
        //                datosInx.GenFrom = email.EntrepriseEmailInteractionCurrent.From;
        //            if (email.EntrepriseEmailInteractionCurrent.To != null)
        //                datosInx.GenTo = string.Join(",", email.EntrepriseEmailInteractionCurrent.To);
        //            if (email.EntrepriseEmailInteractionCurrent.Bcc != null)
        //                datosInx.GenCco = string.Join(",", email.EntrepriseEmailInteractionCurrent.Bcc);

        //            var inxDuration = inx.Duration;
        //            if (inxDuration.Ticks > 0)
        //            {
        //                datosInx.GenDuracionCall = inxDuration.TotalSeconds.ToString(CultureInfo.InvariantCulture);
        //                datosInx.GenFechaTerminoCall = inx.StartDate.Add(inxDuration);
        //            }

        //            datosInx.GenEstadoInteraccion = email.EntrepriseEmailInteractionCurrent.IdType.Subtype;
        //        }
        //        datosInx.GenEstadoTipificacion = "PENDIENTE DE TIPIFICACION";
        //        _metodos.GuardaInteraccionEmail(datosInx);
        //    }
        //    catch (Exception error)
        //    {
        //        GenesysAlert.SendMessage(
        //            "Se ha producido un error al obtener los datos de la interacción en InteractionOutboundEmail -> EventAck: " +
        //            error.Message, "Public", SeverityType.Error);
        //    }
        //}

        //private static void GuardaInteraccionEmailInboundEnAck(Interaccion datosInx, IInteraction inx)
        //{
        //    try
        //    {
        //        var email = inx as IInteractionInboundEmail;
        //        if (email != null)
        //        { 
        //            if (email.EntrepriseEmailInteractionCurrent.Id != null)
        //                datosInx.GenConnid = email.EntrepriseEmailInteractionCurrent.Id;
        //            if (email.EntrepriseEmailInteractionCurrent.ParentID != null)
        //                datosInx.GenParentId = email.EntrepriseEmailInteractionCurrent.ParentID;
        //            datosInx.GenTipoInteraccion = 11;
        //            datosInx.GenFechaInicioCall = inx.StartDate;
        //            datosInx.UserName = Agente.UserName;
        //            datosInx.GenFirstName = AgenteConfig.FirstName.ToString();
        //            datosInx.GenLastName = AgenteConfig.LastName.ToString();
        //            datosInx.GenArea = AreaTipificacion;

        //            if (email.EntrepriseEmailInteractionCurrent.From != null)
        //                datosInx.GenFrom = email.EntrepriseEmailInteractionCurrent.From;
        //            if (email.EntrepriseEmailInteractionCurrent.To != null)
        //                datosInx.GenTo = string.Join(",", email.EntrepriseEmailInteractionCurrent.To);
        //            if (email.EntrepriseEmailInteractionCurrent.Bcc != null)
        //                datosInx.GenCco = string.Join(",", email.EntrepriseEmailInteractionCurrent.Bcc);

        //            var inxDuration = inx.Duration;
        //            if (inxDuration.Ticks > 0)
        //            {
        //                datosInx.GenDuracionCall = inxDuration.TotalSeconds.ToString(CultureInfo.InvariantCulture);
        //                datosInx.GenFechaTerminoCall = inx.StartDate.Add(inxDuration);
        //            }

        //            datosInx.GenEstadoInteraccion = email.EntrepriseEmailInteractionCurrent.IdType.Subtype;
        //        }
        //        datosInx.GenEstadoTipificacion = "PENDIENTE DE TIPIFICACION";
        //        _metodos.GuardaInteraccionEmail(datosInx);
        //    }
        //    catch (Exception error)
        //    {
        //        GenesysAlert.SendMessage(
        //            "Se ha producido un error al obtener los datos de la interacción en InteractionInboundEmail -> EventAck: " +
        //            error.Message, "Public", SeverityType.Error);
        //    }
        //}

        //private static void GuardaInteraccionEmailInboundEnInvite(Interaccion datosInx, IInteraction inx)
        //{
        //    try
        //    {
        //        var email = inx as IInteractionInboundEmail;

        //        if (email != null)
        //        {                    
        //            if (email.EntrepriseEmailInteractionCurrent.Id != null)
        //                datosInx.GenConnid = email.EntrepriseEmailInteractionCurrent.Id;
        //            if (email.EntrepriseEmailInteractionCurrent.ParentID != null)
        //                datosInx.GenParentId = email.EntrepriseEmailInteractionCurrent.ParentID;
        //            datosInx.GenTipoInteraccion = 5;
        //            datosInx.GenFechaInicioCall = inx.StartDate;
        //            datosInx.UserName = Agente.UserName;
        //            datosInx.GenFirstName = AgenteConfig.FirstName.ToString();
        //            datosInx.GenLastName = AgenteConfig.LastName.ToString(); 
        //            datosInx.GenArea = AreaTipificacion;

        //            if (email.EntrepriseEmailInteractionCurrent.From != null)
        //                datosInx.GenFrom = email.EntrepriseEmailInteractionCurrent.From;
        //            if (email.EntrepriseEmailInteractionCurrent.To != null)
        //                datosInx.GenTo = string.Join(",", email.EntrepriseEmailInteractionCurrent.To);
        //            if (email.EntrepriseEmailInteractionCurrent.Bcc != null)
        //                datosInx.GenCco = string.Join(",", email.EntrepriseEmailInteractionCurrent.Bcc);

        //            datosInx.GenEstadoInteraccion = email.EntrepriseEmailInteractionCurrent.IdType.Subtype;
        //        }
        //        datosInx.GenEstadoInteraccion = "INVITACION MAIL ENTRANTE";
        //        datosInx.GenEstadoTipificacion = "PENDIENTE DE TIPIFICACION";

        //        _metodos.GuardaInteraccionEmail(datosInx);
        //    }
        //    catch (Exception error)
        //    {
        //        GenesysAlert.SendMessage(
        //            "Se ha producido un error al obtener los datos de la interacción en InteractionInboundEmail -> EventInvite: " +
        //            error.Message, "Public", SeverityType.Error);
        //    }
        //}

        private static void ConvierteClaseCampana(IInteraction inx)
        {
            try
            {
                var item = inx.OutboundChainRecord.Records.FirstOrDefault();
                if (item != null)
                {
                    Campana campana = new Campana();
                    campana.CaseId = inx.CaseId;
                    campana.Campaing_name = inx.OutboundChainRecord.CampaignName;
                    campana.Calling_list_name = inx.OutboundChainRecord.CallingListName;
                    campana.GUID = item.CallAttemptGUID;
                    campana.campaign_dbid = item.CampaignInfo.ApplicationID;

                    campana.campaignMode = inx.GetAttachedData("DIALMODE") != null ?
                                        inx.GetAttachedData("DIALMODE").ToString() :
                                        "";
                
                    try
                    {
                        XDocument Documento = Utility.GeneraXML(inx);
                        XElement xmlOut = null;
                        if (Documento != null)
                        {
                            xmlOut = XElement.Parse(Documento.ToString());
                        }
                        campana.Field = xmlOut;
                    }
                    catch (Exception) { }

                    EResultado resultado = null;
                    _metodoApi.InteraccionCampana(out resultado, campana, token);
                    _metodos.InsertaRegistroMarkDoneModule(inx);


                }
            }
            catch (Exception)
            {

                
            }
           
        }

        private void ConvierteClase(int idEvento, IMessage evento, IInteraction inx)
        {
            var data = "";
            if (_caseIdentifier.CaseId != null)
            {
                data = _caseIdentifier.CaseId;
            }
            else if (inx.CaseId != null)
            {
                data = _caseIdentifier.CaseId;
            }

            InteractionVoz inxVoz = new InteractionVoz();
            Campana campana = new Campana();
            switch (idEvento)
            {
                case EventDialing.MessageId:
                    var eventDial = evento as EventDialing;
                    inxVoz.CImessage = new CImessage
                    {
                        nombreEvento = eventDial.Name,
                        ani = eventDial.ANI,
                        dnis = eventDial.DNIS,
                        CallType = eventDial.CallType,
                        Connid = eventDial.ConnID.ToString(),
                        eventId = eventDial.Id,
                        TransferConnid = eventDial.TransferConnID != null ?
                                        eventDial.TransferConnID.ToString() :
                                        "",

                    };
                    inxVoz.Cinteraction = new CIInteracion
                    {
                        CaseId = inx.CaseId,
                        HasOutboundRecord = inx.HasOutboundChainRecord,
                        Records = null,
                        type = inx.Type

                    };

                    inxVoz.CinteractionWebInx = new CIInteracionWeb
                    {
                        formulario = null,
                        ParentId = inx.ParentInteractionId
                    };

                    if (inxVoz.Cinteraction.HasOutboundRecord)
                    {
                        ICampaignManager cm = _container.Resolve<ICampaignManager>();
                        IWCampaign c;

                        for (int i = 0; i < cm.IWCampaigns.Count(); i++)
                        {
                            if (cm.IWCampaigns[i].Mode.Value != null)
                                inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
                        }


                    }


                    var xmlAtachado = Utility.GeneraXMLAtachado(listaAtached, inx);
                    if (xmlAtachado != null)
                    {
                        inxVoz.ATTACHED = XElement.Parse(xmlAtachado.ToString());
                    }
                    if (inx.IsMonitoredByMe)
                        inxVoz.CImessage.EsMonitoreada = true;
                    break;
                case EventAck.MessageId:
                    var eventInvite = evento as EventAck;
                    inxVoz.CImessage = new CImessage
                    {
                        nombreEvento = eventInvite.Name,
                        ani = "",
                        dnis = "",

                        Connid = inx.InteractionId,
                        eventId = eventInvite.Id,
                        TransferConnid = ""

                    };
                    inxVoz.Cinteraction = new CIInteracion
                    {
                        CaseId = inx.CaseId,
                        HasOutboundRecord = inx.HasOutboundChainRecord,
                        Records = null,
                        type = inx.Type

                    };

                    inxVoz.CinteractionWebInx = new CIInteracionWeb
                    {
                        formulario = null,
                        ParentId = inx.EntrepriseInteractionCurrent.Id
                    };

                    if (inxVoz.Cinteraction.HasOutboundRecord)
                    {
                        ICampaignManager cm = _container.Resolve<ICampaignManager>();
                        IWCampaign c;

                        for (int i = 0; i < cm.IWCampaigns.Count(); i++)
                        {
                            if (cm.IWCampaigns[i].Mode.Value != null)
                                inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
                        }


                    }


                    var xmlAtachadoInvite = Utility.GeneraXMLAtachado(listaAtached, inx);
                    if (xmlAtachadoInvite != null)
                    {
                        inxVoz.ATTACHED = XElement.Parse(xmlAtachadoInvite.ToString());
                    }
                    if (inx.IsMonitoredByMe)
                        inxVoz.CImessage.EsMonitoreada = true;
                    break;
                case EventEstablished.MessageId:
                    var eventEstablished = evento as EventEstablished;
                    inxVoz.CImessage = new CImessage
                    {
                        nombreEvento = eventEstablished.Name,
                        ani = eventEstablished.ANI,
                        dnis = eventEstablished.DNIS,
                        CallType = eventEstablished.CallType,
                        Connid = eventEstablished.ConnID.ToString(),
                        eventId = eventEstablished.Id,
                        TransferConnid = eventEstablished.TransferConnID != null ?
                                        eventEstablished.TransferConnID.ToString() :
                                        "",

                    };
                    inxVoz.Cinteraction = new CIInteracion
                    {
                        CaseId = inx.CaseId,
                        HasOutboundRecord = inx.HasOutboundChainRecord,
                        Records = null,
                        type = inx.Type


                    };
                    inxVoz.CinteractionWebInx = new CIInteracionWeb
                    {
                        formulario = null,
                        ParentId = inx.ParentInteractionId
                    };

                    if (inxVoz.Cinteraction.HasOutboundRecord)
                    {
                        ICampaignManager cm = _container.Resolve<ICampaignManager>();
                        IWCampaign c;

                        for (int i = 0; i < cm.IWCampaigns.Count(); i++)
                        {
                            if (cm.IWCampaigns[i].Mode.Value != null)
                                inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
                            var nombreCampaign = cm.IWCampaigns[i].Name.ToString();
                        }

                        if (inxVoz.CImessage.DialMode.Equals("PROGRESSIVE") || inxVoz.CImessage.DialMode.Equals("PREDICTIVE"))
                        {
                            ConvierteClaseCampana(inx);
                        }
                    }

                    var xmlAtachad = Utility.GeneraXMLAtachado(listaAtached, inx);
                    if (xmlAtachad != null)
                    {
                        inxVoz.ATTACHED = XElement.Parse(xmlAtachad.ToString());
                    }
                    string transferidoOrigen = "";
                    if (eventEstablished.UserData.GetAsKeyValueCollection("IWAttachedDataInformation") != null)
                    {
                        var ValuesKey = eventEstablished.UserData.GetAsKeyValueCollection("IWAttachedDataInformation");
                        if (ValuesKey.GetValues("GCS_TransferringAgentName") != null)
                        {
                            transferidoOrigen = ValuesKey.GetValues("GCS_TransferringAgentName").FirstOrDefault().ToString();
                        }
                    }
                    if (inx.IsMonitoredByMe)
                        inxVoz.CImessage.EsMonitoreada = true;

                    //if (transferidoOrigen != "" && transferidoOrigen != Agente.UserName)
                    //    return;

                    break;
                case EventPartyChanged.MessageId:
                    var eventPartyChanged = evento as EventPartyChanged;
                    inxVoz.CImessage = new CImessage
                    {
                        nombreEvento = eventPartyChanged.Name,
                        ani = eventPartyChanged.ANI,
                        dnis = eventPartyChanged.DNIS,
                        CallType = eventPartyChanged.CallType,
                        Connid = eventPartyChanged.ConnID.ToString(),
                        eventId = eventPartyChanged.Id,
                        TransferConnid = eventPartyChanged.PreviousConnID != null ?
                                         eventPartyChanged.PreviousConnID.ToString() : "",

                    };
                    inxVoz.Cinteraction = new CIInteracion
                    {
                        CaseId = inx.CaseId,
                        HasOutboundRecord = inx.HasOutboundChainRecord,
                        Records = null,
                        type = inx.Type

                    };

                    inxVoz.CinteractionWebInx = new CIInteracionWeb
                    {
                        formulario = null,
                        ParentId = inx.ParentInteractionId
                    };

                    if (inx.GetAttachedData("GCS_TransferringAgentName") != null)
                    {
                        var agenteTransferencia = inx.GetAttachedData("GCS_TransferringAgentName").ToString();

                        if (!agenteTransferencia.Equals(Agente.UserName))
                        {
                            _metodos.InsertaRegistroMarkDoneModule(inx);
                            return;
                        }
                    }
                    if (inx.IsMonitoredByMe)
                        inxVoz.CImessage.EsMonitoreada = true;
                    break;
                case EventReleased.MessageId:
                    var eventReleased = evento as EventReleased;
                    inxVoz.CImessage = new CImessage
                    {
                        nombreEvento = eventReleased.Name,
                        ani = eventReleased.ANI,
                        dnis = eventReleased.DNIS,
                        CallType = eventReleased.CallType,
                        Connid = eventReleased.ConnID.ToString(),
                        eventId = eventReleased.Id,
                        TransferConnid = eventReleased.TransferConnID != null ?
                                        eventReleased.TransferConnID.ToString() :
                                        "",

                        Release = eventReleased.Extensions.GetAsString("ReleasingParty") != null ?
                                 eventReleased.Extensions.GetAsString("ReleasingParty").ToString() :
                                 null,
                    };
                    inxVoz.Cinteraction = new CIInteracion
                    {
                        CaseId = inx.CaseId,
                        HasOutboundRecord = inx.HasOutboundChainRecord,
                        Records = null,
                        type = inx.Type


                    };

                    if (inx.EntrepriseInteractionCurrent.Profile != null)
                    {
                        if (inx.EntrepriseInteractionCurrent.Profile.ToString().Equals("Transfered"))
                        {

                            inxVoz.CImessage.TransferConnid = inx.EntrepriseInteractionCurrent.Data.Owner != null ?
                               inx.EntrepriseInteractionCurrent.Data.Owner.ToString() : "";
                        }

                    }



                    if (inxVoz.Cinteraction.HasOutboundRecord)
                    {
                        ICampaignManager cm = _container.Resolve<ICampaignManager>();
                        IWCampaign c;

                        for (int i = 0; i < cm.IWCampaigns.Count(); i++)
                        {
                            if (cm.IWCampaigns[i].Mode.Value != null)
                                inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
                        }

                        if (inxVoz.CImessage.DialMode.Equals("PROGRESSIVE") || inxVoz.CImessage.DialMode.Equals("PREDICTIVE"))
                        {
                            ConvierteClaseCampana(inx);
                        }
                    }

                    if (inxVoz.CImessage.TransferConnid.Equals(inxVoz.CImessage.Connid))
                    {
                        inxVoz.CImessage.TransferConnid = eventReleased.TransferConnID != null ?
                                        eventReleased.TransferConnID.ToString() :
                                        "";
                    }
                    if (inx.EntrepriseInteractionCurrent.Profile != null)
                    {
                        if (inx.EntrepriseInteractionCurrent.Profile.ToString().Equals("Transfered"))
                            inxVoz.CImessage.TransferredNetworkCallID = 1;
                    }
                    if (inx.IsMonitoredByMe)
                        inxVoz.CImessage.TransferConnid = "";

                    if (inx.IsMonitoredByMe)
                        inxVoz.CImessage.EsMonitoreada = true;
                    break;
                case Platform.Voice.Protocols.TServer.Events.EventUserEvent.MessageId:
                    var evetUserEvent = evento as Platform.Voice.Protocols.TServer.Events.EventUserEvent;

                    break;               
                default:
                    return;
                    break;
            }


            EResultado resultado = null;
            _metodoApi.InteraccionVoz(out resultado, inxVoz, token);

        }

        //private void ConvierteClase(int idEvento, IMessage evento, IInteraction inx)
        //{
        //    var data = _caseIdentifier.CaseId;
        //    InteractionVoz inxVoz = new InteractionVoz();
        //    Campana campana = new Campana();
        //    switch (idEvento)
        //    {
        //        case EventDialing.MessageId:
        //            var eventDial = evento as EventDialing;
        //            inxVoz.CImessage = new CImessage
        //            {
        //                nombreEvento = eventDial.Name,
        //                ani = eventDial.ANI,
        //                dnis = eventDial.DNIS,
        //                CallType = eventDial.CallType,
        //                Connid = eventDial.ConnID.ToString(),
        //                eventId = eventDial.Id,
        //                TransferConnid = eventDial.TransferConnID != null ?
        //                                eventDial.TransferConnID.ToString() :
        //                                "",

        //            };
        //            inxVoz.Cinteraction = new CIInteracion
        //            {
        //                CaseId = inx.CaseId,
        //                HasOutboundRecord = inx.HasOutboundChainRecord,
        //                Records = null,
        //                type = inx.Type

        //            };
        //            if (inxVoz.Cinteraction.HasOutboundRecord)
        //            {
        //                ICampaignManager cm = _container.Resolve<ICampaignManager>();
        //                IWCampaign c;

        //                for (int i = 0; i < cm.IWCampaigns.Count(); i++)
        //                {
        //                    if (cm.IWCampaigns[i].Mode.Value != null)
        //                        inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
        //                }


        //            }


        //            var xmlAtachado = Utility.GeneraXMLAtachado(listaAtached, inx);
        //            if (xmlAtachado != null)
        //            {
        //                inxVoz.ATTACHED = XElement.Parse(xmlAtachado.ToString());
        //            }
        //            if (inx.IsMonitoredByMe)
        //                inxVoz.CImessage.EsMonitoreada = true;
        //            break;
        //        case EventEstablished.MessageId:
        //            var eventEstablished = evento as EventEstablished;
        //            inxVoz.CImessage = new CImessage
        //            {
        //                nombreEvento = eventEstablished.Name,
        //                ani = eventEstablished.ANI,
        //                dnis = eventEstablished.DNIS,
        //                CallType = eventEstablished.CallType,
        //                Connid = eventEstablished.ConnID.ToString(),
        //                eventId = eventEstablished.Id,
        //                TransferConnid = eventEstablished.TransferConnID != null ?
        //                                eventEstablished.TransferConnID.ToString() :
        //                                "",

        //            };
        //            inxVoz.Cinteraction = new CIInteracion
        //            {
        //                CaseId = inx.CaseId,
        //                HasOutboundRecord = inx.HasOutboundChainRecord,
        //                Records = null,
        //                type = inx.Type


        //            };

        //            if (inxVoz.Cinteraction.HasOutboundRecord)
        //            {
        //                ICampaignManager cm = _container.Resolve<ICampaignManager>();
        //                IWCampaign c;

        //                for (int i = 0; i < cm.IWCampaigns.Count(); i++)
        //                {
        //                    if(cm.IWCampaigns[i].Mode.Value != null)
        //                        inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
        //                     var nombreCampaign = cm.IWCampaigns[i].Name.ToString();
        //                }

        //                if (inxVoz.CImessage.DialMode.Equals("PROGRESSIVE") || inxVoz.CImessage.DialMode.Equals("PREDICTIVE"))
        //                {
        //                    ConvierteClaseCampana(inx);
        //                }
        //            }

        //            var xmlAtachad = Utility.GeneraXMLAtachado(listaAtached, inx);
        //            if (xmlAtachad != null)
        //            {
        //                inxVoz.ATTACHED = XElement.Parse(xmlAtachad.ToString());
        //            }
        //            string transferidoOrigen = "";
        //            if (eventEstablished.UserData.GetAsKeyValueCollection("IWAttachedDataInformation") != null)
        //            {
        //                var ValuesKey = eventEstablished.UserData.GetAsKeyValueCollection("IWAttachedDataInformation");
        //                if (ValuesKey.GetValues("GCS_TransferringAgentName") != null)
        //                {
        //                    transferidoOrigen = ValuesKey.GetValues("GCS_TransferringAgentName").FirstOrDefault().ToString();
        //                }
        //            }
        //            if (inx.IsMonitoredByMe)
        //                inxVoz.CImessage.EsMonitoreada = true;

        //            //if (transferidoOrigen != "" && transferidoOrigen != Agente.UserName)
        //            //    return;

        //            break;
        //        case EventPartyChanged.MessageId:
        //            var eventPartyChanged = evento as EventPartyChanged;
        //            inxVoz.CImessage = new CImessage
        //            {
        //                nombreEvento = eventPartyChanged.Name,
        //                ani = eventPartyChanged.ANI,
        //                dnis = eventPartyChanged.DNIS,
        //                CallType = eventPartyChanged.CallType,
        //                Connid = eventPartyChanged.ConnID.ToString(),
        //                eventId = eventPartyChanged.Id,
        //                TransferConnid = eventPartyChanged.PreviousConnID != null ?
        //                                 eventPartyChanged.PreviousConnID.ToString() : "",

        //            };
        //            inxVoz.Cinteraction = new CIInteracion
        //            {
        //                CaseId = inx.CaseId,
        //                HasOutboundRecord = inx.HasOutboundChainRecord,
        //                Records = null,
        //                type = inx.Type

        //            };

        //            if (inx.GetAttachedData("GCS_TransferringAgentName") != null)
        //            {
        //                var agenteTransferencia = inx.GetAttachedData("GCS_TransferringAgentName").ToString();

        //                if (!agenteTransferencia.Equals(Agente.UserName))
        //                {
        //                    _metodos.InsertaRegistroMarkDoneModule(inx);
        //                    return;
        //                }
        //            }
        //            if (inx.IsMonitoredByMe)
        //                inxVoz.CImessage.EsMonitoreada = true;
        //            break;
        //        case EventReleased.MessageId:
        //            var eventReleased = evento as EventReleased;
        //            inxVoz.CImessage = new CImessage
        //            {
        //                nombreEvento = eventReleased.Name,
        //                ani = eventReleased.ANI,
        //                dnis = eventReleased.DNIS,
        //                CallType = eventReleased.CallType,
        //                Connid = eventReleased.ConnID.ToString(),
        //                eventId = eventReleased.Id,
        //                TransferConnid = eventReleased.TransferConnID != null ?
        //                                eventReleased.TransferConnID.ToString() :
        //                                "",

        //                Release = eventReleased.Extensions.GetAsString("ReleasingParty") != null ?
        //                         eventReleased.Extensions.GetAsString("ReleasingParty").ToString() :
        //                         null,
        //            };
        //            inxVoz.Cinteraction = new CIInteracion
        //            {
        //                CaseId = inx.CaseId,
        //                HasOutboundRecord = inx.HasOutboundChainRecord,
        //                Records = null,
        //                type = inx.Type


        //            };

        //            if (inx.EntrepriseInteractionCurrent.Profile != null)
        //            {
        //                if (inx.EntrepriseInteractionCurrent.Profile.ToString().Equals("Transfered"))
        //                {

        //                    inxVoz.CImessage.TransferConnid = inx.EntrepriseInteractionCurrent.Data.Owner != null ?
        //                       inx.EntrepriseInteractionCurrent.Data.Owner.ToString() : "";
        //                }

        //            }



        //            if (inxVoz.Cinteraction.HasOutboundRecord)
        //            {
        //                ICampaignManager cm = _container.Resolve<ICampaignManager>();
        //                IWCampaign c;

        //                for (int i = 0; i < cm.IWCampaigns.Count(); i++)
        //                {
        //                    if (cm.IWCampaigns[i].Mode.Value != null)
        //                        inxVoz.CImessage.DialMode = cm.IWCampaigns[i].Mode.Value.ToString();
        //                }

        //                if (inxVoz.CImessage.DialMode.Equals("PROGRESSIVE") || inxVoz.CImessage.DialMode.Equals("PREDICTIVE"))
        //                {
        //                    ConvierteClaseCampana(inx);
        //                }
        //            }

        //            if (inxVoz.CImessage.TransferConnid.Equals(inxVoz.CImessage.Connid))
        //            {
        //                inxVoz.CImessage.TransferConnid = eventReleased.TransferConnID != null ?
        //                                eventReleased.TransferConnID.ToString() :
        //                                "";
        //            }
        //            if (inx.EntrepriseInteractionCurrent.Profile != null)
        //            {
        //                if (inx.EntrepriseInteractionCurrent.Profile.ToString().Equals("Transfered"))
        //                    inxVoz.CImessage.TransferredNetworkCallID = 1;
        //            }
        //            if (inx.IsMonitoredByMe)
        //                inxVoz.CImessage.TransferConnid = "";

        //            if (inx.IsMonitoredByMe)
        //                inxVoz.CImessage.EsMonitoreada = true;
        //            break;
        //        case Platform.Voice.Protocols.TServer.Events.EventUserEvent.MessageId:
        //            var evetUserEvent = evento as Platform.Voice.Protocols.TServer.Events.EventUserEvent;

        //            break;
        //        default:
        //            return;
        //            break;
        //    }


        //    EResultado resultado = null;
        //   _metodoApi.InteraccionVoz(out resultado, inxVoz,token);

        //}


    }
}