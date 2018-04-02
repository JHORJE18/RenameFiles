using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace renameFiles
{

    public sealed partial class MainPage : Page
    {
        //Variables
        List<StorageFile> listadoFicheros;
        List<String> listadoPrevio;
        Operaciones ops;
        Comprobaciones check;

        public MainPage()
        {
            this.InitializeComponent();
            listadoFicheros = new List<StorageFile>();
            listadoPrevio = new List<String>();
            ops = new Operaciones();
            check = new Comprobaciones();

            //prevListArchivos.ItemsSource = listadoPrevio;
            rbRemplazarTexto.IsChecked = true;
            rbAñadirFinal.IsChecked = true;
        }

        // Subrutina gestiona boton eliminar disponible o no
        private void listArchivos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Comprueba si esta seleccionado algun item
            Debug.WriteLine("Tienes seleecionado items " + listArchivos.SelectedItems.Count);
            if (listArchivos.SelectedItems.Count > 0)
            {
                btnRemoveFiles.IsEnabled = true;
            }
            else
            {
                btnRemoveFiles.IsEnabled = false;
            }
        }

        private async System.Threading.Tasks.Task openFileAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            openPicker.CommitButtonText = "Añadir archivos";
            openPicker.FileTypeFilter.Add("*");

            IReadOnlyList<StorageFile> listFiles;
            listFiles = await openPicker.PickMultipleFilesAsync();

            for (int i = 0; i < listFiles.Count; i++)
            {
                //Comprobar que fichero no esta añadido
                if (!FicheroEnListado(listFiles.ElementAt(i)))
                {
                    //Añadimos fichero al listado
                    listadoFicheros.Add(listFiles.ElementAt(i));
                    listArchivos.Items.Add(listFiles.ElementAt(i).DisplayName);
                }
            }
        }

        private Boolean FicheroEnListado(StorageFile analizando)
        {
            //Analiza existencia
            for (int i=0; i<listadoFicheros.Count; i++)
            {
                if (listadoFicheros.ElementAt(i).IsEqual(analizando))
                {
                    //Ya esta incluido anteriormente
                    return true;
                }
            }

            return false;
        }

        public async System.Threading.Tasks.Task MostrarDialogoInfo()
        {
            Analytics.TrackEvent("Consultar información app");

            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "Información RenameFILES",
                Content = "renameFILES es una app creada por Jorge López Gil de útilidad para renombrar ficheros de forma masiva." + 
                Environment.NewLine + "Se ha realizado esta app con fines de aprender a usar C#, de forma que es totalmente gratuita y sin anuncios." + 
                Environment.NewLine + Environment.NewLine + "En la Web puedes encontrar más información y el código fuente que esta alojado en GitHub.",
                PrimaryButtonText = "Visitar Web",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                //Abrir Web
                var urlWeb = new Uri(@"http://jhorje18.com/portfolio/renamefiles");

                await Windows.System.Launcher.LaunchUriAsync(urlWeb);
            }
        }

        private async System.Threading.Tasks.Task MostrarDialogoSalirAsync()
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "¿Seguro que quieres salir?",
                Content = "Se finalizaran todas las tareas y perderas el estado actual.",
                PrimaryButtonText = "Salir",
                CloseButtonText = "Cancelar"
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Application.Current.Exit();
            }

        }

        // #Controlador RadioButtons
        public void controladorRadioButtons(object sender, RoutedEventArgs e)
        {
            RadioButton refRadioButton = (RadioButton)sender;

            Debug.WriteLine("Cargando RadioButtons");

            // Control
            switch (refRadioButton.Name)
            {
                // Remplazar Texto
                case "rbRemplazarVacio":
                    rbRemplazarTexto.IsChecked = false;
                    textBoxRemplazarOtroTexto.IsEnabled = false;
                    break;
                case "rbRemplazarTexto":
                    rbRemplazarVacio.IsChecked = false;
                    textBoxRemplazarOtroTexto.IsEnabled = true;
                    break;

                // Añadir Texto
                case "rbAñadirPrincipio":
                    rbAñadirFinal.IsChecked = false;
                    break;
                case "rbAñadirFinal":
                    rbAñadirPrincipio.IsChecked = false;
                    break;
            }
        }

        // # Controlador Buttons
        public void ControladorButtons(object sender, RoutedEventArgs e)
        {
            Button refButtons = (Button)sender;
            Debug.WriteLine("Vengo del botón " + refButtons.Name);

            switch (refButtons.Name)
            {
                //Menu lateral
                case "btnMenuInfo":
                    MostrarDialogoInfo();
                    break;
                case "btnMenuExit":
                    MostrarDialogoSalirAsync();
                    break;

                // Panel archivos
                case "btnAddFiles":
                    openFileAsync();
                    break;
                case "btnRemoveFiles":
                    BorrarFiles();
                    break;

                // Panel Resumen
                case "btnVistaPrevia":
                    RefrescarVistaPrevia();
                    break;
                case "btnAplicar":
                    // TODO: Iniciar conversión
                    break;

                // Panel Remplazar texto
                case "btnAplicarRemplazar":
                    switchRemplazar.IsOn = true;
                    RefrescarVistaPrevia();
                    break;

                // Panel Añadir texto
                case "btnAplicarAñadirTexto":
                    switchAñadir.IsOn = true;
                    RefrescarVistaPrevia();
                    break;


                default:
                    Debug.WriteLine("No entiendo de que botón he venido");
                    break;
            }
        }

        // Elimina ficheros seleccionados de la lista
        private void BorrarFiles()
        {
            // Borra los elementos que esten seleccionados
            Debug.WriteLine("Procediendo a borrar " + listArchivos.SelectedItems.Count + " elementos");
            while (listArchivos.SelectedItems.Count > 0)
            {
                Debug.WriteLine("Archivo >> " + listArchivos.SelectedItems.First() + " << eliminado");
                int index = listArchivos.Items.IndexOf(listArchivos.SelectedItems.First());

                //Eliminamos eleemnto
                listArchivos.Items.RemoveAt(index);
                listadoFicheros.RemoveAt(index);
            }
        }

        // # Controlador de Switch's
        public void controladorSwitchs(object sender, RoutedEventArgs e)
        {
            ToggleSwitch refToogle = (ToggleSwitch)sender;
            Boolean valorNuevo = refToogle.IsOn;
            Debug.WriteLine("Vengo de parte de " + refToogle.Name );

            // Comprobamos a cual pertenece
            switch (refToogle.Name)
            {
                case "switchResumenRemplazar":
                case "switchRemplazar":
                    switchResumenRemplazar.IsOn = valorNuevo;
                    switchRemplazar.IsOn = valorNuevo;
                    break;
                case "switchResumenAñadir":
                case "switchAñadir":
                    switchResumenAñadir.IsOn = valorNuevo;
                    switchAñadir.IsOn = valorNuevo;
                    break;
                case "switchResumenEliminar":
                case "switchEliminar":
                    switchResumenEliminar.IsOn = valorNuevo;
                    switchEliminar.IsOn = valorNuevo;
                    break;
                case "switchResumenMayusculas":
                case "switchMayusculas":
                    switchResumenMayusculas.IsOn = valorNuevo;
                    switchMayusculas.IsOn = valorNuevo;
                    break;
                default:
                    Debug.WriteLine("Switch desconocido");
                    break;
            }
        }

        // Proceso para refrescar vista previa
        public void RefrescarVistaPrevia()
        {
            // Eliminamos listado previo
            listadoPrevio.Clear();
            prevListArchivos.Items.Clear();

            // Añadimos todos los elementos
            for (int i=0; i<listadoFicheros.Count; i++)
            {
                listadoPrevio.Add(listadoFicheros[i].DisplayName);
            }

            // Control que acciones se han activado realizar
            if (switchResumenRemplazar.IsOn)
            {
                bool valido = check.CheckRemplazarTexto(rbRemplazarVacio, rbRemplazarTexto, textBoxRemplazarTextoOrigen, textBoxRemplazarOtroTexto);

                if (valido) {
                    Analytics.TrackEvent("Remplazar texto");

                    // Preparativos
                    string caracterCambiar = textBoxRemplazarTextoOrigen.Text;
                    string caracterNuevo = "";
                    bool isDiferenciaMayusculas = switchRemplazarMayusculas.IsOn;

                    //Comprobar mdoo de remplazar
                    if ((bool)rbRemplazarTexto.IsChecked)
                    {
                        caracterNuevo = textBoxRemplazarOtroTexto.Text;
                    }
                    else if ((bool)rbRemplazarVacio.IsChecked)
                    {
                        caracterNuevo = "";
                    }
                    else
                    {
                        // TODO: ERROR NINGUNO SELECCIONADO
                    }

                    // Recorremos todos los archivos
                    for (int i = 0; i < listadoPrevio.Count; i++)
                    {
                        Debug.WriteLine("-------------------------------------------------------");
                        Debug.WriteLine("Voy a proceder a remplazar texto de ==> " + listadoPrevio[i]);

                        if (ops.CheckCoincidenciaTexto(listadoPrevio[i], caracterCambiar, isDiferenciaMayusculas))
                        {
                            // Procedemos a remplazar texto
                            string resultado = ops.RemplazarTexto(listadoPrevio[i], caracterCambiar, caracterNuevo, isDiferenciaMayusculas);
                            Debug.WriteLine("Se ha cambiado a ==> " + resultado);
                            listadoPrevio[i] = resultado;
                        }
                        else
                        {
                            // Pasamos al siguiente
                            listadoPrevio[i] = listadoPrevio[i];
                            Debug.WriteLine("Ignorado :D");
                        }

                        Debug.WriteLine("-------------------------------------------------------");
                    }
                }
                else
                {
                    // No valido
                    switchResumenRemplazar.IsOn = false;
                    Debug.WriteLine("Algo no esta correcto, por lo que no hago nada :D");
                }
            }

            if (switchResumenAñadir.IsOn)
            {
                bool valido = check.CheckAñadirTexto(rbAñadirPrincipio, rbAñadirFinal, textBoxAñadirTexto);

                if (valido)
                {
                    Analytics.TrackEvent("Añadir texto");

                    bool textoFinal = (bool)rbAñadirFinal.IsChecked;
                    
                    // Procedemos a realizar acción en todos los ficheros
                    for (int i = 0; i<listadoPrevio.Count; i++)
                    {
                        Debug.WriteLine("-------------------------------------------------------");
                        Debug.WriteLine("Voy a proceder a añadir el texto al archivo ==> " + listadoFicheros[i].DisplayName);

                        string resultado = ops.AñadirTexto(listadoPrevio[i], textBoxAñadirTexto.Text, textoFinal);
                        Debug.WriteLine("Se ha cambiado a ==> " + resultado);
                        listadoPrevio[i] = resultado;

                        Debug.WriteLine("-------------------------------------------------------");

                    }
                }
                else
                {
                    // No valido
                    switchResumenAñadir.IsOn = false;
                    Debug.WriteLine("Algo no esta correcto en Añadir Texto, asi que mejor no hago nada :D");
                }

            }

            if (switchResumenEliminar.IsOn)
            {
                Analytics.TrackEvent("Eliminar caracteres");

            }

            if (switchResumenMayusculas.IsOn)
            {
                Analytics.TrackEvent("Cambiar Mayusculas y minusculas");


            }

            Debug.WriteLine("Listado previo actualizado con actualmente " + listadoPrevio.Count + " elementos");

            for (int i=0; i<listadoPrevio.Count; i++)
            {
                prevListArchivos.Items.Add(listadoPrevio[i]);
                Debug.WriteLine("Elemento nº " + (i+1) + " ==> " + listadoPrevio[i]);
            }
        }

    }
}
