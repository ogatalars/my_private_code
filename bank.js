const CryptoJs = require('crypto-js')
const readline = require("readline")

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
})

const perguntar = (pergunta) => {
    return new Promise((resolve) => rl.question(pergunta, resolve))
}

// Chave mestra interna do seu cofre
const MASTER_KEY = "sua_chave_mestra_privada_123";

function encriptar(valor) {
    return CryptoJs.AES.encrypt(valor, MASTER_KEY).toString();
}

function decriptar(token) {
    try {
        const bytes = CryptoJs.AES.decrypt(token, MASTER_KEY);
        const original = bytes.toString(CryptoJs.enc.Utf8);
        return original || "ERRO: Token inválido.";
    } catch (e) {
        return "ERRO: Falha ao descriptografar.";
    }
}

async function iniciar() {
    console.log("=== COFRE DIGITAL PRIVADO ===");
    
    const acao = await perguntar("O que deseja fazer? (1) Encriptar ou (2) Descriptar: ");

    if (acao === "1") {
        const texto = await perguntar("Digite o segredo/senha que deseja encriptar: ");
        const token = encriptar(texto);
        console.log("\n[!] Token Gerado (Guarde isso): ", token);
    } 
    else if (acao === "2") {
        const token = await perguntar("Digite o token encriptado: ");
        const texto = decriptar(token);
        console.log("\n[V] Segredo Original: ", texto);
    } 
    else {
        console.log("Opção inválida.");
    }

    rl.close(); 
}

iniciar();