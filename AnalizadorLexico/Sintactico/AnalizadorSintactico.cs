using System;
using System.Collections.Generic;
using System.Linq;
using AnalizadorLexico.Lexico;

namespace AnalizadorLexico.Sintactico
{
    public static class AnalizadorSintactico
    {
        public class Parser
        {
            private readonly List<Token> _tokens;
            private int _pos;
            public readonly List<string> Errores = new();

            public NodoSintactico RaizAST { get; private set; } = new NodoSintactico("Programa");

            public Parser(List<Token> tokens)
            {
                _tokens = tokens ?? new List<Token>();
                _tokens.Add(new Token(TokenType.Error, "<EOF>", 0, 0));
                _pos = 0;
            }

            private Token Peek(int offset = 0)
            {
                int idx = _pos + offset;
                if (idx >= _tokens.Count)
                    return new Token(TokenType.Error, "<EOF>", 0, 0);
                return _tokens[idx];
            }

            private Token Advance()
            {
                var t = Peek();
                if (_pos < _tokens.Count) _pos++;
                return t;
            }

            private bool IsAtEnd() => Peek().Lexema == "<EOF>";

            private bool MatchLexema(params string[] lexemas)
            {
                var p = Peek();
                foreach (var l in lexemas)
                {
                    if (p.Lexema == l)
                    {
                        Advance();
                        return true;
                    }
                }
                return false;
            }

            private bool MatchTipo(TokenType tipo)
            {
                if (Peek().Tipo == tipo)
                {
                    Advance();
                    return true;
                }
                return false;
            }

            private void Error(string msg)
            {
                var t = Peek();
                Errores.Add($"{msg} en {t.Linea}:{t.Columna} (token '{t.Lexema}')");
                if (!IsAtEnd()) Advance();
            }

            // ---------------- PROGRAMA ----------------
            public bool ParseProgram()
            {
                RaizAST = new NodoSintactico("Programa");
                ParseDeclaraciones(RaizAST);
                return Errores.Count == 0;
            }

            private void ParseDeclaraciones(NodoSintactico padre)
            {
                while (!IsAtEnd() && Peek().Lexema != "}")
                {
                    if (CanStartDeclaration(Peek()))
                        padre.AgregarHijo(ParseDeclaracion());
                    else
                    {
                        if (Peek().Lexema == "}") break;
                        Error("Inicio de declaración no válido");
                        Advance();
                    }
                }
            }

            private bool CanStartDeclaration(Token t)
            {
                if (t.Tipo == TokenType.Identificador) return true;
                if (t.Tipo == TokenType.PalabraReservada || t.Tipo == TokenType.TipoDato)
                {
                    string[] starters =
                    {
            "entero","decimal","booleano","cadena","caracter",
            "Entero","Decimal","Booleano","Cadena","Caracter",
            "funcion","si","sino","mientras","para","intentar",
            "romper","continuar","devolver","imprimir","entrada",
            "clase"
        };
                    return starters.Contains(t.Lexema);
                }
                return false;
            }


            private NodoSintactico ParseDeclaracion()
            {
                var t = Peek();

                if (IsTipo(t)) return ParseDeclaracionVariable();
                if (t.Lexema == "funcion") return ParseDeclaracionFuncion();
                if (t.Lexema == "clase") return ParseDeclaracionClase();
                if (t.Lexema is "si" or "mientras" or "para" or "intentar") return ParseEstructuraControl();
                if (t.Lexema is "romper" or "continuar" or "devolver" or "imprimir" or "entrada") return ParseSentenciaControl();

                // expresión sola
                var nodo = new NodoSintactico("Expresión");
                ParseExpresion();
                if (!MatchLexema(";")) Error("Se esperaba ';' después de la expresión");
                return nodo;
            }

            // ------------- TIPOS Y VARIABLES -------------
            public bool IsTipo(Token t)
            {
                string[] tipos =
                {
        "entero","decimal","booleano","cadena","caracter",
        "Entero","Decimal","Booleano","Cadena","Caracter"
    };
                return (t.Tipo == TokenType.PalabraReservada || t.Tipo == TokenType.TipoDato)
                       && tipos.Contains(t.Lexema);
            }


            private NodoSintactico ParseDeclaracionVariable()
            {
                var nodo = new NodoSintactico("Declaración Variable");

                var tipo = Advance();
                nodo.AgregarHijo(new NodoSintactico($"Tipo: {tipo.Lexema}"));

                if (!MatchTipo(TokenType.Identificador))
                    Error("Se esperaba nombre de variable");
                else
                    nodo.AgregarHijo(new NodoSintactico($"Identificador: {_tokens[_pos - 1].Lexema}"));

                if (MatchLexema("="))
                {
                    var exp = new NodoSintactico("Valor asignado");
                    nodo.AgregarHijo(exp);
                    ParseExpresion();
                }

                if (!MatchLexema(";"))
                    Error("Falta ';' en declaración de variable");

                return nodo;
            }


            private void ParseTipo() => Advance();

