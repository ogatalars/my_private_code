using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

class Program {
    const string MASTER_KEY = "sua_chave_mestra_privada_123";

    static void Main() {
        Console.WriteLine("=== COFRE DIGITAL PRIVADO (C#) ===");
        Console.Write("O que deseja fazer? (1) Encriptar ou (2) Descriptar: ");
        string acao = Console.ReadLine();

        if (acao == "1") {
            Console.Write("Digite o segredo/senha que deseja encriptar: ");
            string texto = Console.ReadLine();
            string token = Encrypt(texto, MASTER_KEY);
            Console.WriteLine($"\n[!] Token Gerado: {token}");
        } else if (acao == "2") {
            Console.Write("Digite o token encriptado: ");
            string token = Console.ReadLine();
            string texto = Decrypt(token, MASTER_KEY);
            Console.WriteLine($"\n[V] Segredo Original: {texto}");
        } else {
            Console.WriteLine("Opção inválida.");
        }
    }

    static void DeriveKeyAndIV(string password, byte[] salt, out byte[] key, out byte[] iv) {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var combined = new byte[passwordBytes.Length + salt.Length];
        Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);

        byte[] d = Array.Empty<byte>();
        byte[] di = Array.Empty<byte>();
        using (var md5 = MD5.Create()) {
            while (d.Length < 48) {
                var input = new byte[di.Length + combined.Length];
                Buffer.BlockCopy(di, 0, input, 0, di.Length);
                Buffer.BlockCopy(combined, 0, input, di.Length, combined.Length);
                di = md5.ComputeHash(input);
                var newD = new byte[d.Length + di.Length];
                Buffer.BlockCopy(d, 0, newD, 0, d.Length);
                Buffer.BlockCopy(di, 0, newD, d.Length, di.Length);
                d = newD;
            }
        }

        key = new byte[32];
        iv = new byte[16];
        Buffer.BlockCopy(d, 0, key, 0, 32);
        Buffer.BlockCopy(d, 32, iv, 0, 16);
    }

    static string Encrypt(string plainText, string password) {
        byte[] salt = new byte[8];
        RandomNumberGenerator.Fill(salt);

        DeriveKeyAndIV(password, salt, out byte[] key, out byte[] iv);

        using (var aes = Aes.Create()) {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream()) {
                ms.Write(Encoding.UTF8.GetBytes("Salted__"), 0, 8);
                ms.Write(salt, 0, 8);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    static string Decrypt(string token, string password) {
        try {
            byte[] data = Convert.FromBase64String(token);
            if (data.Length < 16 || Encoding.UTF8.GetString(data, 0, 8) != "Salted__")
                return "ERRO: Token inválido.";

            byte[] salt = new byte[8];
            Buffer.BlockCopy(data, 8, salt, 0, 8);
            byte[] cipherText = new byte[data.Length - 16];
            Buffer.BlockCopy(data, 16, cipherText, 0, cipherText.Length);

            DeriveKeyAndIV(password, salt, out byte[] key, out byte[] iv);

            using (var aes = Aes.Create()) {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(cipherText))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs)) {
                    return reader.ReadToEnd();
                }
            }
        } catch {
            return "ERRO: Falha ao descriptografar.";
        }
    }
}
