using System.Collections.Generic;
using System.Text;

namespace AnalizadorLexico.Sintactico
{
    public class NodoSintactico
    {
        public string Etiqueta { get; set; }
        public List<NodoSintactico> Hijos { get; set; } = new();

        public NodoSintactico(string etiqueta)
        {
            Etiqueta = etiqueta;
        }

        public void AgregarHijo(NodoSintactico hijo)
        {
            Hijos.Add(hijo);
        }

        public string Imprimir(string prefijo = "", bool esUltimo = true)
        {
            var sb = new StringBuilder();

            sb.Append(prefijo);
            sb.Append(esUltimo ? "└─" : "├─");
            sb.AppendLine(Etiqueta);

            for (int i = 0; i < Hijos.Count; i++)
            {
                sb.Append(Hijos[i].Imprimir(prefijo + (esUltimo ? "  " : "│ "), i == Hijos.Count - 1));
            }

            return sb.ToString();
        }
    }
}
