using System;
using System.Collections.Generic;

namespace AnalizadorLexico.Semantico
{
    public class TablaSimbolos
    {
        private readonly Dictionary<string, Simbolo> simbolos = new Dictionary<string, Simbolo>();

        public void DeclararVariable(string nombre, string tipo, int linea, int columna)
        {
            if (simbolos.ContainsKey(nombre))
                throw new Exception($"Error semántico: Variable '{nombre}' ya declarada (Línea {linea}, Columna {columna})");
            
            simbolos[nombre] = new Simbolo(nombre, tipo, linea, columna);
        }

        public Simbolo ObtenerSimbolo(string nombre)
        {
            return simbolos.ContainsKey(nombre) ? simbolos[nombre] : null;
        }

        public bool ExisteVariable(string nombre)
        {
            return simbolos.ContainsKey(nombre);
        }
    }

    public class Simbolo
    {
        public string Nombre { get; }
        public string Tipo { get; }
        public int LineaDeclaracion { get; }
        public int ColumnaDeclaracion { get; }

        public Simbolo(string nombre, string tipo, int linea, int columna)
        {
            Nombre = nombre;
            Tipo = tipo;
            LineaDeclaracion = linea;
            ColumnaDeclaracion = columna;
        }
    }
}