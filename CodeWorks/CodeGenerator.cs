using System.Security.Cryptography;

namespace CodeWorks
{
    public class CodeGenerator
    {
        // izin verilen karakterler uzerinden secretkey ile sırası degistirilen karakterler
        private string Characters { get; set; }

        // default ise linear olarak 1,3,5 index numaralı karakterleri belirler
        private Func<int,int,int,int> FindNextLetterIndex { get; set; }

        // secure random numbers uretmek icin kullanilmistir
        private readonly RNGCryptoServiceProvider provider;

        public CodeGenerator()
        {
            provider = new RNGCryptoServiceProvider();
            FindNextLetterIndex = (a,x,b) => a * x + b;
            Randomize();
        }

        public CodeGenerator(string secretKey)
        {
            provider = new RNGCryptoServiceProvider();
            FindNextLetterIndex = (a, x, b) => a * x + b;
            Randomize(secretKey);
        }

        public CodeGenerator(string secretKey, Func<int, int, int, int> findNextLetterIndex)
        {
            provider = new RNGCryptoServiceProvider();
            FindNextLetterIndex = findNextLetterIndex;
            Randomize(secretKey);
        }


        /// <summary>
        /// Verilen secret-key ile izin verilen karakterleri tekrar sıralar
        /// </summary>
        /// <param name="secretKey"></param>
        private void Randomize(string secretKey = "MySecretKey")
        {
            // izin verilen karakterler
            var allowedCharacters = "ACDEFGHKLMNPRTXYZ234579";
            // izin verilen karakterler char list olarak atanır
            var characterLists = allowedCharacters.ToCharArray().ToList();
            // kullanıcı tarafından belirtilen secret-key karakterleri char list olarak atanır
            var secretKeyCharArray = secretKey.ToCharArray().ToList();
            // izin verilen karakter sayısınca donulur
            for (var i = 0; i < allowedCharacters.Length; i++)
            { 
                // secretkey mod i index'indeki char'in ascii kodu alınır
                var ascii = (int)secretKeyCharArray[i%secretKey.Length];
                // ascii modu üzerinden karakterin sonraki index'i belirlenir
                var nextIndex = ascii % allowedCharacters.Length;
                // mevcut karakter mevcut index'inden yeni index'ine tasinir
                characterLists.Remove(allowedCharacters[i]);
                characterLists.Insert(nextIndex, allowedCharacters[i]);
            }
            // yeni sıralama ile izin verilen karakterler dönülür
            Characters = string.Join("", characterLists);
        }

        /// <summary>
        /// algoritmaya göre token üretilir
        /// </summary>
        /// <returns></returns>
        public string GenerateToken()
        {
            // her token'ın son iki karakteri encoding ve decoding key'i olur
            // random belirlenir
            var key = GetKey();
            // key'in ilk karakterinin index'i bulunur
            var a = Characters.IndexOf(key[0]);
            // key'in ikinci karakterinin index'i bulunur
            var b = Characters.IndexOf(key[1]);
            var token = "";
            // key haricinde kalan ilk 6 karakter burada belirlenir
            // 0,2,4 index'inde bulunan karakterler random belirlenir
            // 1,3.5 index'i kendinden önceki index degerleri uzerinde linear (default ise) hesaplanır
            for (var i = 0; i < 3; i++)
            {
                // 0,2,4 index'inde bulunan karakterler icin random belirleme
                var rndLetter = GetRandomCharacter();
                // 1,3.5 index'inde bulunan karakterler icin linear (default ise) hesaplama
                var nextLetter = FindNextCharacter(a, Characters.IndexOf(rndLetter), b);
                // berlilenen basamaklar ikili olarak token degerine eklenir
                token += $"{rndLetter}{nextLetter}";
            }
            // token'a encoding ve decoding key'i eklenir
            token += $"{key[0]}{key[1]}";
            return token;
        }

        /// <summary>
        /// token'ın verify islemi gerceklestirilir
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool VerifyToken(string token)
        {
            // izin verilemeyen karakter ve length kontolu
            if (token.Length != 8 || token.Any(x => !Characters.Contains(x)))
            { 
                return false;
            }

            // her token'ın son iki karakteri encoding ve decoding key'i olur
            var a = Characters.IndexOf(token[6]);
            var b = Characters.IndexOf(token[7]);
            // 1,3,5 index'i key karakterleri a ve b aynı zamanda önceki index karakteri üzerinden kontrol edilir
            // [i+1] = a * [i] + b fonksitonu ile uyumlu olmalıdır
            for (var i = 0; i < 3; i = i + 2)
            {
                if (token[i + 1] != FindNextCharacter(a, Characters.IndexOf(token[i]), b))
                { 
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// her token'ın son iki karakteri encoding ve decoding key'i olur
        /// key karakterlerini random verir
        /// </summary>
        /// <returns></returns>
        private char[] GetKey()
        {
            return new char[] { GetRandomCharacter(), GetRandomCharacter() };
        }

        /// <summary>
        /// Chracters listesinden random character doner
        /// </summary>
        /// <returns></returns>
        private char GetRandomCharacter()
        {
            var byteArray = new byte[8];
            provider.GetBytes(byteArray);
            var n = BitConverter.ToUInt32(byteArray, 0);
            var randomInteger = (int)(n % Characters.Length);
            return Characters[randomInteger];
        }

        /// <summary>
        /// a ve b key karakterlerini ve onceki index karakterini kullanarak
        /// linear (default ise) olarak belirlenecek karakteri belirler
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private char FindNextCharacter(int a, int x, int b)
        {
            var calculatedNmb = FindNextLetterIndex(a,x,b);
            return Characters[(x + calculatedNmb) % Characters.Length];
        }
    }
}
