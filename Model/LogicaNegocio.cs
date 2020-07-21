using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Database;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones;
using Genesyslab.Desktop.Modules.Outbound.Model.Campaign;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model
{
    public class LogicaNegocio
    {
        private ModeloClasesDataContext _dc = new ModeloClasesDataContext();
        
        //Metodo genérico que trae todos los subitems de un item determinado, segun el id de dependencia ( padre) en las tipificaciones
        public DataTable GeneraArbolJerarquico(string dependencia = "")
        {
            try
            {
                var drs = InteractionExtensionCrossnetModule.TablaTipificaciones
                                .Select("DEPENDIENTE = " + Convert.ToInt32(dependencia));

                var retorno = InteractionExtensionCrossnetModule.TablaTipificaciones.Clone();
                foreach (var d in drs)
                    retorno.ImportRow(d);

                return retorno;
            }
            catch (Exception error)
            {
                GenesysAlert.SendMessage(
                    "Se ha producido un error al obtener la lista de sub-tipificaciones: " + error.Message, "Public",
                    SeverityType.Error);
                return null;
            }
        }

        
        
        public void ActualizaRegistroMarkDone(ICase _caseIdentifier, Interaccion _inx)
        {
            DataRow rowUpdate = null;
            string connid = "";
            var tipo = _caseIdentifier.MainInteraction.Type;
            switch (tipo)
            {
                case "InteractionVoice":
                    if (_caseIdentifier.MainInteraction.HasOutboundChainRecord)
                    {
                        ICampaignManager cm = InteractionExtensionCrossnetModule._container.Resolve<ICampaignManager>();
                        IWCampaign c;

                        for (int i = 0; i < cm.IWCampaigns.Count(); i++)
                        {
                            if (cm.IWCampaigns[i].Mode.Value != null)
                                _inx.genDialMode = cm.IWCampaigns[i].Mode.Value.ToString();
                        }


                        if (_inx.genDialMode != null)
                        {
                            if (_inx.genDialMode.Equals("PROGRESSIVE") || _inx.genDialMode.Equals("PREDICTIVE"))
                            {
                                rowUpdate = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + _caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Id + "'").FirstOrDefault();
                                rowUpdate["Estado"] = "TIPIFICADO";
                            }
                            else
                            {
                                var item3 = _caseIdentifier.MainInteraction.OutboundChainRecord.Records.FirstOrDefault();
                                rowUpdate = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + item3.CallAttemptGUID + "'").FirstOrDefault();
                            }
                        }
                        else
                        {
                            var item2 = _caseIdentifier.MainInteraction.OutboundChainRecord.Records.FirstOrDefault();
                            rowUpdate = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + item2.CallAttemptGUID + "'").FirstOrDefault();
                        }

                    }
                    else
                    if (_caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Profile != null)
                    {
                        if (_caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Profile.ToString().Equals("Consult"))
                        {

                            connid = _caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Data.Owner != null ?
                               _caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Data.Owner.ToString() : "";
                        }
                    }
                   

                    rowUpdate = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + _caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Id + "'").FirstOrDefault();
                    rowUpdate["Estado"] = "TIPIFICADO";
                    break;
                case "InteractionPullPreview":
                    var item = _caseIdentifier.MainInteraction.OutboundChainRecord.Records.FirstOrDefault();
                    rowUpdate = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + item.CallAttemptGUID + "'").FirstOrDefault();
                    rowUpdate["Estado"] = "TIPIFICADO";
                    break;
                case "InteractionCallback":                   
                    rowUpdate = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + _caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Id + "'").FirstOrDefault();
                    rowUpdate["Estado"] = "TIPIFICADO";
                    break;
            }


        }

        public void InsertaRegistroMarkDoneModule(IInteraction Inx)
        {
            try
            {
                DataRow drTablaInteraccion = InteractionExtensionCrossnetModule._TablaInteraccion.NewRow();
                var tipo = Inx.Type;
                string nombreEvento = "";
                string idEvento = "";
                string UserName = "";
                switch (tipo)
                {
                    case "InteractionVoice":
                         nombreEvento = "InteractionVoice";
                         idEvento = Inx.EntrepriseInteractionCurrent.Id;
                        break;
                    case "InteractionPullPreview":
                        var item = Inx.OutboundChainRecord.Records.FirstOrDefault();
                        nombreEvento = "PreviewRecord";
                        idEvento = item.CallAttemptGUID;
                        break;
                    case "InteractionCallback":
                        nombreEvento = "InteractionCallback";
                        idEvento = Inx.EntrepriseInteractionCurrent.Id;
                        break;
                }
                UserName = InteractionExtensionCrossnetModule.Agente != null ? InteractionExtensionCrossnetModule.Agente.UserName : null;
                drTablaInteraccion["idMarkDone"] = idEvento;
                drTablaInteraccion["EventoMarkdone"] = nombreEvento;
                drTablaInteraccion["Estado"] = "NO_TIPIFICADO";
                drTablaInteraccion["Agente"] = UserName;
                var resultMarkDone = InteractionExtensionCrossnetModule._TablaInteraccion.Select("idMarkDone = '" + idEvento +
                                                                                              "' and Agente ='" + UserName + "'").FirstOrDefault();
                if (resultMarkDone == null)
                {
                    InteractionExtensionCrossnetModule._TablaInteraccion.Rows.Add(drTablaInteraccion);
                }

            }
            catch (Exception ex)
            {

            }
        }

      
    }
}