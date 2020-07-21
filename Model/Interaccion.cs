using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model
{
    public class Interaccion
    {
        public string GenGuid { get; set; }
        public string GenArea { get; set; }
        public string GenCc { get; set; }
        public string GenCco { get; set; }
        public string GenFrom { get; set; }
        public string GenTo { get; set; }
        public string NombreCliente { get; set; }
        public string GenConnid { get; set; }
        public string GenPrevConnid { get; set; }
        public string GenGrupoCampana { get; set; }
        public string GenRecordIdCampana { get; set; }
        public int GenChainIdCampana { get; set; }
        public string GenNombreCampana { get; set; }
        public string GenOrigen { get; set; }
        public string GenDestino { get; set; }
        public string GenAgente { get; set; }
        public string GenEvento { get; set; }
        public int GenTipoInteraccion { get; set; }
        public string GenNombreInteraccion { get; set; }
        public string GenParentId { get; set; }
        public string GenEstadoTipificacion { get; set; }
        public string GenEstadoInteraccion { get; set; }
        public DateTime? GenFechaInicioCall { get; set; }
        public DateTime? GenFechaTerminoCall { get; set; }
        public string GenDuracionCall { get; set; }
        public string GenCanalEntrada { get; set; }
        public string GenCanalEstrategia { get; set; }
        public string GenRutEstrategia { get; set; }
        public string GenOpcionEstrategia { get; set; }
        public string GenFirstName { get; set; }
        public string GenLastName { get; set; }
        public string UserName { get; set; }

        //tipificacion
        public string Motivo { get; set; }
        public string MotivoIds { get; set; }
        public string Observaciones { get; set; }
        public int GenRecordId { get; set; }
        public string GenCampaingName { get; set; }
        public int GenAttempt { get; set; }
        public string GenGroupName { get; set; }
        public string GenRecordStatus { get; set; }
        public int? GenCallResult { get; set; }
        public int? GenCampaingMode { get; set; }
        public string GenEventoCampana { get; set; }

        public string genEventoMarkdone { get; set; }
        public string IdMarkdone { get; set; }

        public XElement fields { get; set; }
        public string genDialMode { get; set; }

    }
    
}