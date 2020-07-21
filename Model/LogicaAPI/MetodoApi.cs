using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.LogicaAPI
{
   public class MetodoApi : IMetodoApi
    {
       
       public static string token = "";
       HttpWebRequest Webrequest = null;
       HttpWebResponse response = null;
       WebClient client = new WebClient();
       EResultado error = new EResultado();
        

       public Autentificacion autentificacion(out EResultado MensaejError,Agente ag) 
       {
            Autentificacion _auth = new Autentificacion();
            MensaejError = null;
           try
           {
               
              _auth = request<Autentificacion>("Autentificacion?username="+ag.userName +"&firstName="+ag.firstName+ "&lastName="+ ag.lastName + "&tenantDBID="+ ag.tenantDBid+ "&Agentdbid="+ag.agentDBid+"&employeID="+ ag.EmployeeId + "", "GET", "", out error,"") as Autentificacion;
              MensaejError = error;
           }
           catch (Exception ex)
           {
               return null;
           }
           return _auth;
       }

       public Arbol ArbolTipificacion(out EResultado MensaejError, string token)
       {
            Arbol listaArbol = new Arbol();
           EResultado error = new EResultado();
           MensaejError = null;
           try
           {
               listaArbol = request<Arbol>("ArbolTipificacion", "GET", token, out error,"") as Arbol;
               MensaejError = error;
           }
           catch (Exception ex)
           {
               return null;
           }
           return listaArbol;
       }

        public HistorialCliente Historial(out EResultado MensaejError, string token, string medio, string valorTipo, string periodo, string tipo, string top, int tenant)
        //public HistorialCliente Historial(out EResultado MensaejError, string token,string medio,string valorTipo,string periodo,string tipo, string top)
        {
            HistorialCliente listaArbol = new HistorialCliente();
            EResultado error = new EResultado();
            MensaejError = null;
            try
            {
                //Historial? ani = 932 & dni = 932 & valorTipo = &periodo = T & tipo = &top = 10
               listaArbol = request<HistorialCliente>("Historial?medio="+medio+"&valorTipo="+valorTipo+"&periodo="+periodo+"&tipo="+tipo+"&top="+top+"&tenant=" + tenant + "", "GET", token, out error, "") as HistorialCliente;
                MensaejError = error;
            }
            catch (Exception ex)
            {
                return null;
            }
            return listaArbol;
        }
        public List<Atached> ListAtached(out EResultado MensaejError, string token)
        {
            List<Atached> listaArbol = new List<Atached>();
            EResultado error = new EResultado();
            MensaejError = null;
            try
            {
                listaArbol = request<List<Atached>>("listaAttach", "GET", token, out error, "") as List<Atached>;
                MensaejError = error;
            }
            catch (Exception ex)
            {
                return null;
            }
            return listaArbol;
        }

        public Filtro ListaFiltro(out EResultado MensaejError, string token)
        {
            Filtro filtro = new Filtro();
            EResultado error = new EResultado();
            MensaejError = null;
            try
            {
                filtro = request<Filtro>("listaFiltros", "GET", token, out error, "") as Filtro;
                MensaejError = error;
            }
            catch (Exception ex)
            {
                return null;
            }
            return filtro;
        }

        public EResultado Tipifcacion(out EResultado MensaejError, Tipificacion tipificacion, string token )
        {
            MensaejError = null;
         
            try
            {
                var JsonTipificacion = JsonConvert.SerializeObject(tipificacion);
                error = request<EResultado>("Tipificacion", "POST", token, out error, JsonTipificacion) as EResultado;
            }
            catch (Exception ex)
            {
                return null;
            }
            return error;
        }

        public EResultado VistaInteraccion(out EResultado MensaejError, EVistaInteraccion vistaInteraccion, string token)
        {
            MensaejError = null;

            try
            {
                var JsonTipificacion = JsonConvert.SerializeObject(vistaInteraccion);
                error = request<EResultado>("VistaInteraccion", "POST", token, out error, JsonTipificacion) as EResultado;
            }
            catch (Exception ex)
            {
                return null;
            }
            return error;
        }

        public EResultado InteraccionVoz(out EResultado MensaejError, InteractionVoz inxvoz, string token)
        {
            MensaejError = null;

            try
            {
                var JsonInxVoz = JsonConvert.SerializeObject(inxvoz);
                error = request<EResultado>("InteraccionVoz", "POST", token, out error, JsonInxVoz) as EResultado;
            }
            catch (Exception ex)
            {
                return null;
            }
            return error;
        }

        public EResultado InteraccionCampana(out EResultado MensaejError, Campana inxCampana, string token)
        {
            MensaejError = null;

            try
            {
                var JsonInxCampana = JsonConvert.SerializeObject(inxCampana);
                error = request<EResultado>("Campanas", "POST", token, out error, JsonInxCampana) as EResultado;
            }
            catch (Exception ex)
            {
                return null;
            }
            return error;
        }

        public EResultado TerminoInteraccion(out EResultado MensaejError, TerminoInteraccion inxtermino, string token)
        {
            MensaejError = null;

            try
            {
                var JsonInxVoz = JsonConvert.SerializeObject(inxtermino);
                error = request<EResultado>("TerminoInteraccion", "POST", token, out error, JsonInxVoz) as EResultado;
            }
            catch (Exception ex)
            {
                return null;
            }
            return error;
        }

        internal object request<T>(string operacion, string Method, string _token, out EResultado ERROR,string inx)
        {
            object _obj = new object();
            string _objeto = "";
            ERROR = null;
            try
            {
                //string URL = "http://14.10.0.15:9109/api/"; //QA LIRAY
                string URL = "http://14.10.0.15:9105/api/"; //PRODUCCION LIRAY

                //string URL = "http://10.68.71.103:9105/api/";
                //string URL = "http://localhost:49353/api/";
                URL = URL + operacion;
                Webrequest = (HttpWebRequest)WebRequest.Create(URL);
                Webrequest.Method = Method;
                if(Webrequest.Method == "POST")
                {
                   
                    using (var streamWriter = new StreamWriter(Webrequest.GetRequestStream()))
                    {
                       streamWriter.Write(inx);
                       
                    }
                    Webrequest.ContentType = "application/json";
                    
                }
               
                Webrequest.Headers.Add("TokenAplicativo", "5CD2487E16E828E46AEDC36288E38");
                Webrequest.Headers.Add("Token", _token);
                
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byte1 = encoding.GetBytes(URL);
                
                using (response = Webrequest.GetResponse() as HttpWebResponse)
                {
                    var dataStream = response.GetResponseStream();
                    var reader = new StreamReader(dataStream);
                    var _response = reader.ReadToEnd();
                    var bytes = Encoding.UTF8.GetBytes(_response);
                    _objeto = Encoding.UTF8.GetString(bytes);
                    _obj = JsonConvert.DeserializeObject<T>(_objeto);
                }

                
            }
            catch (WebException webex)
            {

                WebResponse errResp = webex.Response;
                string text = "";
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    text = reader.ReadToEnd();

                    string request = "";



                }
              
            }
            catch (Exception e)
            {



         

            }

            return _obj;
        }

    }
}
