using System;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model
{
    public class TipificacionItem
    {
        public TipificacionItem(string value, string nombre, string dependiente = "", string padrePrincipal = "")
        {
            Value = value;
            Nombre = nombre;
            Dependiente = dependiente;
            Padre = padrePrincipal;
            Nivel = COP_GET_NIVEL(value);
        }

   

        public string Value { get; set; }
        public string Nombre { get; set; }
        public string Dependiente { get; set; }
        public string Padre { get; set; }
        public string Nivel { get; set; }

        public string COP_GET_NIVEL(string input)
        {
            try
            {
                var drs =
                    InteractionExtensionCrossnetModule.TablaTipificaciones.Select("DICCIONARIO_ID = " + Convert.ToInt32(input));
                var source = InteractionExtensionCrossnetModule.TablaTipificaciones.Clone();
                foreach (var d in drs)
                    source.ImportRow(d);
                var retorno = source.Rows[0]["NIVEL"].ToString();
                return retorno;
            }
            catch (Exception error)
            {
                return "Error GET_NIVEL: " + error.Message;
            }
        }
    }
}