# BlueSentinal — Documentação de Endpoints

Status: Hospedado em http://bluesentinal.somee.com  
Base API: http://bluesentinal.somee.com/api  
Grupo Identity: http://bluesentinal.somee.com/Usuario  
Swagger (interativo): http://bluesentinal.somee.com/swagger

Este README documenta os endpoints expostos pela API do BlueSentinal com foco em: autenticação/usuários (Identity), gerenciamento de drones e registros de fábrica (DroneFabris). A documentação foi produzida a partir da leitura dos controllers no último commit — use o Swagger no deploy para confirmar schemas em runtime.

Sumário
- Visão geral
- Base URL e headers
- Sessão: Usuário & Identity (todos os endpoints relacionados)
- Sessão: DroneFabris (fábrica / hardware)
- Sessão: Drones (usuário / vinculação)
- Códigos de resposta e formato de erro
- Exemplos curl
- Boas práticas / Observações

1) Visão geral
BlueSentinal é uma API ASP.NET Core que combina endpoints do Identity (MapIdentityApi) e controllers REST para gerenciar usuários, drones e registros de hardware. Autenticação baseada em JWT (Bearer) é usada na documentação — o login foi documentado para retornar accessToken/tokenType.

2) Base URL e headers
- Host (produção): http://bluesentinal.somee.com
- API prefix (controllers): http://bluesentinal.somee.com/api
- Grupo Identity: http://bluesentinal.somee.com/Usuario
Headers recomendados:
- Content-Type: application/json (para requests com body)
- Accept: application/json
- Authorization: Bearer <JWT_TOKEN> (para endpoints protegidos)

3) Sessão: Usuário & Identity
Nesta seção estão todos os endpoints relacionados a usuários, autenticação e fluxos do Identity (registro, login, recuperação, perfil, roles).

3.1 POST /Usuario/registrar
- Descrição: registra um novo usuário (endpoint custom no Program.cs).
- Auth: aberto.
- Headers: Content-Type: application/json
- Body (exemplo):
```json
{
  "email": "usuario@example.com",
  "password": "Senha123!",
  "confirmPassword": "Senha123!",
  "nome": "Nome Completo",
  "nascimento": "1990-01-01"
}
```
- Success (200/201):
```json
{
  "message": "Usuário criado com sucesso",
  "userId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77"
}
```
- Validation (400):
```json
{
  "errors": ["Email já existe", "Senha inválida"]
}
```

3.2 POST /Usuario/login
- Descrição: realiza login — documentado para retornar token JWT (Bearer) e não usar cookies.
- Auth: aberto.
- Body (exemplo):
```json
{
  "email": "usuario@example.com",
  "password": "Senha123!"
}
```
- Success (200):
```json
{
  "accessToken": "<jwt_token_here>",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "<user-id>",
    "email": "usuario@example.com",
    "nome": "Nome Completo"
  }
}
```
- Invalid (401):
```json
{ "error": "Credenciais inválidas" }
```
Uso posterior: incluir no header Authorization: Bearer <accessToken>.

3.3 Outros endpoints padrões do Identity (quando expostos pelo MapIdentityApi)
OBS: a lista abaixo corresponde aos endpoints que MapIdentityApi normalmente expõe. Confirme via Swagger em runtime.
- POST /Usuario/logout
  - Auth: Authorization: Bearer <token> (quando aplicável)
  - Success: 204 No Content
- POST /Usuario/forgot-password
  - Body: { "email": "usuario@example.com" }
  - Success: { "message": "Email de recuperação enviado (se o email existir)" }
- POST /Usuario/reset-password
  - Body: { "email":"usuario@example.com", "token":"<token>", "password":"NovaSenha123!" }
  - Success: { "message": "Senha redefinida com sucesso" }
- POST /Usuario/confirm-email
  - Body: { "userId":"<id>", "code":"<codigo>" }
  - Success: { "message": "Email confirmado com sucesso" }
- POST /Usuario/change-password
  - Auth: Authorization: Bearer <token>
  - Body: { "currentPassword":"SenhaAntiga", "newPassword":"SenhaNova" }
  - Success: { "message": "Senha alterada com sucesso" }
- GET /Usuario/me (se exposto)
  - Auth: Authorization: Bearer <token>
  - Success:
