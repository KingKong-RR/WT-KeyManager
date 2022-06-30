using System;
using System.Security.Cryptography;

namespace KeyManager.Utilities
{
    public class CharGenerator
    {
        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        // Methode um die Randomchars zu generieren
        public static string GetRandomChars(int length)
        {
            // Die Variable returnValue mit einem Leerzeichen füllen da sie sonst den Wert "NULL" hätte und man somit nichts einfügen könnte
            string returnValue = "";
            // Alle Zeichen die erlaubt sind in ein Array füllen und somit einen Zeichen-Range setzen
            char[] range = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            // Puffer Speicher
            byte[] data = new byte[8];

            // Durchläufe wie viele Random Chars generiert werden sollen
            for (int i = 0; i < length; i++)
            {
                ulong codeByte;
                do
                {
                    // Randomchars (1 Byte = 1 Char) in den Pufferspeicher schreiben
                    RngCsp.GetBytes(data);
                    // Bit in Byte konvertieren
                    codeByte = BitConverter.ToUInt64(data, 0);
                    // uCodeByte angeben das er nur die Zahlen 1-13 nehmen muss
                    codeByte = (ulong)(codeByte % 12 + 1);
                    //es wird solange gewartet bis der unsignedLong (ulong) uCodeByte nicht mehr 0 ist
                } while (codeByte == 0);

                // returnValue mit den Zeichen befüllen
                returnValue += range[codeByte];
            }
            // returnValue zurück geben
            return returnValue;
        }
    }
}