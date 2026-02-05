# Cofre Digital Privado

Um script simples em Node.js para encriptar e descriptografar segredos (senhas, chaves, etc) usando o algoritmo AES.

## 🚀 Como usar

### 1. Instalação
Certifique-se de ter o Node.js instalado e execute:
```bash
npm install
```

### 2. Execução
O script funciona de forma interativa. Basta rodar:
```bash
node bank.js
```

### 3. Fluxo de uso
- **Encriptar**: Escolha a opção `1`, digite o texto que deseja proteger e o script gerará um **Token**.
- **Descriptar**: Escolha a opção `2`, cole o **Token** gerado anteriormente e o script revelará o segredo original.

---
**Nota**: O script utiliza uma chave mestra interna para realizar a criptografia.
