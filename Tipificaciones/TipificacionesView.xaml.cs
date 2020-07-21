using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Database;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.LogicaAPI;
using Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model;
using Genesyslab.Desktop.Modules.Windows.Common.DimSize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Tipificaciones
{
    public partial class TipificacionesView : UserControl, ITipificacionesView
    {
        private readonly IObjectContainer _container;
        private readonly List<TipificacionItem> _listaSeleccion = new List<TipificacionItem>();
        private readonly LogicaNegocio _metodos = new LogicaNegocio();
        ModeloClasesDataContext _dc = new ModeloClasesDataContext();
        private ICase _caseIdentifier;
        private MSize _minSize;
        private Interaccion _inxDatos;
        public Tipificacion tipificacion = new Tipificacion();       
        public MetodoApi metodosAPi = new MetodoApi();
        

        public TipificacionesView(ITipificacionesViewModel tipificacionesViewModel, IObjectContainer container)
        {
            Model = tipificacionesViewModel;
            _container = container;
            InitializeComponent();
            Width = double.NaN;
            Height = double.NaN;
            MinSize = new MSize { Width = 740.0, Height = 400.0 };
        }

        public MSize MinSize
        {
            get { return _minSize; }

            set
            {
                _minSize = value;
                OnPropertyChanged("MinSize");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ITipificacionesViewModel Model
        {
            get { return DataContext as ITipificacionesViewModel; }
            set { DataContext = value; }
        }

        public object Context { get; set; }

        public void Create()
        {
            var contextDictionary = Context as IDictionary<string, object>;

            object caseObject;

            if (contextDictionary.TryGetValue("Case", out caseObject))
            {
                var theCase = caseObject as ICase;
                _caseIdentifier = theCase;
            }

            if (_caseIdentifier != null)
            {
                _inxDatos = InteraccionGenesys.GetAttachedData(_caseIdentifier.MainInteraction);

                FormatearTiempoDeInicioInteraccion();
                LoadFilter();

                if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                {
                    FormatearNumeroLlamadaEntranteParaEncabezado();
                    if (InteractionExtensionCrossnetModule.TablaTipificaciones != null)
                    {
                        if (InteractionExtensionCrossnetModule.TablaTipificaciones != null)
                        {
                            
                            Model.GenerarTipificaciones(null, null, "VistaMelon");
                            string vista = "VistaMelon";
                            var result = InteractionExtensionCrossnetModule.TablaTipificaciones.Select("VISTA = '" + vista + "'").FirstOrDefault();
                            string VistaId = result["VISTA_ID"] != null ? result["VISTA_ID"].ToString() : "";
                            CreateView(VistaId);
                            cargaHistorialInicio();

                        }
                    }
                }

                else if (_inxDatos.GenCanalEntrada.Equals("Outbound"))
                {
                    FormatearNumeroLlamadaSalidaParaEncabezado();
                    if (InteractionExtensionCrossnetModule.TablaTipificaciones != null)
                    {
                        if (InteractionExtensionCrossnetModule.TablaTipificaciones != null)
                        {
                            Model.GenerarTipificaciones(null, null, "VistaMelon");
                            //Model.GenerarTipificaciones(null, null, "TODOS");
                            //string vista = "VistaInbound";
                            //var result = InteractionExtensionCrossnetModule.TablaTipificaciones.Select("VISTA = '" + vista + "'").FirstOrDefault();
                            //string VistaId = result["VISTA_ID"] != null ? result["VISTA_ID"].ToString() : "";
                            //CreateView(VistaId);

                        }
                    }
                    cargaHistorialInicio();
                }
                else if (_inxDatos.GenCanalEntrada.Equals("ScheduledCall") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview")|| _inxDatos.GenCanalEntrada.Equals("InteractionPushPreview") || _inxDatos.GenCanalEntrada.Equals("PreviewRecord"))
                {
                    if (InteractionExtensionCrossnetModule.TablaTipificaciones != null)
                    {
                        if (InteractionExtensionCrossnetModule.TablaTipificaciones != null)
                        {
                            Model.GenerarTipificaciones(null, null, "VistaSolventaSucursales");
                            string vista = "VistaSolventaSucursales";
                            var result = InteractionExtensionCrossnetModule.TablaTipificaciones.Select("VISTA = '" + vista + "'").FirstOrDefault();
                            string VistaId = result["VISTA_ID"] != null ? result["VISTA_ID"].ToString() : "";
                            CreateView(VistaId);
                            

                        }
                    }
                    cargaHistorialInicio();
                }
                FormatearTipoInteraccion();
                FormatearToPHistorial();
            }
            
        }
        private void CreateView(string vistaId)
        {
            try
            {
                EVistaInteraccion evista = new EVistaInteraccion();
                evista.VistaID = vistaId != "" ? int.Parse(vistaId) : 0;
                evista.CaseId = _caseIdentifier.MainInteraction.CaseId;
                evista.Agentdbid = InteractionExtensionCrossnetModule.AgenteConfig.DBID;
                EResultado resultado = null;
                metodosAPi.VistaInteraccion(out resultado, evista, InteractionExtensionCrossnetModule.token);

            }
            catch (Exception ex)
            {
                
            }
        }
        private void FormatearTipoInteraccion()
        {
            TextBoxCanalInteraccion.Text = _caseIdentifier.MainInteraction.Type != null
                ? "Tipo de Interacción: " + _caseIdentifier.MainInteraction.Type + " - " +
                  _inxDatos.GenCanalEntrada
                : "Tipo de interacción: Desconocido";
        }

        private void FormatearToPHistorial()
        {
            cbTop.ItemsSource = null;
            cbTop.Items.Add("50");
            cbTop.Items.Add("10");
            cbTop.Items.Add("5");
        }

        private void FormatearNumeroLlamadaSalidaParaEncabezado()
        {
            TextBoxNumeroEntrante.Text = _inxDatos.GenDestino != null
                ? "Número llamada: " + _inxDatos.GenDestino
                : "Número llamada: Desconocido";
        }

        private void FormatearNumeroLlamadaEntranteParaEncabezado()
        {
            TextBoxNumeroEntrante.Text = _inxDatos.GenOrigen != null
                ? "Número llamada: " + _inxDatos.GenOrigen
                : "Número llamada: Desconocido";
        }

        private void FormatearTiempoDeInicioInteraccion()
        {
            if (_caseIdentifier.MainInteraction.Type.Equals("InteractionPullPreview"))
            {
                TextBoxFechaHora.Text = "Fecha interacción: " + _inxDatos.GenFechaInicioCall.ToString();
                return;
            }

            TextBoxFechaHora.Text = _caseIdentifier.MainInteraction.StartDate != null
                ? "Fecha interacción: " + _caseIdentifier.MainInteraction.StartDate.ToShortDateString() + " " +
                  _caseIdentifier.MainInteraction.StartDate.ToShortTimeString()
                : "Fecha interacción: Desconocido";
        }

        public void Destroy()
        {
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

       
        private async void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            var motivos = "";
            var ids = "";

            var nivel1 = string.Join("|", from item in _listaSeleccion where item.Nivel == "1" select item.Nombre);
            var nivel2 = string.Join("|", from item in _listaSeleccion where item.Nivel == "2" select item.Nombre);
            var nivel3 = string.Join("|", from item in _listaSeleccion where item.Nivel == "3" select item.Nombre);

            var nivel1Id = string.Join("|", from item in _listaSeleccion where item.Nivel == "1" select item.Value);
            var nivel2Id = string.Join("|", from item in _listaSeleccion where item.Nivel == "2" select item.Value);
            var nivel3Id = string.Join("|", from item in _listaSeleccion where item.Nivel == "3" select item.Value);

            if (!nivel1.Equals(string.Empty)) motivos = string.Join(",", nivel1);
            if (!nivel2.Equals(string.Empty)) motivos = string.Join(",", nivel1, nivel2);
            if (!nivel3.Equals(string.Empty)) motivos = string.Join(",", nivel1, nivel2, nivel3);

            if (!nivel1Id.Equals(string.Empty)) ids = string.Join(",", nivel1Id);
            if (!nivel2Id.Equals(string.Empty)) ids = string.Join(",", nivel1Id, nivel2Id);
            if (!nivel3Id.Equals(string.Empty)) ids = string.Join(",", nivel1Id, nivel2Id, nivel3Id);

          
                try
                {
                    tipificacion.CaseId = _caseIdentifier.CaseId;
                    tipificacion.Agentdbid = InteractionExtensionCrossnetModule.AgenteConfig.DBID;
                    tipificacion.VISTA_ID = 18;
                    tipificacion.OBSERVACIONES = TextBoxObservacion.Text;

                    var DatosDemograficos = Utility.GeneraDatosDemograficos(GridDatosDemograficos, ExpanderDatosDemograficos);

                    if (DatosDemograficos != null)
                    {
                        tipificacion.DATOS_DEMOGRAFICOS = XElement.Parse(DatosDemograficos.ToString());
                    }

                    var listaTipificacion = Utility.GeneraTipificacion(_listaSeleccion);
                    if (listaTipificacion != null)
                    {
                        tipificacion.TIPIFICACION = XElement.Parse(listaTipificacion.ToString());
                    }
                    //tipificacion.VALOR_KEY = "188652565";
                    //tipificacion.DICCIONARIO_ID_KEY = "1";
                    //tipificacion.TIPIFICACION_CUSTOM = null;
                }
                catch (Exception)
                {

                }

                EResultado resultado = null;
                resultado = metodosAPi.Tipifcacion(out resultado, tipificacion, InteractionExtensionCrossnetModule.token);


                if (resultado != null)
                {
                    if (resultado.codigo.Equals(2))
                        GenesysAlert.SendMessage("Registro de Tipificación insertado con éxito", _caseIdentifier.CaseId,
                            SeverityType.Information);
                    else
                        GenesysAlert.SendMessage("Error al insertar Registro", _caseIdentifier.CaseId, SeverityType.Error);
                    //Codigos 0: ok 1: insert 2: update 3: delete 10: error SqlException 11 : otros errores
                }



                var numero = "";
                if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                    numero = _inxDatos.GenOrigen;
                if (_inxDatos.GenCanalEntrada.Equals("Outbound") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview") || _inxDatos.GenCanalEntrada.Equals("InteractionPushPreview"))
                    numero = _inxDatos.GenDestino;

                EResultado resultadoHistorial = null;
                GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, _inxDatos.GenCanalEntrada, numero, "D", "ani", "5", 161).Historial;
  
                _metodos.ActualizaRegistroMarkDone(_caseIdentifier, _inxDatos);
                cargaHistorialInicio();
       
           
        }

        private void ActualizarDatosInteraccion()
        {
            if (_caseIdentifier.MainInteraction.HasOutboundChainRecord)
            {
                if (_caseIdentifier.MainInteraction.Type.Equals("InteractionPullPreview"))
                {
                    _inxDatos.GenEventoCampana = "PreviewRecord";
                    _inxDatos.GenConnid = _inxDatos.GenGuid;
                }
                else
                {
                    _inxDatos.GenConnid = _caseIdentifier.MainInteraction.EntrepriseInteractionCurrent.Id;
                    _inxDatos.GenEventoCampana = _caseIdentifier.MainInteraction.Type;
                    _inxDatos.GenEvento = _caseIdentifier.MainInteraction.Type;
                }
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            var tip = Utility.FindChildControl<StackPanel>(Tipificaciones);
            SetNodeExpandedState(tip.Children, false);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var check = (CheckBox)sender;
                var panel = Utility.FindParent<StackPanel>(check);
                var nombrepan = panel.Name;

                foreach (var chidren in panel.Children)
                    if (chidren is Expander && (panel.Parent.GetType() != typeof(Expander)))
                    {
                        var childrenCheck = (Expander)chidren;
                        if (!check.Content.Equals(childrenCheck.Header))
                        {
                            /* para que sea excluyente
                             * childrenCheck.IsExpanded = false;
                            if (childrenCheck.Content != null)
                            {
                                var stack = (StackPanel)childrenCheck.Content;
                                SetNodeExpandedState(stack.Children, false);
                            }*/
                        }
                        else
                        {
                            childrenCheck.IsExpanded = true;
                        }
                    }
                _listaSeleccion.Add(new TipificacionItem(check.Tag.ToString(), check.Content.ToString(), check.Uid,
                    check.ToolTip.ToString()));

                //////con este foreach se hace excluyente el nivel 2
                ////foreach (var chidren in panel.Children)
                ////{
                ////    if (chidren is Expander)
                ////    {
                ////        Expander childrenCheck = (Expander)chidren;

                ////        if (check.Content.Equals(childrenCheck.Header))
                ////        {
                ////            if (childrenCheck.Content != null)
                ////            {
                ////                StackPanel stack = (StackPanel)childrenCheck.Content;
                ////                this.SetNodeExpandedState(stack.Children, false);
                ////            }
                ////        }
                ////        else
                ////        {
                ////            childrenCheck.IsExpanded = false;
                ////        }
                ////    }
                ////}
            }
            catch (Exception error)
            {
                GenesysAlert.SendMessage("Se ha producido un error al obtener las tipificaciones2: " + error.Message,
                    _caseIdentifier.CaseId, SeverityType.Error);
            }
        }

        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Tipificaciones.Content != null)
                {
                    var check = (CheckBox)sender;
                    var panel = Utility.FindParent<StackPanel>(check);

                    foreach (var item in panel.Children)
                        if (item is Expander)
                        {
                            var hijos = (Expander)item;
                            if (hijos.Header.Equals(check.Content.ToString()))
                                if (hijos.Content != null)
                                {
                                    var subitems = (StackPanel)hijos.Content;
                                    SetNodeExpandedState(subitems.Children, false);
                                }
                        }
                    var name = panel.Name;
                    //this.SetNodeExpandedState(panel.Children, false);
                    var itemToRemove =
                        _listaSeleccion.Single(
                            r =>
                                (r.Value == check.Tag.ToString()) && (r.Dependiente == check.Uid.ToString()) &&
                                (r.Padre == check.ToolTip.ToString()));

                    _listaSeleccion.Remove(itemToRemove);
                }
            }
            catch (Exception error)
            {
                GenesysAlert.SendMessage(
                    "Se ha producido un error en la jerarquia de tipificaciones: " + error.Message,
                    _caseIdentifier.CaseId, SeverityType.Error);
            }
        }

        private void SetNodeExpandedState(UIElementCollection nodes, bool expandNode)
        {
            foreach (UIElement item in nodes)
            {
                if (item is Expander)
                {
                    var itemHIjo = (Expander)item;
                    itemHIjo.IsExpanded = expandNode;
                    if (itemHIjo.Content != null)
                    {
                        var stack = (StackPanel)itemHIjo.Content;
                        SetNodeExpandedState(stack.Children, false);
                    }
                }
                if (item is CheckBox)
                {
                    var itemHIjo = (CheckBox)item;
                    itemHIjo.IsChecked = expandNode;
                }
            }
        }

        private void TextBoxObservaciones_TextChanged(object sender, TextChangedEventArgs e)
        {
            LabelNumeroCaracteres.Content = 500;
            LabelNumeroCaracteres.Content = Convert.ToInt32(LabelNumeroCaracteres.Content) - TextBoxObservacion.Text.Length;

            if (TextBoxObservacion.Text.Length == 500)
            {
                LabelNumeroCaracteres.Foreground = Brushes.Red;
                LabelTituloCaracteres.Foreground = Brushes.Red;
            }
            else
            {
                LabelNumeroCaracteres.Foreground = Brushes.Black;
                LabelTituloCaracteres.Foreground = Brushes.Black;
            }
        }

        private void ButtonBuscarRut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TipificacionesView_Loaded(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(TextBoxRut.Text, true);           
        }

        public void LoadFilter()
        {
            try
            {
                cbFiltro.ItemsSource = InteractionExtensionCrossnetModule.listaFiltro;
                cbFiltro.DisplayMemberPath = "descripcion";
            }
            catch (Exception)
            {

            }
        }

        private void cargaHistorialInicio()
        {
            var numero = "";
            var key = "";

            if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                key = "ANI";
            if (_inxDatos.GenCanalEntrada.Equals("Outbound") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview") || _inxDatos.GenCanalEntrada.Equals("InteractionPushPreview")|| _inxDatos.GenCanalEntrada.Equals("PreviewRecord"))
                key = "DNIS";

            if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                numero = _inxDatos.GenOrigen;
            if (_inxDatos.GenCanalEntrada.Equals("Outbound") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview")|| _inxDatos.GenCanalEntrada.Equals("InteractionPushPreview") || _inxDatos.GenCanalEntrada.Equals("PreviewRecord"))
              numero = _inxDatos.GenDestino;
            EResultado resultadoHistorial = null;
            
            double indexSlider = sliderHistorial.Value;
            string periodo = "";
            string filtro = "";
            switch (indexSlider)
            {
                case 0:
                    periodo = "T";
                    break;
                case 1:
                    periodo = "M";
                    break;
                case 2:
                    periodo = "S";
                    break;
                case 3:
                    periodo = "D";
                    break;
                default:
                    periodo = "T";
                    break;
            }
            if (cbFiltro.Text == "")
            {
                filtro = "Todo";
            }
            else { filtro = cbFiltro.Text; }
            //GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ANI", "5").Historial;
            GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, key, "5", 161).Historial;
        }
        private void SliderHistorial_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var numero = "";
            if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                numero = _inxDatos.GenOrigen;
            if (_inxDatos.GenCanalEntrada.Equals("Outbound") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview"))
                numero = _inxDatos.GenDestino;
            EResultado resultadoHistorial = null;
            double indexSlider = sliderHistorial.Value;
            string periodo = "";
            string filtro = "";
            switch (indexSlider)
            {
                case 0:
                    periodo = "T";
                    break;
                case 1:
                    periodo = "M";
                    break;
                case 2:
                    periodo = "S";
                    break;
                case 3:
                    periodo = "D";
                    break;
                default:
                    periodo = "T";
                    break;
            }
            if (cbFiltro.Text == "")
            {
                filtro = "Todo";
            }else { filtro = cbFiltro.Text; }
            //GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ani", "5").Historial;
            GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ani", "5", 161).Historial;
        }

        private void CbTop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string top = "";
            if (cbTop.SelectedValue != null)
                top = cbTop.SelectedValue.ToString();
            else
                top = "5";
            var numero = "";
            if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                numero = _inxDatos.GenOrigen;
            if (_inxDatos.GenCanalEntrada.Equals("Outbound") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview"))
                numero = _inxDatos.GenDestino;
            EResultado resultadoHistorial = null;
            string filtro = "";
            if (cbFiltro.Text == "")
            {
                filtro = "Todo";
            }
            else { filtro = cbFiltro.Text; }
            double indexSlider = sliderHistorial.Value;
            string periodo = "";
            switch (indexSlider)
            {
                case 0:
                    periodo = "T";
                    break;
                case 1:
                    periodo = "M";
                    break;
                case 2:
                    periodo = "S";
                    break;
                case 3:
                    periodo = "D";
                    break;
                default:
                    periodo = "T";
                    break;
            }

            //GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ani",top).Historial;
            GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ani", top, 161).Historial;
        }

        private void CbFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string top = "";
            if (cbTop.SelectedValue != null)
                top = cbTop.SelectedValue.ToString();
            else
                top = "5";
            var numero = "";
            if (_inxDatos.GenCanalEntrada.Equals("Inbound"))
                numero = _inxDatos.GenOrigen;
            if (_inxDatos.GenCanalEntrada.Equals("Outbound") || _inxDatos.GenCanalEntrada.Equals("InteractionPullPreview"))
                numero = _inxDatos.GenDestino;
            EResultado resultadoHistorial = null;
            string filtro = "";
            if (cbFiltro.SelectedValue != null)
            {
                filtro = cbFiltro.SelectedValue.ToString();
                
            }
            else { filtro = "Todo"; }
            double indexSlider = sliderHistorial.Value;
            string periodo = "";
            switch (indexSlider)
            {
                case 0:
                    periodo = "T";
                    break;
                case 1:
                    periodo = "M";
                    break;
                case 2:
                    periodo = "S";
                    break;
                case 3:
                    periodo = "D";
                    break;
                default:
                    periodo = "T";
                    break;
            }

            //GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ani", top).Historial;
            GridHistorialTipificaciones.ItemsSource = metodosAPi.Historial(out resultadoHistorial, InteractionExtensionCrossnetModule.token, filtro, numero, periodo, "ani", top, 161).Historial;
        }
    }
}