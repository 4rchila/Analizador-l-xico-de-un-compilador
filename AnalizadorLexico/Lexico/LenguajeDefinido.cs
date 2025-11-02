using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnalizadorLexico.Lexico
{
    public static class LenguajeDefinido
    {
        public static readonly List<string> TiposDatos = new List<string>()
        {
            "entero", "decimal", "booleano", "cadena", "caracter",
            "Entero", "Decimal", "Booleano", "Cadena", "Caracter"
        };

        public static readonly List<string> PalabrasReservadas = new List<string>()
        {
            "verdadero", "falso", "nulo",
            "si", "sino", "mientras", "hacer", "para", "romper", "continuar",
            "funcion", "devolver",
            "intentar", "atrapar",
            "imprimir", "entrada",
            "clase", "nuevo", "publico", "privado", "protegida", "estatica", "esto",
            "o", "y",

            "Verdadero", "Falso", "Nulo",
            "Si", "Sino", "Mientras", "Hacer", "Para", "Romper", "Continuar",
            "Funcion", "Devolver",
            "Intentar", "Atrapar",
            "Imprimir", "Entrada",
            "Clase", "Nuevo", "Publico", "Privado", "Protegida", "Estatica", "Esto",
            "O", "Y"
        };

        public static readonly List<string> Operadores = new List<string>()
        {
            "+", "-", "*", "/", "%", "=", "==", "!=", "<", ">", "<=", ">="
        };

        public static readonly List<string> Signos = new List<string>()
        {
            "(", ")", "{", "}", ",", ";", "\"", "'", "."
        };

        public static readonly Regex NumeroEntero = new Regex(@"^\d+$");
        public static readonly Regex NumeroDecimal = new Regex(@"^\d+\.\d+$");
        public static readonly Regex Identificador = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        public static readonly Regex Cadena = new Regex("^\".*\"$");
        public static readonly Regex Caracter = new Regex(@"^'.{1}'$");
        public static readonly Regex Booleano = new Regex(@"^(verdadero|falso)$");
    }
}



