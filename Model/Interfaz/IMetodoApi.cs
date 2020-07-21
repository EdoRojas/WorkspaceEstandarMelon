using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    public interface IMetodoApi
    {
        Autentificacion autentificacion(out EResultado error,Agente ag);
        Arbol ArbolTipificacion(out EResultado error, string token);
        EResultado InteraccionVoz(out EResultado error, InteractionVoz inxvoz,string token);

    }
}
