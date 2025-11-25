# BlueSentinal — Documentação completa de Endpoints (implantada)

Status: Hospedado em http://bluesentinal.somee.com  
Base API: http://bluesentinal.somee.com  
Grupo Identity: http://bluesentinal.somee.com/Usuario

Este README mapeia todos os endpoints implementados, requisitos de header, parâmetros e exemplos de request/response em JSON. Use-o como referência para integrar clientes (Postman, Insomnia, SDKs) ou para testes diretos (curl).

Índice
- Regras gerais (headers / auth)
- Endpoints Identity (registrar e login) — exemplos de body e responses JSON
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
- CORS: servidor tem política "AllowAll" (mas recomenda-se HTTPS em produção).
- Para rotas que usam path params: substituir conforme indicado (ex.: {id}, {userName}).
- Para rotas que usam query params: passar na query string (ex.: ?mac=AA:BB:CC...).

--------------------------------------------------------------------------------
ENDPOINTS IDENTITY (grupo /Usuario)
Observação: o projeto usa AddIdentityApiEndpoints e MapIdentityApi; existe um endpoint customizado POST /Usuario/registrar mapeado manualmente no Program.cs. O login foi documentado para retornar explicitamente um token Bearer (sem uso de cookies).

1) POST /Usuario/registrar
- Descrição: registra novo usuário.
- Autenticação: aberto.
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
- Responses JSON:

Success (201/200):
```json
{
  "message": "Usuário criado com sucesso",
  "userId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77"
}
```

Validation error (400):
```json
{
  "errors": [
    "Email já existe",
    "Password deve ter ao menos 8 caracteres com letras maiúsculas, minúsculas e caracteres especiais"
  ]
}
```

--------------------------------------------------------------------------------
2) POST /Usuario/login
- Descrição: realiza login. Este endpoint NÃO usa cookies; retorna token JWT (Bearer) que o cliente deve usar nas chamadas subsequentes.
- Autenticação: aberto.
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
- Responses JSON:

Success (200):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9....",
  "tokenType": "Bearer",
  "expiresIn": 3600,
}
```

Invalid credentials (401):
```json
{
  "error": "Credenciais inválidas"
}
```

Uso pelo cliente:
- Depois de receber accessToken incluir header:
  Authorization: Bearer <accessToken>

--------------------------------------------------------------------------------
ENDPOINTS /Usuarios
Base: http://bluesentinal.somee.com/api/Usuarios

1) GET /Usuarios
- Descrição: retorna todos os usuários.
- Auth: (verificar ambiente; caso privado usar Authorization: Bearer <token>)
- Response (200):
```json
[
  {
    "id": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
    "userName": "usuario@example.com",
    "email": "usuario@example.com",
    "nome": "Nome Completo",
    "nascimento": "1990-01-01"
  },
  {
    "id": "a1b2c3d4-1111-2222-3333-444455556666",
    "userName": "admin@example.com",
    "email": "admin@example.com",
    "nome": "Administrador",
    "nascimento": null
  }
]
```

2) DELETE /Usuarios/{id}
- Descrição: exclui usuário por id.
- Auth: Authorization: Bearer <token> (role = Admin)
- Success (204 No Content): (sem corpo)
- Not found (404):
```json
{
  "error": "Usuário não encontrado"
}
```
- Unauthorized/Forbidden (401/403):
```json
{
  "error": "Sem permissão para executar esta ação"
}
```

3) DELETE /Usuarios/me
- Descrição: exclui a própria conta do usuário autenticado.
- Auth: Authorization: Bearer <token>
- Success (204): (sem corpo)
- Unauthorized (401):
```json
{
  "error": "Usuário não autenticado."
}
```
- Error ao excluir (400):
```json
{
  "error": "Erro ao excluir o usuário: [descrição dos erros]"
}
```

4) POST /Usuarios/adicionarRole
- Descrição: adiciona role a usuário; cria a role se não existir.
- Auth: Authorization: Bearer <token> (role = Admin)
- Exemplo via query:
  POST /Usuarios/adicionarRole?userId=<id>&role=Admin
- Ou body:
```json
{
  "userId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
  "role": "Admin"
}
```
- Success (200):
```json
{
  "message": "Role 'Admin' adicionada ao usuário 'usuario@example.com'."
}
```
- Errors (400/404):
```json
{
  "error": "Usuário não encontrado"
}
```
ou
```json
{
  "error": "Usuário já está na role 'Admin'."
}
```

--------------------------------------------------------------------------------
ENDPOINTS /DroneFabris
Base: http://bluesentinal.somee.com/DroneFabris  
Observação: classe decorada com [Authorize(Roles = "Admin")] — exige role Admin.

1) GET /DroneFabris
- Success (200):
```json
[
  {
    "droneFabriId": "11111111-2222-3333-4444-555566667777",
    "modelo": "X1",
    "mac": "AA:BB:CC:DD:EE:FF",
    "status": false
  },
  {
    "droneFabriId": "88888888-9999-0000-aaaa-bbbbccccdddd",
    "modelo": "Y2",
    "mac": "FF:EE:DD:CC:BB:AA",
    "status": true
  }
]
```

2) GET /DroneFabris/{id}
- Success (200):
```json
{
  "droneFabriId": "11111111-2222-3333-4444-555566667777",
  "modelo": "X1",
  "mac": "AA:BB:CC:DD:EE:FF",
  "status": false
}
```
- Not found (404):
```json
{
  "error": "DroneFabri não encontrado"
}
```

3) PUT /DroneFabris/{id}
- Body exemplo:
```json
{
  "droneFabriId": "11111111-2222-3333-4444-555566667777",
  "modelo": "X1-Atualizado",
  "mac": "AA:BB:CC:DD:EE:FF",
  "status": true
}
```
- Success (204): (sem corpo)
- Bad Request (400):
```json
{
  "error": "Payload inválido / id mismatch"
}
```

4) POST /api/DroneFabris
- Body exemplo:
```json
{
  "modelo": "Z3",
  "mac": "12:34:56:78:9A:BC",
  "status": false
}
```
- Success (201 Created):
```json
{
  "droneFabriId": "aaaaaaaa-bbbb-cccc-dddd-eeeeffff0000",
  "modelo": "Z3",
  "mac": "12:34:56:78:9A:BC",
  "status": false
}
```
- Duplicate MAC (400):
```json
{
  "error": "Este MAC já está vinculado a outro drone."
}
```

5) DELETE /api/DroneFabris/{id}
- Success (204): (sem corpo)
- Not found (404):
```json
{
  "error": "DroneFabri não encontrado"
}
```

--------------------------------------------------------------------------------
ENDPOINTS /Drones
Base: http://bluesentinal.somee.com/Drones  
A classe tem [Authorize] — todos os endpoints requerem autenticação; alguns exigem role Admin.

1) GET /Drones
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
    "droneFabri": {
      "droneFabriId": "11111111-2222-3333-4444-555566667777",
      "modelo": "X1",
      "mac": "AA:BB:CC:DD:EE:FF",
      "status": true
    },
    "usuario": {
      "id": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
      "email": "usuario@example.com",
      "nome": "Nome Completo"
    }
  }
]
```

