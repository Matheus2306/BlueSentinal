# BlueSentinal — Documentação completa de Endpoints (implantada, v8)

Status: Hospedado em http://bluesentinal.somee.com  
Base API: http://bluesentinal.somee.com/api  
Grupo Identity: http://bluesentinal.somee.com/Usuario  
Swagger (interativo): http://bluesentinal.somee.com/swagger

Este README mapeia todos os endpoints implementados, requisitos de header, parâmetros e exemplos de request/response em JSON. Use-o como referência para integrar clientes (Postman, Insomnia, SDKs) ou para testes diretos (curl).

Índice
- Regras gerais (headers / auth)
- Endpoints Identity (registrar, login e demais endpoints padrão do Identity)
- Endpoints /Usuarios — responses JSON
- Endpoints /DroneFabris — responses JSON
- Endpoints /Drones — responses JSON
- Exemplos curl rápidos
- Observações e recomendações

--------------------------------------------------------------------------------
Regras gerais (headers / auth)
- Content-Type: application/json — para requests com body JSON.
- Accept: application/json — recomendado para todas as requisições.
- Authorization: Bearer <JWT_TOKEN> — usado por endpoints decorados com [Authorize].
- CORS: servidor tem política "AllowAll" (recomendado revisar em produção).
- Para rotas com path params, substituir conforme indicado (ex.: {id}, {userName}).
- Para rotas com query params, passar na query string (ex.: ?mac=AA:BB:CC...).

--------------------------------------------------------------------------------
ENDPOINTS IDENTITY (grupo /Usuario)
Observação: o projeto usa AddIdentityApiEndpoints e MapIdentityApi. Além dos endpoints custom (ex.: POST /Usuario/registrar) o MapIdentityApi normalmente expõe os endpoints padrão do Identity. Abaixo estão os endpoints Identity mais comuns que devem existir quando MapIdentityApi está habilitado — ver /Usuario/swagger para confirmar quais exatamente estão ativos no ambiente.

1) POST /Usuario/registrar
- Descrição: registra novo usuário (endpoint custom do projeto).
- Headers:
  - Content-Type: application/json
  - Accept: application/json
- Body exemplo:
```json
{
  "email": "usuario@example.com",
  "password": "Senha123!",
  "confirmPassword": "Senha123!",
  "nome": "Nome Completo",
  "nascimento": "1990-01-01"
}
```
- Success (201/200):
```json
{
  "message": "Usuário criado com sucesso",
  "userId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77"
}
```
- Validation error (400):
```json
{
  "errors": ["Email já existe", "Password inválida"]
}
```

2) POST /Usuario/login
- Descrição: realiza login. Documentado para NÃO usar cookies; retorna token JWT (Bearer).
- Headers:
  - Content-Type: application/json
  - Accept: application/json
