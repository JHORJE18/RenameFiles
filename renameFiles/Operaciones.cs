using System;

public class Operaciones
{
    // # Metodos públicos

    // Check coincidencia
    public bool CheckCoincidenciaTexto(string cadenaOriginal, string cadenaComparar, bool diferenciarMayusculas)
    {

        if (diferenciarMayusculas)
        {
            StringComparison comp = StringComparison.Ordinal;

            int i = cadenaOriginal.IndexOf(cadenaComparar, comp);

            if (i > -1) return true;
        }
        else
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            int i = cadenaOriginal.IndexOf(cadenaComparar, comp);

            if (i > -1) return true;
        }

        return false;
    }
    
    // Remplazar texto
    public string RemplazarTexto(string cadenaOriginal, string cadenaRemplazar, string cadenaDestino, bool diferenciaMayusculas)
    {
        String resultado = "";

        while (CheckCoincidenciaTexto(cadenaOriginal, cadenaRemplazar, diferenciaMayusculas))
        {
            // Diferenciar Mayusculas y minusculas??
            if (diferenciaMayusculas)
            {
                StringComparison comp = StringComparison.Ordinal;

                //cadenaOriginal.Contains(cadenaDestino, comp);
                int i = cadenaOriginal.IndexOf(cadenaRemplazar, comp);

                // Principio cadena
                resultado = cadenaOriginal.Substring(0, i);

                // Media cadena
                resultado += cadenaDestino;

                // Final cadena
                resultado += cadenaOriginal.Substring((i + cadenaRemplazar.Length));
            }
            else
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;

                int i = cadenaOriginal.IndexOf(cadenaRemplazar, comp);

                // Principio cadena
                resultado = cadenaOriginal.Substring(0, i);

                // Media cadena
                resultado += cadenaDestino;

                // Final cadena
                resultado += cadenaOriginal.Substring((i + cadenaRemplazar.Length));
            }

            cadenaOriginal = resultado;
        }

        return resultado;
    }

    // Añadir Texto
    public string AñadirTexto(string nombreOriginal, string textoAdd, bool textoFinal)
    {
        string resultado = "";

        if (textoFinal)
        {
            resultado = nombreOriginal + textoAdd;
        } else
        {
            resultado = textoAdd + nombreOriginal;
        }

        return resultado;
    }
}