using Genesyslab.Desktop.Infrastructure;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    public interface ITipificacionesButtonView : IView
    {
        ITipificacionesViewModel Model { get; set; }
    }
}