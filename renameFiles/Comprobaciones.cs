using System;
using Windows.UI.Xaml.Controls;

public class Comprobaciones
{

    // Comprobaciones para Remplazar Texto
    public bool CheckRemplazarTexto(RadioButton rbVacio, RadioButton rbTexto, TextBox textRemplazar, TextBox textDestino)
    {
        // Comprobar Radio Buttons
        if (rbVacio.IsChecked == rbTexto.IsChecked)
        {
            MostrarErrorAsync("Solo puede seleccionar un modo de remplazar texto.");
            return false;
        }

        // Comprobar TextBox
        if (textRemplazar.Text.Equals(""))
        {
            MostrarErrorAsync("El campo del texto que se va a remplazar no puede estar vacio.");
            return false;
        }

        return true;
    }

    // Comprobaciones para Añadir Texto
    public bool CheckAñadirTexto(RadioButton rbAñadirPrincipio, RadioButton rbAñadirFinal, TextBox textBoxAñadirTexto)
    {
        // Comprobar Radio Buttons
        if (rbAñadirPrincipio.IsChecked == rbAñadirFinal.IsChecked)
        {
            MostrarErrorAsync("Solo puede seleccionar un modo de Añadir Texto.");
            return false;
        }

        // Comprobar TextBox
        if (textBoxAñadirTexto.Text.Equals(""))
        {
            MostrarErrorAsync("El campo del texto que se va a añadir no puede estar vacio.");
            return false;
        }

        return true;
    }

    public async System.Threading.Tasks.Task MostrarErrorAsync(String mensaje)
    {
        ContentDialog notifyDialog = new ContentDialog
        {
            Title = "Error",
            Content = mensaje,
            CloseButtonText = "Aceptar",
            DefaultButton = ContentDialogButton.Close
        };
        await notifyDialog.ShowAsync();
    }
}
