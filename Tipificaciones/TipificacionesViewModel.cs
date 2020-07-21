using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Database;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    public class TipificacionesViewModel : ITipificacionesViewModel, INotifyPropertyChanged
    {
        private readonly LogicaNegocio _metodos = new LogicaNegocio();
        private StackPanel _arbolTipificaciones = new StackPanel();
        private ICase _case;
        private string _header = "Crossnet Custom Header";
        

        public StackPanel ArbolTipificaciones
        {
            get { return _arbolTipificaciones; }
            set
            {
                if (_arbolTipificaciones != value)
                {
                    _arbolTipificaciones = value;
                    OnPropertyChanged("ArbolTipificaciones");
                }
            }
        }

        public ICase Case
        {
            get { return _case; }
            set
            {
                if (_case != value)
                {
                    _case = value;
                    OnPropertyChanged("Case");
                }
            }
        }

        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged("Header");
                }
            }
        }

      

        //Metodo encargado de generar control dinamico que contiene las tipificaciones con sus items subitems segun corresponda.
        public void GenerarTipificaciones(DataTable source, UIElement elementoPadre, string tipo)
        {
            if (!tipo.Equals("NOTIPO"))
            {
                DataRow[] drs = null;
                
                    drs =
                        InteractionExtensionCrossnetModule.TablaTipificaciones.Select("VISTA = '" + tipo +
                                                                                     "' AND [DEPENDIENTE] ='0' ");
                source = InteractionExtensionCrossnetModule.TablaTipificaciones.Clone();
                foreach (var d in drs)
                    source.ImportRow(d);
            }
            var contenido = new StackPanel { Name = "Principal" };

            var childsc = new StackPanel { Name = "panelChilds" };
            foreach (DataRow dr in source.Rows)
            {
                var expandSub = new Expander
                {
                    Header = dr[1].ToString(),
                    Tag = dr[0].ToString(),
                    Name = "chk_" + dr[0],
                    FontSize = 9
                };
                if (elementoPadre == null)
                {
                    expandSub.Uid = dr[0].ToString();
                    expandSub.ToolTip = dr[0].ToString();
                    contenido.Children.Add(expandSub);
                }
                else
                {
                    expandSub.Uid = ((Expander)elementoPadre).Tag.ToString();
                    expandSub.ToolTip = ((Expander)elementoPadre).ToolTip;
                    var margin = expandSub.Margin;
                    margin.Left = 45;
                    margin.Right = 2;
                    margin.Bottom = 2;
                    margin.Top = 2;
                    expandSub.Margin = margin;
                    childsc.Children.Add(expandSub);
                    ((Expander)elementoPadre).Content = childsc;
                }
                var childs = _metodos.GeneraArbolJerarquico(expandSub.Tag.ToString());
                if (childs.Rows.Count > 0)
                    GenerarTipificaciones(childs, expandSub, "NOTIPO");
            }
            ArbolTipificaciones = contenido;
        }


        

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}