```json
{
  "id": "<user-id>",
  "email": "usuario@example.com",
  "nome": "Nome Completo",
  "nascimento": "1990-01-01"
}
```

3.4 Endpoints de gerenciamento de usuários (controller /api/Usuarios)
- GET /api/Usuarios
  - Descrição: lista todos os usuários.
  - Auth: nenhum atributo específico no código; pode requerer autorização em produção.
  - Success (200): array de usuários (exemplo):
```json
[
  { "id":"<id>", "userName":"usuario@example.com", "email":"usuario@example.com", "nome":"Nome" }
]
```
- DELETE /api/Usuarios/{id}
  - Auth: Admin (Authorization: Bearer <token> com role = Admin)
  - Success: 204 No Content
  - Not found (404):
```json
{ "error": "Usuário não encontrado" }
```
- DELETE /api/Usuarios/me
  - Auth: qualquer usuário autenticado
  - Success: 204 No Content
  - Unauthorized (401):
```json
{ "error": "Usuário não autenticado." }
```
- POST /api/Usuarios/adicionarRole
  - Auth: Admin
  - Params: query (userId & role) ou body { "userId":"...", "role":"Admin" }
  - Success:
```json
{ "message": "Role 'Admin' adicionada ao usuário 'usuario@example.com'." }
```
  - Errors:
```json
{ "error": "Usuário não encontrado" }
```
ou
```json
{ "error": "Usuário já está na role 'Admin'." }
```

4) Sessão: DroneFabris (fábrica / hardware)
Classe decorada com [Authorize(Roles = "Admin")] — exige role Admin.

- GET /api/DroneFabris
  - Success (200):
```json
[
  { "droneFabriId":"<guid>", "modelo":"X1", "mac":"AA:BB:CC:DD:EE:FF", "status": false }
]
```
- GET /api/DroneFabris/{id}
  - Success (200):
```json
{ "droneFabriId":"<guid>", "modelo":"X1", "mac":"AA:BB:CC:DD:EE:FF", "status": false }
```
  - Not found (404):
```json
{ "error": "DroneFabri não encontrado" }
```
- PUT /api/DroneFabris/{id}
  - Body exemplo:
```json
{ "droneFabriId":"<guid>", "modelo":"X1-Atualizado", "mac":"AA:BB:...", "status": true }
```
  - Success: 204 No Content
  - Bad Request (400): id mismatch
```json
{ "error": "Payload inválido / id mismatch" }
```
- POST /api/DroneFabris
  - Body exemplo:
```json
{ "modelo":"Z3", "mac":"12:34:56:78:9A:BC", "status": false }
```
  - Duplicate MAC (400):
```json
{ "error": "Este MAC já está vinculado a outro drone." }
```
  - Success (201):
```json
{ "droneFabriId":"<guid>", "modelo":"Z3", "mac":"12:34:56:78:9A:BC", "status": false }
```
- DELETE /api/DroneFabris/{id}
  - Success: 204 No Content
  - Not found:
```json
{ "error": "DroneFabri não encontrado" }
```

5) Sessão: Drones (usuário / vinculação)
Classe tem [Authorize] — exige autenticação; alguns endpoints exigem Admin.

- GET /api/Drones
  - Auth: Admin
  - Success (200): lista completa (includes DroneFabri e Usuario)
```json
[
  {
    "droneId":"<guid>",
    "droneFabriId":"<guid>",
    "usuarioId":"<guid>",
    "localizacao":"Gate A",
    "tempoEmMili":3600000,
    "tempoEmHoras":1.0,
    "status": true,
    "droneFabri": { "droneFabriId":"...", "modelo":"X1", "mac":"AA:BB:...", "status": true },
    "usuario": { "id":"...", "email":"..." }
  }
]
```
- GET /api/Drones/getDroneUser
  - Auth: qualquer usuário autenticado
  - Retorna drones vinculados ao usuário do token (Claim NameIdentifier)
  - Success (200):
```json
[
  {
    "droneId":"<guid>",
    "droneFabriId":"<guid>",
    "localizacao":"Gate A",
    "tempoEmMili":3600000,
    "tempoEmHoras":1.0,
    "status": true,
    "droneFabri": { "droneFabriId":"...", "modelo":"X1", "mac":"AA:BB:..." }
  }
]
```
- GET /api/Drones/getByUserName/{userName}
  - Auth: Admin
  - Path param: userName
  - Errors:
