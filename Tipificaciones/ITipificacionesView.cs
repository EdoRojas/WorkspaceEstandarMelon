using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Modules.Windows.Common.DimSize;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    public interface ITipificacionesView : IView, IMin
    {
        ITipificacionesViewModel Model { get; set; }
    }
}