using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnalizadorLexico.Lexico
{
    public class Analizador
    {
        public List<Token> Analizar(string codigo)
        {
            var tokens = new List<Token>();
            var lineas = codigo.Split('\n');

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i];

                List<string> lexemas = PreprocesarLinea(linea);

                int columna = 1;
                foreach (var lexema in lexemas)
                {
                    TokenType tipo = ClasificarToken(lexema);
                    tokens.Add(new Token(tipo, lexema, i + 1, columna));
                    columna += lexema.Length + 1;
                }
            }

            return tokens;
        }

        private List<string> PreprocesarLinea(string linea)
        {
            List<string> resultado = new List<string>();
            string actual = "";
            bool enCadena = false;
            bool enCaracter = false;

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                // Manejo de cadenas
                if (c == '"' && !enCaracter)
                {
                    actual += c;
                    if (enCadena)
                    {
                        resultado.Add(actual);
                        actual = "";
                        enCadena = false;
                    }
                    else
                    {
                        enCadena = true;
                    }
                    continue;
                }

                // Manejo de caracteres
                if (c == '\'' && !enCadena)
                {
                    actual += c;
                    if (enCaracter)
                    {
                        resultado.Add(actual);
                        actual = "";
                        enCaracter = false;
                    }
                    else
                    {
                        enCaracter = true;
                    }
                    continue;
                }

                if (enCadena || enCaracter)
                {
                    actual += c;
                    continue;
                }

                // Espacios
                if (char.IsWhiteSpace(c))
                {
                    if (!string.IsNullOrEmpty(actual))
                    {
                        resultado.Add(actual);
                        actual = "";
                    }
                    continue;
                }

                // Operadores de 2 caracteres
                if (i + 1 < linea.Length && LenguajeDefinido.Operadores.Contains(c.ToString() + linea[i + 1]))
                {
                    if (!string.IsNullOrEmpty(actual))
                    {
                        resultado.Add(actual);
                        actual = "";
                    }
                    resultado.Add(c.ToString() + linea[i + 1]);
                    i++;
                    continue;
                }

                // Operadores de 1 caracter
                if (LenguajeDefinido.Operadores.Contains(c.ToString()) || LenguajeDefinido.Signos.Contains(c.ToString()))
                {
                    if (!string.IsNullOrEmpty(actual))
                    {
                        resultado.Add(actual);
                        actual = "";
                    }
                    resultado.Add(c.ToString());
                    continue;
                }

                // Acumular identificadores y números
                actual += c;
            }

            if (!string.IsNullOrEmpty(actual))
                resultado.Add(actual);

            return resultado;
        }

        private TokenType ClasificarToken(string token)
        {
            token = token.Trim();

            if (LenguajeDefinido.PalabrasReservadas.Contains(token))
                return TokenType.PalabraReservada;

            if (LenguajeDefinido.Operadores.Contains(token))
                return TokenType.Operador;

            if (LenguajeDefinido.NumeroDecimal.IsMatch(token))
                return TokenType.Decimal;

            if (LenguajeDefinido.NumeroEntero.IsMatch(token))
                return TokenType.Entero;

            if (LenguajeDefinido.Cadena.IsMatch(token))
                return TokenType.Cadena;

            if (LenguajeDefinido.Caracter.IsMatch(token))
                return TokenType.Caracter;

            if (LenguajeDefinido.Booleano.IsMatch(token))
                return TokenType.Entero; // o TokenType.Booleano si lo agregas

            if (LenguajeDefinido.Identificador.IsMatch(token))
                return TokenType.Identificador;

            if (LenguajeDefinido.Signos.Contains(token))
                return TokenType.Signo;

            return TokenType.Error;
        }
    }
}