- Body exemplo:
```json
{
  "email": "usuario@example.com",
  "password": "Senha123!"
}
```
- Success (200):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9....",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
    "email": "usuario@example.com",
    "nome": "Nome Completo"
  }
}
```
- Invalid credentials (401):
```json
{ "error": "Credenciais inválidas" }
```

3) POST /Usuario/logout
- Descrição: encerra a sessão; se houver refresh tokens armazenados, invalida-os.
- Auth: Authorization: Bearer <token> (dependendo da configuração)
- Headers: Authorization
- Success (204): sem corpo
- Error (401):
```json
{ "error": "Não autenticado" }
```

4) POST /Usuario/refresh-token (se presente)
- Descrição: renova o access token usando refresh token.
- Headers:
  - Content-Type: application/json
- Body exemplo:
```json
{
  "refreshToken": "<refresh_token>"
}
```
- Success (200):
```json
{
  "accessToken": "<new_jwt>",
  "tokenType": "Bearer",
  "expiresIn": 3600
}
```
- Invalid / expired (401):
```json
{ "error": "Refresh token inválido ou expirado" }
```

5) POST /Usuario/forgot-password
- Descrição: inicia fluxo de recuperação de senha; envia e-mail com token.
- Headers: Content-Type: application/json
- Body exemplo:
```json
{ "email": "usuario@example.com" }
```
- Success (200):
```json
{ "message": "Email de recuperação enviado (se o email existir)" }
```

6) POST /Usuario/reset-password
- Descrição: redefinir senha usando token recebido por e-mail.
- Headers: Content-Type: application/json
- Body exemplo:
```json
{
  "email": "usuario@example.com",
  "token": "<token_enviado_por_email>",
  "password": "NovaSenha123!"
}
```
- Success (200):
```json
{ "message": "Senha redefinida com sucesso" }
```
- Error (400):
```json
{ "errors": ["Token inválido", "Senha não atende requisitos"] }
```

7) POST /Usuario/confirm-email
- Descrição: confirma o email do usuário (token enviado por e-mail).
- Headers: Content-Type: application/json
- Body exemplo:
```json
{
  "userId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
  "code": "<codigo_de_confirmacao>"
}
```
- Success (200):
```json
{ "message": "Email confirmado com sucesso" }
```
- Error (400):
```json
{ "error": "Código de confirmação inválido" }
```

8) POST /Usuario/change-password
- Descrição: altera senha do usuário autenticado.
- Auth: Authorization: Bearer <token>
- Body exemplo:
```json
{
  "currentPassword": "SenhaAntiga123!",
  "newPassword": "SenhaNova123!"
}
```
- Success (200):
```json
{ "message": "Senha alterada com sucesso" }
```
- Error (400):
```json
{ "error": "Senha atual incorreta" }
```

9) GET /Usuario/me (se exposto)
- Descrição: retorna perfil do usuário autenticado.
- Auth: Authorization: Bearer <token>
- Success (200):
```json
{
  "id": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
  "email": "usuario@example.com",
  "nome": "Nome Completo",
  "nascimento": "1990-01-01"
}
```
- Unauthorized (401):
```json
{ "error": "Usuário não autenticado." }
```

10) Endpoints de login externo (se habilitado)
- /Usuario/external-login, /Usuario/external-callback — fluxos com provedores externos (Google, Facebook). A estrutura depende dos provedores configurados.

Observações:
- Nem todos os itens acima são criados automaticamente em todas as configurações; verifique o Swagger em /Usuario/swagger ou /swagger para confirmar quais endpoints do Identity estão expostos no ambiente.
- Todos os endpoints autenticados devem ser chamados com header Authorization: Bearer <accessToken> retornado no login.

--------------------------------------------------------------------------------
ENDPOINTS /Usuarios
Base: http://bluesentinal.somee.com/api/Usuarios

1) GET /api/Usuarios
- Descrição: retorna todos os usuários.
- Auth: ver código; se privado use Authorization: Bearer <token>
- Response (200):
```json
[
  {
    "id": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
    "userName": "usuario@example.com",
    "email": "usuario@example.com",
    "nome": "Nome Completo",
    "nascimento": "1990-01-01"
  }
]
```

2) DELETE /api/Usuarios/{id}
- Auth: Authorization: Bearer <token> (role = Admin)
- Success (204): sem corpo
- Not found (404):
```json
{ "error": "Usuário não encontrado" }
```

3) DELETE /api/Usuarios/me
- Auth: Authorization: Bearer <token>
- Success (204): sem corpo
- Unauthorized (401):
```json
{ "error": "Usuário não autenticado." }
```

4) POST /api/Usuarios/adicionarRole
- Auth: Authorization: Bearer <token> (role = Admin)
- Body / Query:
```json
{ "userId":"<id>", "role":"Admin" }
```
- Success (200):
```json
{ "message": "Role 'Admin' adicionada ao usuário 'usuario@example.com'." }
```

--------------------------------------------------------------------------------
ENDPOINTS /DroneFabris
Base: http://bluesentinal.somee.com/api/DroneFabris  
Classe decorada com [Authorize(Roles = "Admin")] — exige role Admin.

1) GET /api/DroneFabris
- Success (200):
```json
[
  {
    "droneFabriId": "11111111-2222-3333-4444-555566667777",
    "modelo": "X1",
    "mac": "AA:BB:CC:DD:EE:FF",
    "status": false
  }
]
```

2) GET /api/DroneFabris/{id}
- Success (200):
```json
{
  "droneFabriId": "11111111-2222-3333-4444-555566667777",
  "modelo": "X1",
  "mac": "AA:BB:CC:DD:EE:FF",
  "status": false
}
```

3) PUT /api/DroneFabris/{id}
- Body exemplo:
```json
{
  "droneFabriId": "11111111-2222-3333-4444-555566667777",
  "modelo": "X1-Atualizado",
  "mac": "AA:BB:CC:DD:EE:FF",
  "status": true
}
```
- Success (204): sem corpo

4) POST /api/DroneFabris
- Body exemplo:
```json
{
  "modelo": "Z3",
  "mac": "12:34:56:78:9A:BC",
  "status": false
}
```
- Success (201):
```json
{
  "droneFabriId": "aaaaaaaa-bbbb-cccc-dddd-eeeeffff0000",
  "modelo": "Z3",
  "mac": "12:34:56:78:9A:BC",
  "status": false
}
```

5) DELETE /api/DroneFabris/{id}
- Success (204) ou 404
```json
{ "error": "DroneFabri não encontrado" }
```

--------------------------------------------------------------------------------
ENDPOINTS /Drones
Base: http://bluesentinal.somee.com/api/Drones  
Classe tem [Authorize] — exige autenticação.

1) GET /api/Drones
- Auth: Admin
- Success (200):
```json
[
  {
    "droneId": "00000000-1111-2222-3333-444455556666",
    "droneFabriId": "11111111-2222-3333-4444-555566667777",
    "usuarioId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
    "localizacao": "Gate A",
    "tempoEmMili": 3600000,
    "tempoEmHoras": 1.0,
    "status": true,
    "droneFabri": { "droneFabriId":"...", "modelo":"X1","mac":"AA:BB:..." },
    "usuario": { "id":"...", "email":"..." }
  }
]
```

2) GET /api/Drones/getDroneUser
- Auth: usuário autenticado
- Success (200):
```json
[
  {
    "droneId": "00000000-1111-2222-3333-444455556666",
    "droneFabriId": "11111111-2222-3333-4444-555566667777",
    "localizacao": "Gate A",
    "tempoEmMili": 3600000,
    "tempoEmHoras": 1.0,
    "status": true,
    "droneFabri": { "droneFabriId":"...", "modelo":"X1","mac":"AA:BB:..." }
  }
]
```

3) GET /api/Drones/getByUserName/{userName}
- Auth: Admin
- Success (200) / 400 / 404
```json
{ "error": "Nome do usuário não fornecido." }
```

4) POST /api/Drones/vincular?mac={mac}
- Auth: usuário autenticado
- Query param: mac
- Body exemplo:
```json
{
  "localizacao": "Gate A",
  "tempoEmMili": 0,
  "status": false
}
```
- Success (200):
```json
{
  "droneId": "22222222-3333-4444-5555-666677778888",
  "droneFabriId": "11111111-2222-3333-4444-555566667777",
  "usuarioId": "d9b1d7db-...",
  "localizacao": "Gate A",
  "tempoEmMili": 0,
  "tempoEmHoras": 0.0,
  "status": false
}
```

5) PUT /api/Drones/tempo/{id}
- Auth: usuário autenticado
- Body: raw number (long) — ex.: 3600000
- Success (200):
```json
{
  "droneId": "00000000-1111-2222-3333-444455556666",
  "tempoEmMili": 7200000,
  "tempoEmHoras": 2.0
}
```

6) DELETE /api/Drones/{id}
- Auth: usuário autenticado
- Success (204) / 404:
```json
{ "error": "Drone não encontrado." }
```

--------------------------------------------------------------------------------
Exemplos curl (substitua placeholders)

Registrar:
```bash
curl -X POST "http://bluesentinal.somee.com/Usuario/registrar" \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario@example.com","password":"Senha123!","confirmPassword":"Senha123!","nome":"Joao Silva","nascimento":"1990-01-01"}'
```

Login (recebe accessToken/tokenType — sem cookies):
```bash
curl -X POST "http://bluesentinal.somee.com/Usuario/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario@example.com","password":"Senha123!"}'
```

Usando token:
```bash
curl -X GET "http://bluesentinal.somee.com/api/Drones/getDroneUser" \
  -H "Authorization: Bearer <jwt_token_here>" \
  -H "Accept: application/json"