```json
{ "error": "Nome do usuário não fornecido." }
```
ou
```json
{ "error": "Usuário não encontrado." }
```
- POST /api/Drones/vincular?mac={mac}
  - Auth: usuário autenticado
  - Query param: mac (MAC do DroneFabri)
  - Body exemplo:
```json
{ "localizacao":"Gate A", "tempoEmMili":0, "status": false }
```
  - Comportamento:
    - Busca DroneFabri por MAC; se não encontrado -> 404
    - Verifica se já existe vínculo com esse DroneFabriId e UsuarioId -> 400
    - Atribui UsuarioId = id do claim NameIdentifier e vincula DroneFabri; seta DroneFabri.Status = true; persiste.
  - Success (200):
```json
{
  "droneId":"<guid>",
  "droneFabriId":"<guid>",
  "usuarioId":"<guid>",
  "localizacao":"Gate A",
  "tempoEmMili":0,
  "tempoEmHoras":0.0,
  "status": false
}
```
  - Errors:
```json
{ "error": "DroneFabri não encontrado" }
```
ou
```json
{ "error": "Este drone já está vinculado ao usuário." }
```
- PUT /api/Drones/tempo/{id}
  - Auth: usuário autenticado
  - Body: raw number (long) — tempo em milissegundos (ex.: 3600000)
  - Success (200):
```json
{ "droneId":"<guid>", "tempoEmMili":7200000, "tempoEmHoras":2.0 }
```
  - Not found (404):
```json
{ "error": "Drone não encontrado." }
```
- DELETE /api/Drones/{id}
  - Auth: usuário autenticado
  - Comportamento: marca DroneFabri.Status = false, remove drone
  - Success: 204 No Content
  - Not found:
```json
{ "error": "Drone não encontrado." }
```

6) Códigos de resposta e formato de erro
- 200 OK — sucesso com corpo
- 201 Created — recurso criado
- 204 No Content — sucesso sem corpo (ex.: DELETE)
- 400 Bad Request — validação / parâmetros inválidos
- 401 Unauthorized — token ausente ou inválido
- 403 Forbidden — sem permissão / role
- 404 Not Found — recurso não encontrado
- 422 Unprocessable Entity — falha de validação detalhada
- 500 Internal Server Error — erro no servidor

Formato de erro sugerido (padronizar):
```json
{
  "error": {
    "code": 400,
    "message": "Validation failed",
    "details": [
      { "field": "email", "message": "Email inválido" }
    ]
  }
}
```

7) Exemplos curl
- Registrar (Identity custom)
```bash
curl -X POST "http://bluesentinal.somee.com/Usuario/registrar" \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario@example.com","password":"Senha123!","confirmPassword":"Senha123!","nome":"Joao Silva","nascimento":"1990-01-01"}'
```
- Login (recebe accessToken/tokenType — sem cookies)
```bash
curl -X POST "http://bluesentinal.somee.com/Usuario/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario@example.com","password":"Senha123!"}'
```
- Listar drones do usuário autenticado
```bash
curl -X GET "http://bluesentinal.somee.com/api/Drones/getDroneUser" \
  -H "Authorization: Bearer <TOKEN>" -H "Accept: application/json"
```
- Vincular drone por MAC
```bash
curl -X POST "http://bluesentinal.somee.com/api/Drones/vincular?mac=AA:BB:CC:DD:EE:FF" \
  -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" \
  -d '{"localizacao":"Gate A","tempoEmMili":0,"status":false}'
```

8) Boas práticas e observações
- Use HTTPS em produção (o host atual está em HTTP público; configure TLS).
- Não exponha tokens em repositórios públicos.
- Use paginação para listagens grandes.
- Padronize erros e mensagens para facilitar tratamento no cliente.
- Verifique o Swagger do deploy para obter schemas / exemplos reais:
  - http://bluesentinal.somee.com/swagger
  - http://bluesentinal.somee.com/Usuario/swagger

Se desejar eu:
- Faço o push desta versão para uma branch (ex.: v8) e abro PR no repositório — autorize se quiser que eu tente criar o PR.
- Gero coleção Postman/Insomnia com todos os endpoints.
- Gero um OpenAPI (YAML/JSON) a partir do Swagger do host (se acessível).
