namespace AnalizadorLexico.Lexico
{
    public class Token
    {
        public TokenType Tipo { get; }
        public string Lexema { get; }
        public int Linea { get; }
        public int Columna { get; }

        public Token(TokenType tipo, string lexema, int linea, int columna)
        {
            Tipo = tipo;
            Lexema = lexema;
            Linea = linea;
            Columna = columna;
        }

        public override string ToString()
        {
            return $"{Tipo}: '{Lexema}' (Línea {Linea}, Columna {Columna})";
        }
    }

}