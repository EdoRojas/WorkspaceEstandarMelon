using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Platform.Voice.Protocols.TServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    class EstructuraDatos { }
    public class Autentificacion
    {
        public string token { get; set; }
        public string IngresoUsuario { get; set; }
    }

    public class Filtro
    {
        public List<FiltroResult> listaFiltro;
        public EResultado resultado;
       
    }
    public class FiltroResult
    {
        public string descripcion { get; set; }
    }


    public class EResultado
    {
        public int? codigo { get; set; }
        public bool error { get; set; }
        public string mesaje { get; set; }
    }

    public class Arbol
    {
        public List<ArbolTipificacion> listaArbol;
        public EResultado resultado;
    }

    

    public class ArbolTipificacion
    {
        public int DICCIONARIO_ID { get; set; }

        public string DESCRIPCION { get; set; }

        public int DEPENDIENTE { get; set; }

        public int ORDEN { get; set; }

        public int NIVEL { get; set; }

        public int TIPO_DATO_ID { get; set; }

        public int VISTA_ID { get; set; }

        public string VISTA { get; set; }








    }

    public class Atached
    {
        public int DICCIONARIO_ATACHED_ID { get; set; }

        public string DESCRIPCION { get; set; }

        public int ACTIVO { get; set; }

    }


    public class InteractionVoz
    {
        public CIInteracion Cinteraction { get; set; }
        public CImessage CImessage { get; set; }
        public XElement ATTACHED { get; set; }
        public CIInteracionWeb CinteractionWebInx { get; set; }
    }
    public class CIInteracionWeb
    {
        public XElement formulario { get; set; }
        public string ParentId { get; set; }

    }
    public class CIInteracion
    {
        public string type { get; set; }
        public bool HasOutboundRecord { get; set; }
        public ICollection<IRecord> Records { get; set; }
        public string CaseId { get; set; }
    }

    public class CImessage
    {
        public int eventId { get; set; }
        public CallType CallType { get; set; }
        public string DialMode { get; set; }
        public string TransferConnid { get; set; }
        public string Connid { get; set; }
        public string ani { get; set; }
        public string dnis { get; set; }
        public int TransferredNetworkCallID { get; set; }
        public string nombreEvento { get; set; }
        public string Release { get; set; }
        public bool EsMonitoreada { get; set; }
    }

    public class Atachado
    {
        public string OPCION { get; set; }
        public string VALOR { get; set; }
    }
    public class Agente
    {
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int tenantDBid { get; set; }
        public int agentDBid { get; set; }
        public string EmployeeId { get; set; }

    }

    public class TerminoInteraccion
    {
        public string CaseId { get; set; }

        public int Agentdbid { get; set; }

    }

    public class EVistaInteraccion
    {
        public string CaseId { get; set; }
        public int Agentdbid { get; set; }
        public int VistaID { get; set; }
    }

    public class Tipificacion
    {
        public string CaseId { get; set; }
        public int Agentdbid { get; set; }
        public int VISTA_ID { get; set; }
        public int MedioID { get; set; }
        public string OBSERVACIONES { get; set; }
        public XElement DATOS_DEMOGRAFICOS { get; set; }
        public XElement TIPIFICACION { get; set; }
        public string VALOR_KEY { get; set; }
        public string DICCIONARIO_ID_KEY { get; set; }
        public XElement TIPIFICACION_CUSTOM { get; set; }
    }
  
    public class HistorialCliente
    {
        public List<Historial> Historial;
        public EResultado resultado;
    }
    
    public class Historial
    {
        public int ID { get; set; }
        public string USERNAME { get; set; }
        public string ESTADO_TIPIFICADO { get; set; }
        public int? VISTAID { get; set; }
        public int INTERACCIONID { get; set; }
        public string OBSERVACION { get; set; }
        public DateTime FECHA_INCIO { get; set; }
        public DateTime? FECHA_HORA_TERMINO { get; set; }
        public string MOTIVO { get; set; }
        public string MEDIO { get; set; }
        public string ANI { get; set; }
        public string DNIS { get; set; }
        public bool TRANSFERT { get; set; }
        public string INTERACCION { get; set; }

    }

    public class Campana
    {
        public string CaseId { get; set; }
        public int Agentdbid { get; set; }
        public int campaign_dbid { get; set; }
        public string GUID { get; set; }
        public string Campaing_name { get; set; }
        public string Nombre_formato { get; set; }
        public string Calling_list_name { get; set; }
        public string campaignMode { get; set; }
        public XElement Field { get; set; }

        
    }
}
