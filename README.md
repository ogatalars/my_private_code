# Cofre Digital Privado - Multi-Linguagem

Um script interativo para encriptar e descriptografar segredos usando o algoritmo AES-256-CBC, compatível entre Node.js, Python, Go e C#.

## 🚀 Como usar

### 1. JavaScript (Node.js)
```bash
cd js
npm install
node bank.js
```

### 2. Python
```bash
cd python
pip install pycryptodome
python bank.py
```

### 3. Go
```bash
cd go
go run bank.go
```

### 4. C#
```bash
cd csharp
dotnet run
```
*(Nota: Para C#, você pode precisar criar um projeto simples com `dotnet new console` e adicionar o arquivo code)*

## 💡 Funcionamento
Todas as implementações usam a mesma **Chave Mestra** interna, permitindo que você encripte um segredo em uma linguagem e descriptografe em qualquer outra.

---
**Nota**: O formato do Token é o padrão OpenSSL/Crypto-JS (`Salted__...`).
