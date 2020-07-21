using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Database;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    public interface ITipificacionesViewModel
    {
        string Header { get; set; }

        ICase Case { get; set; }

        StackPanel ArbolTipificaciones { get; set; }
        
        void GenerarTipificaciones(DataTable source, UIElement elementoPadre, string tipo);
        
    }
}