```

Vincular drone por MAC:
```bash
curl -X POST "http://bluesentinal.somee.com/api/Drones/vincular?mac=AA:BB:CC:DD:EE:FF" \
  -H "Authorization: Bearer <jwt_token_here>" \
  -H "Content-Type: application/json" \
  -d '{"localizacao":"Gate A","tempoEmMili":0,"status":false}'
```

--------------------------------------------------------------------------------
Observações finais e recomendações
- Para confirmar exatamente quais endpoints do Identity estão expostos no seu build, abra: http://bluesentinal.somee.com/Usuario/swagger ou http://bluesentinal.somee.com/swagger.
- A documentação acima inclui os endpoints padrões do Identity mais comuns; se algum não existir no seu deploy, remova-o/ignore-o e consulte o Swagger para o comportamento real.
- Recomendações:
  - Padronizar respostas de erro em formato JSON consistente.
  - Forçar HTTPS em produção.
  - Revisar CORS (AllowAll inseguro em produção).
  - Adotar expiração curta para accessToken e refresh tokens seguros.
- Posso:
  - Gerar uma coleção Postman/Insomnia com todos os endpoints mapeados.
  - Gerar OpenAPI (YAML/JSON) com base no Swagger do host.
  - Atualizar a documentação com exemplos reais extraídos do Swagger ao vivo.
