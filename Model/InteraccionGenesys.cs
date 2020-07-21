using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.Outbound.Model.Campaign;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using System;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model
{
    public static class InteraccionGenesys
    {
        public static Interaccion GetAttachedData(IInteraction inx)
        {
            var datosInx = new Interaccion
            {
                GenAgente = InteractionExtensionCrossnetModule.Agente.UserName
            };


            if (inx.EntrepriseInteractionCurrent != null)
            {
                datosInx.GenConnid = inx.EntrepriseInteractionCurrent.Id;

                switch (inx.EntrepriseInteractionCurrent.IdType.Direction.ToString())
                {
                    case "In":
                        {
                            datosInx.GenCanalEntrada = "Inbound";
                            break;
                        }

                    case "Out":
                        {
                            datosInx.GenCanalEntrada = "Outbound";
                            break;
                        }
                }
            }

            if (inx.Type.Equals("InteractionVoice"))
            {
                datosInx.GenFechaInicioCall = inx.StartDate;
                datosInx.GenNombreInteraccion = inx.Type;

                switch (inx.EntrepriseLastInteractionEvent.Id)
                {
                    case EventDialing.MessageId:
                        {
                            var eventDial = inx.EntrepriseLastInteractionEvent as EventDialing;
                            if (eventDial != null)
                            {
                                datosInx.GenOrigen = eventDial.ANI;
                                datosInx.GenDestino = eventDial.DNIS;
                            }
                            return datosInx;
                        }

                    case EventRinging.MessageId:
                        {
                            var eventRing = inx.EntrepriseLastInteractionEvent as EventRinging;
                            if (eventRing != null)
                            {
                                datosInx.GenOrigen = eventRing.ANI;
                                datosInx.GenDestino = eventRing.DNIS;
                            }
                            return datosInx;
                        }

                    case EventEstablished.MessageId:
                        {
                            var eventEstab = inx.EntrepriseLastInteractionEvent as EventEstablished;
                            if (eventEstab != null)
                            {
                                datosInx.GenOrigen = eventEstab.ANI;
                                datosInx.GenDestino = eventEstab.DNIS;

                              
                            }
                            return datosInx;
                        }

                    case EventReleased.MessageId:
                        {
                            var eventRelease = inx.EntrepriseLastInteractionEvent as EventReleased;
                            if (eventRelease != null)
                            {
                                datosInx.GenOrigen = eventRelease.ANI;
                                datosInx.GenDestino = eventRelease.DNIS;
                            }
                            return datosInx;
                        }
                    case EventAttachedDataChanged.MessageId:
                        {
                            var eventDataChanged =
                                inx.EntrepriseLastInteractionEvent as EventAttachedDataChanged;
                            if (eventDataChanged != null)
                            {
                                datosInx.GenOrigen = eventDataChanged.ANI;
                                datosInx.GenDestino = eventDataChanged.DNIS;
                            }
                            return datosInx;
                        }
                }

            }

            if (inx.Type.Equals("InteractionChat"))
            {
                try
                {
                    datosInx.GenConnid = inx.EntrepriseInteractionCurrent != null &&
                                         inx.EntrepriseInteractionCurrent.Id != null
                        ? inx.EntrepriseInteractionCurrent.Id
                        : "";
                    datosInx.GenFechaInicioCall = inx.StartDate;
                    datosInx.GenNombreInteraccion = inx.Type;
                    if (datosInx.GenCanalEntrada.Equals("Inbound"))
                    {
                        datosInx.GenTipoInteraccion = 1;
                    }else if (datosInx.GenCanalEntrada.Equals("Outbound"))
                    {
                        datosInx.GenTipoInteraccion = 2;
                    }
                }
                catch (Exception error)
                {
                    GenesysAlert.SendMessage(
                        "Se ha producido un error al obtener los campos del Chat en InteractionChat: " +
                        error.Message, "Public", SeverityType.Error);
                }
            }
            if (inx.Type.Equals("InteractionCallback"))
            {
                try
                {

                    datosInx.GenOrigen = inx.GetAttachedData("PhoneNumber").ToString();
                    
                }
                catch (Exception error)
                {
                    GenesysAlert.SendMessage(
                        "Se ha producido un error al obtener los campos del fono en InteractionCallback: " +
                        error.Message, "Public", SeverityType.Error);
                }
            }
            if (inx.Type.Equals("InteractionInboundEmail"))
            {
                try
                {
                    datosInx.GenConnid = inx.EntrepriseInteractionCurrent != null &&
                                         inx.EntrepriseInteractionCurrent.Id != null
                        ? inx.EntrepriseInteractionCurrent.Id
                        : "";
                    datosInx.GenFechaInicioCall = inx.StartDate;
                    datosInx.GenTipoInteraccion = 10;
                }
                catch (Exception error)
                {
                    GenesysAlert.SendMessage(
                        "Se ha producido un error al obtener los campos del email en InteractionInboundEmail: " +
                        error.Message, "Public", SeverityType.Error);
                }

            }

            if (inx.Type.Equals("InteractionOutboundEmail"))
            {
                try
                {
                    datosInx.GenConnid = inx.EntrepriseInteractionCurrent != null &&
                                         inx.EntrepriseInteractionCurrent.Id != null
                        ? inx.EntrepriseInteractionCurrent.Id
                        : "";
                    datosInx.GenFechaInicioCall = inx.StartDate;
                    datosInx.GenTipoInteraccion = 11;
                }
                catch (Exception error)
                {
                    GenesysAlert.SendMessage(
                        "Se ha producido un error al obtener los campos del email en InteractionOutboundEmail: " +
                        error.Message, "Public", SeverityType.Error);
                }
            }

            if (inx.Type.Equals("InteractionPullPreview"))
            {
                try
                {
                    datosInx.GenRecordId = inx.OutboundChainRecord.UserData.GetAsInt("recordid");
                    datosInx.GenChainIdCampana = inx.OutboundChainRecord.UserData.GetAsInt("GSW_CHAIN_ID");
                    datosInx.GenCampaingName = inx.OutboundChainRecord.CampaignName;
                    datosInx.GenGuid = inx.OutboundChainRecord.UserData.GetAsString("GSW_CALL_ATTEMPT_GUID");
                    datosInx.GenAttempt = inx.OutboundChainRecord.UserData.GetAsInt("GSW_ATTEMPTS");
                    datosInx.GenGroupName = inx.OutboundChainRecord.UserData.GetAsString("GSW_CAMPAIGN_GROUP_NAME");
                    datosInx.GenTipoInteraccion = 7;
                    datosInx.GenCanalEntrada = inx.OutboundChainRecord.UserData.GetAsString("InteractionType");
                    datosInx.GenCanalEntrada = "PreviewRecord";
                    datosInx.GenEvento = inx.OutboundChainRecord.UserData.GetAsString("GSW_USER_EVENT");
                    datosInx.GenDestino = inx.OutboundChainRecord.UserData.GetAsString("GSW_PHONE");
                    datosInx.GenOrigen = inx.Agent.Place.PlaceName;
                    datosInx.GenFechaInicioCall = DateTime.Now;
                }
                catch (Exception error)
                {
                    GenesysAlert.SendMessage(
                        "Se ha producido un error al obtener los campos de la campaña en InteractionPullPreview: " +
                        error.Message, "Public", SeverityType.Error);
                }
            }

            if (inx.Type.Equals("InteractionPushPreview"))
            {
                try
                {
                    datosInx.GenRecordId = inx.OutboundChainRecord.UserData.GetAsInt("recordid");
                    datosInx.GenChainIdCampana = inx.OutboundChainRecord.UserData.GetAsInt("GSW_CHAIN_ID");
                    datosInx.GenCampaingName = inx.OutboundChainRecord.CampaignName;
                    datosInx.GenGuid = inx.OutboundChainRecord.UserData.GetAsString("GSW_CALL_ATTEMPT_GUID");
                    datosInx.GenAttempt = inx.OutboundChainRecord.UserData.GetAsInt("GSW_ATTEMPTS");
                    datosInx.GenGroupName = inx.OutboundChainRecord.UserData.GetAsString("GSW_CAMPAIGN_GROUP_NAME");
                    datosInx.GenTipoInteraccion = 37;
                    datosInx.GenCanalEntrada = inx.OutboundChainRecord.UserData.GetAsString("InteractionType");
                    datosInx.GenCanalEntrada = "PreviewRecord";
                    datosInx.GenEvento = inx.OutboundChainRecord.UserData.GetAsString("GSW_USER_EVENT");
                    datosInx.GenDestino = inx.OutboundChainRecord.UserData.GetAsString("GSW_PHONE");
                    datosInx.GenOrigen = inx.Agent.Place.PlaceName;
                    datosInx.GenFechaInicioCall = DateTime.Now;
                }
                catch (Exception error)
                {
                    GenesysAlert.SendMessage(
                        "Se ha producido un error al obtener los campos de la campaña en InteractionPullPreview: " +
                        error.Message, "Public", SeverityType.Error);
                }
            }

            return datosInx;
        }
    }
}