2) GET /Drones/getDroneUser
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
    "droneFabri": { "droneFabriId": "11111111-2222-3333-4444-555566667777", "modelo": "X1", "mac": "AA:BB:CC:DD:EE:FF", "status": true }
  }
]
```

3) GET /Drones/getByUserName/{userName}
- Auth: Admin
- Success (200): mesma estrutura do GET acima, filtrada por username
- Bad request (400) / Not found (404):
```json
{ "error": "Nome do usuário não fornecido." }
```
ou
```json
{ "error": "Usuário não encontrado." }
```

4) POST /Drones/vincular?mac={mac}
- Auth: usuário autenticado
- Query param: mac (string)
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
  "usuarioId": "d9b1d7db-5c9a-4f1a-9f7d-3b1c2a5e6f77",
  "localizacao": "Gate A",
  "tempoEmMili": 0,
  "tempoEmHoras": 0.0,
  "status": false
}
```
- DroneFabri not found (404):
```json
{ "error": "DroneFabri não encontrado" }
```
- Already linked (400):
```json
{ "error": "Este drone já está vinculado ao usuário." }
```

5) PUT /Drones/tempo/{id}
- Auth: usuário autenticado
- Body: raw number (long) no corpo (ex.: 3600000)
- Success (200) — retorno do drone atualizado:
```json
{
  "droneId": "00000000-1111-2222-3333-444455556666",
  "tempoEmMili": 7200000
}
```
- Not found (404):
```json
{ "error": "Drone não encontrado." }
```

6) DELETE  /Drones/{id}
- Auth: usuário autenticado
- Success (204): (sem corpo)
- Not found (404):
```json
{ "error": "Drone não encontrado." }
```


--------------------------------------------------------------------------------
Observações finais e recomendações
- Para obter os schemas e respostas exatas (incluindo códigos/formatos retornados pelo MapIdentityApi em runtime), abra o Swagger no ambiente hospedado: http://bluesentinal.somee.com/swagger
- Esta documentação exige que o fluxo de autenticação do cliente use o token JWT retornado (accessToken/tokenType) e não cookies. Se o servidor ainda estiver configurado para emitir cookies, ajuste a configuração do Identity/MapIdentityApi para evitar emissão de cookies ou mantenha a opção de login com token que já existe.
- Padronize erros: alguns controllers retornam strings; recomendamos usar objeto { "error": { "code":..., "message": "...", "details":[...] } } para consistência.
- Segurança: habilitar HTTPS no ambiente de produção e revisar política CORS ("AllowAll" é útil para desenvolvimento, mas inseguro em produção).

Próximos passos que posso executar para você (se quiser):
- Gerar uma coleção Postman/Insomnia com todos os endpoints e placeholders de token.
- Gerar OpenAPI (YAML/JSON) a partir do Swagger do host.
- Normalizar exemplos de request/response com payloads reais retornados pelo Swagger em runtime.