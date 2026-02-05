import sys
from Crypto.Cipher import AES
from Crypto.Util import Padding
import base64
import hashlib
from Crypto import Random

# Chave mestra interna (deve ser a mesma do JS)
MASTER_KEY = "sua_chave_mestra_privada_123"

def derive_key_and_iv(password, salt, key_len, iv_len):
    d = d_i = b""
    while len(d) < key_len + iv_len:
        d_i = hashlib.md5(d_i + password.encode('utf-8') + salt).digest()
        d += d_i
    return d[:key_len], d[key_len:key_len + iv_len]

def encrypt(plaintext, password):
    salt = Random.new().read(8)
    key, iv = derive_key_and_iv(password, salt, 32, 16)
    cipher = AES.new(key, AES.MODE_CBC, iv)
    padded_data = Padding.pad(plaintext.encode('utf-8'), AES.block_size)
    encrypted_data = cipher.encrypt(padded_data)
    result = b"Salted__" + salt + encrypted_data
    return base64.b64encode(result).decode('utf-8')

def decrypt(ciphertext, password):
    try:
        data = base64.b64decode(ciphertext)
        if data[:8] != b"Salted__":
            return "ERRO: Token inválido."
        salt = data[8:16]
        encrypted_data = data[16:]
        key, iv = derive_key_and_iv(password, salt, 32, 16)
        cipher = AES.new(key, AES.MODE_CBC, iv)
        padded_data = cipher.decrypt(encrypted_data)
        plaintext = Padding.unpad(padded_data, AES.block_size)
        return plaintext.decode('utf-8')
    except Exception:
        return "ERRO: Falha ao descriptografar."

def main():
    print("=== COFRE DIGITAL PRIVADO (PYTHON) ===")
    acao = input("O que deseja fazer? (1) Encriptar ou (2) Descriptar: ")

    if acao == "1":
        texto = input("Digite o segredo/senha que deseja encriptar: ")
        token = encrypt(texto, MASTER_KEY)
        print(f"\n[!] Token Gerado: {token}")
    elif acao == "2":
        token = input("Digite o token encriptado: ")
        texto = decrypt(token, MASTER_KEY)
        print(f"\n[V] Segredo Original: {texto}")
    else:
        print("Opção inválida.")

if __name__ == "__main__":
    main()
