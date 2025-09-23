using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnalizadorLexico.Lexico
{
    public static class LenguajeDefinido
    {
        // =============================
        // PALABRAS RESERVADAS
        // =============================
        public static readonly List<string> PalabrasReservadas = new List<string>()
        {
            "entero", "decimal", "booleano", "cadena", "caracter",
            "verdadero", "falso", "nulo",
            "si", "sino", "mientras", "hacer", "para", "romper", "continuar",
            "funcion", "devolver",
            "intentar", "atrapar",
            "imprimir", "entrada",
            "clase", "nuevo", "publico", "privado", "protegida", "estatica", "esto",
            "o", "y"
        };

        // =============================
        // OPERADORES
        // =============================
        public static readonly List<string> Operadores = new List<string>()
        {
            "+", "-", "*", "/", "%", "=", "==", "!=", "<", ">", "<=", ">="
        };

        // =============================
        // SIGNOS
        // =============================
        public static readonly List<string> Signos = new List<string>()
        {
            "(", ")", "{", "}", ",", ";", "\"", "'", "."
        };

        // =============================
        // EXPRESIONES REGULARES
        // =============================
        public static readonly Regex NumeroEntero = new Regex(@"^\d+$");
        public static readonly Regex NumeroDecimal = new Regex(@"^\d+\.\d+$");
        public static readonly Regex Identificador = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        public static readonly Regex Cadena = new Regex("^\".*\"$");
        public static readonly Regex Caracter = new Regex(@"^'.{1}'$");
        public static readonly Regex Booleano = new Regex(@"^(verdadero|falso)$");
    }
}


