# BlueSentinal API

API REST para gerenciamento de drones, fabricantes e usuários, com autenticação baseada em roles (`Admin` e `User`).  
Desenvolvida em .NET 8, C# 12, com Entity Framework Core, Identity e Blazor.

---

## Sumário

- [Autenticação](#autenticação)
- [Endpoints Principais](#endpoints-principais)
  - [Usuários](#usuários)
  - [Fabricantes de Drones (DroneFabris)](#fabricantes-de-drones-dronefabris)
  - [Drones](#drones)
- [Exemplo de Uso](#exemplo-de-uso)
- [Swagger](#swagger)
- [Observações](#observações)
- [Configurações do Projeto](#configurações-do-projeto)

---

## Autenticação

A API utiliza autenticação JWT.  
Para acessar endpoints protegidos, inclua o token JWT no header:
O cadastro e login de usuários é feito via endpoints do Identity API, mapeados em `/Usuario`.

Authorization: Bearer {seu_token}

> Necessário estar autenticado.

- **DELETE /api/Usuarios/{id}**  
Remove um usuário pelo ID.

- **POST /api/Usuarios/adicionarRole**  
Adiciona uma role ao usuário.  
**Body (form-data ou JSON):**
---

## Endpoints Principais

### Usuários

- **GET /api/Usuarios**  
  Lista todos os usuários cadastrados.

- **POST /api/Usuarios/registrar**  
  Cria um novo usuário.  
  **Body:**
- { "email": "usuario@email.com", "nome": "Nome do Usuário", "nascimento": "2000-01-01" }


---

### Fabricantes de Drones (DroneFabris)

> **Acesso restrito ao role `Admin`**

- **GET /api/DroneFabris**  
  Lista todos os fabricantes de drones.

- **GET /api/DroneFabris/{id}**  
  Retorna os dados de um fabricante específico.

- **POST /api/DroneFabris**  
  Cria um novo fabricante.  
  **Body:**

> O campo `mac` deve ser único.

- **PUT /api/DroneFabris/{id}**  
Atualiza os dados de um fabricante.

- **DELETE /api/DroneFabris/{id}**  
Remove um fabricante.

---

### Drones

- **GET /api/Drones**  
(Apenas `Admin`)  
Lista todos os drones cadastrados.

- **GET /api/Drones/getDroneUser**  
(Usuário autenticado)  
Lista todos os drones vinculados ao usuário logado.

- **POST /api/Drones/vincular?mac={mac}**  
Vincula um drone ao usuário logado, usando o MAC do fabricante.  
**Body:**
> O drone será vinculado ao usuário autenticado e ao fabricante correspondente ao MAC.

- **PUT /api/Drones/tempo/{id}**  
Atualiza o tempo de uso do drone.  
**Body:**
(Tempo em milissegundos)

- **DELETE /api/Drones/{id}**  
Remove o drone e atualiza o status do fabricante para `false`.

- **GET /api/Drones/getByUserName/{userName}**  
(Apenas `Admin`)  
Lista todos os drones vinculados ao usuário informado.

---

## Exemplo de Uso

1. **Autentique-se** usando os endpoints de `/Usuario` para obter o token JWT.
2. **Inclua o token** no header `Authorization` em todas as requisições protegidas.
3. **Utilize os endpoints** conforme sua role (`Admin` ou `User`).

---

## Swagger

A interface Swagger está disponível em `/swagger` para testes e documentação interativa.

---

## Observações

- O campo `mac` do fabricante deve ser único.
- Apenas usuários com role `Admin` podem gerenciar fabricantes e visualizar todos os drones.
- Usuários comuns podem vincular drones a si e consultar seus próprios drones.
- Roles são criadas automaticamente na inicialização do sistema (`Admin`, `User`).
- O projeto utiliza validação de dados via DataAnnotations e jQuery Validation.

---

## Configurações do Projeto

- **.NET 8 / C# 12**
- **Blazor** como framework principal.
- **Swagger** configurado para autenticação JWT.
- **CORS** liberado para qualquer origem, método e header.
- **Identity** configurado para roles e autenticação via endpoints REST.
- **Criação automática das roles** `Admin` e `User` ao iniciar o sistema.
- **Banco de dados** configurado via Entity Framework Core (`APIContext`).

---

**Dúvidas ou problemas?**  
Consulte a interface Swagger ou entre em contato com o responsável pelo projeto.

---

_GitHub Copilot_
