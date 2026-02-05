package main

import (
	"bufio"
	"bytes"
	"crypto/aes"
	"crypto/cipher"
	"crypto/md5"
	"crypto/rand"
	"encoding/base64"
	"fmt"
	"io"
	"os"
	"strings"
)

const MasterKey = "sua_chave_mestra_privada_123"

func deriveKeyAndIV(password string, salt []byte) (key, iv []byte) {
	data := append([]byte(password), salt...)
	var d []byte
	var di []byte
	for len(d) < 48 {
		hash := md5.New()
		hash.Write(append(di, data...))
		di = hash.Sum(nil)
		d = append(d, di...)
	}
	return d[:32], d[32:48]
}

func encrypt(plaintext string, password string) (string, error) {
	salt := make([]byte, 8)
	if _, err := io.ReadFull(rand.Reader, salt); err != nil {
		return "", err
	}

	key, iv := deriveKeyAndIV(password, salt)
	block, _ := aes.NewCipher(key)

	padLen := aes.BlockSize - (len(plaintext) % aes.BlockSize)
	padded := append([]byte(plaintext), bytes.Repeat([]byte{byte(padLen)}, padLen)...)

	ciphertext := make([]byte, len(padded))
	mode := cipher.NewCBCEncrypter(block, iv)
	mode.CryptBlocks(ciphertext, padded)

	result := append([]byte("Salted__"), salt...)
	result = append(result, ciphertext...)

	return base64.StdEncoding.EncodeToString(result), nil
}

func decrypt(token string, password string) (string, error) {
	data, err := base64.StdEncoding.DecodeString(token)
	if err != nil || len(data) < 16 || string(data[:8]) != "Salted__" {
		return "", fmt.Errorf("ERRO: Token inválido.")
	}

	salt := data[8:16]
	ciphertext := data[16:]

	key, iv := deriveKeyAndIV(password, salt)
	block, _ := aes.NewCipher(key)

	if len(ciphertext)%aes.BlockSize != 0 {
		return "", fmt.Errorf("ERRO: Ciphertext corrompido.")
	}

	decrypted := make([]byte, len(ciphertext))
	mode := cipher.NewCBCDecrypter(block, iv)
	mode.CryptBlocks(decrypted, ciphertext)

	padLen := int(decrypted[len(decrypted)-1])
	if padLen > aes.BlockSize || padLen == 0 {
		return "", fmt.Errorf("ERRO: Falha ao descriptografar.")
	}

	return string(decrypted[:len(decrypted)-padLen]), nil
}

func main() {
	reader := bufio.NewReader(os.Stdin)
	fmt.Println("=== COFRE DIGITAL PRIVADO (GO) ===")
	fmt.Print("O que deseja fazer? (1) Encriptar ou (2) Descriptar: ")
	acao, _ := reader.ReadString('\n')
	acao = strings.TrimSpace(acao)

	if acao == "1" {
		fmt.Print("Digite o segredo/senha que deseja encriptar: ")
		texto, _ := reader.ReadString('\n')
		texto = strings.TrimSpace(texto)
		token, err := encrypt(texto, MasterKey)
		if err != nil {
			fmt.Println("Erro:", err)
			return
		}
		fmt.Printf("\n[!] Token Gerado: %s\n", token)
	} else if acao == "2" {
		fmt.Print("Digite o token encriptado: ")
		token, _ := reader.ReadString('\n')
		token = strings.TrimSpace(token)
		texto, err := decrypt(token, MasterKey)
		if err != nil {
			fmt.Println("\n[V] Segredo Original:", err)
			return
		}
		fmt.Printf("\n[V] Segredo Original: %s\n", texto)
	} else {
		fmt.Println("Opção inválida.")
	}
}