            // ------------- FUNCIONES -------------
            private NodoSintactico ParseDeclaracionFuncion()
            {
                var nodo = new NodoSintactico("Función");
                MatchLexema("funcion");

                if (!MatchTipo(TokenType.Identificador))
                    Error("Se esperaba nombre de función");
                else
                    nodo.AgregarHijo(new NodoSintactico($"Nombre: {_tokens[_pos - 1].Lexema}"));

                MatchLexema("(");
                nodo.AgregarHijo(new NodoSintactico("Parámetros"));
                if (!MatchLexema(")")) Error("Falta ')'");
                MatchLexema("{");
                ParseDeclaraciones(nodo);
                MatchLexema("}");
                return nodo;
            }

            // ------------- EXPRESIONES -------------
            private void ParseExpresion() => ParseExpresionLogica();
            private void ParseExpresionLogica()
            {
                ParseExpArit();
                while (MatchLexema("==", "!=", ">", "<", ">=", "<=", "y", "o", "Y", "O"))
                    ParseExpArit();
            }
            private void ParseExpArit()
            {
                ParseFactor();
                while (MatchLexema("+", "-", "*", "/", "%"))
                    ParseFactor();
            }
            private void ParseFactor()
            {
                var t = Peek();

                // Expresión entre paréntesis
                if (MatchLexema("("))
                {
                    ParseExpresion();
                    MatchLexema(")");
                    return;
                }

                // Llamada a función: identificador(...)
                if (t.Tipo == TokenType.Identificador && Peek(1).Lexema == "(")
                {
                    Advance(); // identificador
                    Advance(); // '('
                    ParseArgumentos();
                    MatchLexema(")");
                    return;
                }

                // Literales o identificadores simples
                if (t.Tipo is TokenType.Identificador or TokenType.Entero or TokenType.Decimal or
                    TokenType.Cadena or TokenType.Caracter or TokenType.Booleano)
                {
                    Advance();
                    return;
                }

                Error($"Expresión inválida");
                Advance();
            }


            private void ParseArgumentos()
            {
                if (Peek().Lexema == ")") return;
                ParseExpresion();
                while (MatchLexema(",")) ParseExpresion();
            }

            // ------------- CONTROL (if, while, etc) -------------
            private NodoSintactico ParseEstructuraControl()
            {
                var nodo = new NodoSintactico("Estructura de Control");
                var palabra = Advance(); // si, mientras, para, intentar

                if (palabra.Lexema == "si")
                {
                    if (!MatchLexema("(")) Error("Se esperaba '(' después de 'si'");
                    ParseExpresion();
                    if (!MatchLexema(")")) Error("Se esperaba ')' después de la condición");
                    if (!MatchLexema("{")) Error("Se esperaba '{' para abrir bloque");
                    ParseDeclaraciones(nodo);
                    if (!MatchLexema("}")) Error("Se esperaba '}' para cerrar bloque");

                    if (MatchLexema("sino"))
                    {
                        if (!MatchLexema("{")) Error("Se esperaba '{' después de 'sino'");
                        ParseDeclaraciones(nodo);
                        if (!MatchLexema("}")) Error("Se esperaba '}' al cerrar 'sino'");
                    }
                }
                else if (palabra.Lexema == "mientras")
                {
                    if (!MatchLexema("(")) Error("Se esperaba '(' después de 'mientras'");
                    ParseExpresion();
                    if (!MatchLexema(")")) Error("Se esperaba ')' después de la condición");
                    if (!MatchLexema("{")) Error("Se esperaba '{' para abrir bloque");
                    ParseDeclaraciones(nodo);
                    if (!MatchLexema("}")) Error("Se esperaba '}' para cerrar bloque");
                }
                else
                {
                }

                return nodo;
            }


            private NodoSintactico ParseSentenciaControl()
            {
                var nodo = new NodoSintactico("Sentencia Control");
                var palabra = Advance(); 

                switch (palabra.Lexema)
                {
                    case "imprimir":
                    case "entrada":
                        if (!MatchLexema("(")) Error($"Se esperaba '(' después de '{palabra.Lexema}'");
                        if (palabra.Lexema == "imprimir") 
                            ParseArgumentos();
                        if (!MatchLexema(")")) Error("Se esperaba ')' al final de la sentencia");
                        if (!MatchLexema(";")) Error("Falta ';' al final de la sentencia");
                        break;

                    case "devolver":
                        ParseExpresion();
                        if (!MatchLexema(";")) Error("Falta ';' después de 'devolver'");
                        break;

                    case "romper":
                    case "continuar":
                        if (!MatchLexema(";")) Error($"Falta ';' después de '{palabra.Lexema}'");
                        break;
                }

                return nodo;
            }


            // ------------- CLASES -------------
            private NodoSintactico ParseDeclaracionClase()
            {
                var nodo = new NodoSintactico("Clase");
                MatchLexema("clase");

                if (!MatchTipo(TokenType.Identificador))
                    Error("Se esperaba nombre de clase");
                else
                    nodo.AgregarHijo(new NodoSintactico($"Nombre: {_tokens[_pos - 1].Lexema}"));

                MatchLexema("{");
                ParseDeclaraciones(nodo);
                MatchLexema("}");
                return nodo;
            }
        }
    }
}

