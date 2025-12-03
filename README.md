# BlueSentinal — Documentação completa de Endpoints (implantada)

Status: Hospedado em http://bluesentinal.somee.com  
Base API: http://bluesentinal.somee.com/api  
Grupo Identity: http://bluesentinal.somee.com/Usuario  
Swagger (interativo): http://bluesentinal.somee.com/swagger

Este README mapeia todos os endpoints implementados, requisitos de header, parâmetros e exemplos de request/response em JSON. Use-o como referência para integrar clientes (Postman, Insomnia, SDKs...). A documentação foi gerada a partir da leitura do código-fonte do repositório.

Índice
- Regras gerais (headers / auth)
- Endpoints Identity (grupo /Usuario) — endpoints gerados + custom
- Endpoints /Usuarios (controller api)
- Endpoints /DroneFabris (controller api)
- Endpoints /Drones (controller api)
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
Observação: o projeto registra endpoints do Identity via builder.Services.AddIdentityApiEndpoints<Usuario>(...) e, em Program.cs, cria um group com app.MapGroup("/Usuario"); dentro dele chama usuarioGroup.MapIdentityApi<Usuario>() — isso adiciona automaticamente um conjunto de endpoints relacionados ao fluxo de autenticação/conta. Além disso, existe um endpoint custom mapeado manualmente: POST /Usuario/registrar.

Importante: os endpoints gerados pelo MapIdentityApi são adicionados em runtime; nomes exatos, verbos e payloads podem variar conforme a versão do pacote. Para obter a lista e especificações exatas em execução, abra o Swagger UI (http://<host>/swagger).

1) POST /Usuario/registrar
- Onde: Program.cs (mapeado manualmente em usuarioGroup.MapPost)
- Método: POST
- Rota: /Usuario/registrar
- Autenticação: aberto
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
- Responses (exemplo):
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
  "errors": ["Email já existe", "Password inválida"]
}
```

--------------------------------------------------------------------------------
2) Endpoints gerados automaticamente pelo MapIdentityApi (grupo /Usuario)
- Observação: MapIdentityApi normalmente expõe endpoints padrão de Identity. Abaixo estão os endpoints típicos que costumam existir; confirme via Swagger para a especificação exata.

Exemplos típicos (nome e payload podem variar):
- POST /Usuario/login
  - Descrição: autentica usuário e retorna token JWT (accessToken, tokenType, expiresIn)
  - Auth: aberto
  - Body exemplo: { "email": "user@example.com", "password": "Senha123!" }
  - Response (200): { "accessToken": "<jwt>", "tokenType": "Bearer", "expiresIn": 3600 }

- POST /Usuario/logout
  - Descrição: encerra sessão (quando aplicável)
  - Auth: pode exigir token
  - Response: 204 No Content

- POST /Usuario/forgot-password
  - Descrição: inicia fluxo de recuperação (envia token por email)
  - Auth: aberto
  - Body: { "email": "user@example.com" }

- POST /Usuario/reset-password
  - Descrição: redefine senha usando token
  - Auth: aberto
  - Body: { "email": "user@example.com", "token": "<token>", "password": "NovaSenha123!" }

- POST /Usuario/change-password
  - Descrição: altera senha do usuário autenticado
  - Auth: Authorization: Bearer <token>
  - Body: { "oldPassword": "...", "newPassword": "..." }

- GET /Usuario/me  (ou GET /Usuario)
  - Descrição: retorna informações do usuário autenticado
  - Auth: Authorization: Bearer <token>

- POST /Usuario/confirm-email
  - Descrição: confirma email usando token (usado em rotas geradas)
  - Body: { "userId": "...", "token": "..." }

- Endpoints adicionais possíveis: external login callbacks, set-password, enable/disable 2FA, manage external logins.

Importante:
- Como esses endpoints são gerados automaticamente, os nomes e contratos podem diferir. Use o Swagger UI (ex.: http://bluesentinal.somee.com/swagger) para visualizar a lista completa, exemplos de request/response e exigência de autorização em seu ambiente de execução.

--------------------------------------------------------------------------------
ENDPOINTS /Usuarios (controller: BlueSentinal/Controllers/UsuariosController.cs)
Base: http://bluesentinal.somee.com/api/Usuarios  
- Route attribute: [Route("api/[controller]")]

1) GET /api/Usuarios
- Método: GET
- Descrição: retorna todos os usuários (lista de Usuario)
- Auth: não há [Authorize] nesse método no código atual (verificar política do ambiente)
- Response (200): array de usuários

2) DELETE /api/Usuarios/{id}
- Método: DELETE
- Rota: /api/Usuarios/{id}
- Auth: [Authorize(Roles = "Admin")]
- Descrição: exclui usuário por id
- Responses:
  - 204 No Content em sucesso
  - 404 Not Found se usuário não existir

3) DELETE /api/Usuarios/me
- Método: DELETE
- Rota: /api/Usuarios/me
- Auth: [Authorize]
- Descrição: exclui a própria conta do usuário autenticado (obtém userId do claim NameIdentifier)
- Responses:
  - 204 No Content em sucesso
  - 401 Unauthorized se token inválido/ausente
  - 404 Not Found se usuário não encontrado
  - 400 Bad Request em erros ao deletar (retorna descrição)

4) POST /api/Usuarios/adicionarRole
- Método: POST
- Rota: /api/Usuarios/adicionarRole
- Auth: [Authorize(Roles = "Admin")]
- Parâmetros: userId (string), role (string) — podem ser fornecidos via query ou body
- Descrição: adiciona uma role a um usuário; cria a role se não existir
- Responses:
  - 200 OK com mensagem de sucesso
  - 400 Bad Request se usuário já tiver a role ou erro na criação
  - 404 Not Found se usuário não encontrado

5) GET /api/Usuarios/me
- Método: GET
- Rota: /api/Usuarios/me
- Auth: [Authorize]
- Descrição: retorna dados do usuário autenticado (Id, UserName, Email, Nome, Nascimento, Roles)
- Responses:
  - 200 OK com objeto do usuário
  - 401/404 conforme aplicável

--------------------------------------------------------------------------------
ENDPOINTS /DroneFabris (controller: BlueSentinal/Controllers/DroneFabrisController.cs)
Base: http://bluesentinal.somee.com/api/DroneFabris  
- Route attribute: [Route("api/[controller]")]
- Controller decorado com [Authorize(Roles = "Admin")] — exige Admin para todos os métodos

1) GET /api/DroneFabris
- Método: GET
- Descrição: lista todos os DroneFabri
- Auth: Admin
- Response (200): array de DroneFabri

2) GET /api/DroneFabris/{id}
- Método: GET
- Rota: /api/DroneFabris/{id}
- Auth: Admin
- Descrição: retorna DroneFabri por id
- Responses: 200 com objeto, 404 se não encontrado

3) GET /api/DroneFabris/getByModel/{model}
- Método: GET
- Rota: /api/DroneFabris/getByModel/{model}
- Auth: Admin
- Descrição: retorna DroneFabri filtrados por modelo (case-insensitive)
- Responses: 200 OK, 400 Bad Request se model vazio, 404 se nenhum encontrado

4) PUT /api/DroneFabris/{id}
- Método: PUT
- Rota: /api/DroneFabris/{id}
- Auth: Admin
- Descrição: atualiza DroneFabri
- Responses: 204 No Content em sucesso, 400 Bad Request em payload inválido, 404 se id não existir

5) POST /api/DroneFabris
- Método: POST
- Auth: Admin
- Descrição: cria novo DroneFabri (verifica MAC duplicado)
- Responses: 201 Created com objeto, 400 em MAC duplicado

6) DELETE /api/DroneFabris/{id}
- Método: DELETE
- Auth: Admin
- Descrição: remove DroneFabri
- Responses: 204 No Content, 404 se não encontrado

--------------------------------------------------------------------------------
ENDPOINTS /Drones (controller: BlueSentinal/Controllers/DronesController.cs)
Base: http://bluesentinal.somee.com/api/Drones  
- Route attribute: [Route("api/[controller]")]

1) GET /api/Drones
- Método: GET
- Auth: [Authorize(Roles = "Admin")]
- Descrição: lista todos os drones (inclui DroneFabri e Usuario)
- Response: 200 OK array de drones

2) GET /api/Drones/getDroneUser
- Método: GET
- Rota: /api/Drones/getDroneUser
- Auth: [Authorize]
- Descrição: retorna drones vinculados ao usuário autenticado (usa claim NameIdentifier do token)
- Responses: 200 OK com lista de drones, 400 Bad Request se claim ausente

3) GET /api/Drones/getByUserName/{userName}
- Método: GET
- Rota: /api/Drones/getByUserName/{userName}
- Auth: [Authorize(Roles = "Admin")]
- Descrição: retorna drones filtrados por userName (procura usuários por Nome e então busca drones)
- Responses: 200 OK, 400 Bad Request se userName vazio, 404 se nenhum usuário/drones encontrados

4) POST /api/Drones/vincular?mac={mac}
- Método: POST
- Rota: /api/Drones/vincular
- Auth: [Authorize]
- Query param: mac (string)
- Body: objeto Drone (ex.: localizacao, tempoEmMili, status)
- Descrição: vincula um DroneFabri existente (identificado por mac) ao usuário autenticado criando a entidade Drone; marca DroneFabri.Status = true
- Responses: 200 OK com Drone criado, 404 se DroneFabri não encontrado, 400 se já vinculado ou usuário sem login

5) PUT /api/Drones/tempo/{id}
- Método: PUT
- Rota: /api/Drones/tempo/{id}
- Auth: (não há atributo explícito no método; apenas [HttpPut("tempo/{id}")])
- Body: AtualizarTempoStatusDto { Tempo: long, Status: bool }
- Descrição: atualiza TempoEmMili, tempoEmHoras e Status do drone
- Responses: 200 OK com drone atualizado, 404 se drone não encontrado

6) DELETE /api/Drones/{id}
- Método: DELETE
- Rota: /api/Drones/{id}
- Auth: [Authorize]
- Descrição: remove Drone por id e seta DroneFabri.Status = false
- Responses: 204 No Content, 404 se não encontrado

--------------------------------------------------------------------------------
Outras observações do Program.cs
- O projeto cria roles padrão no startup: Admin e User (ver Program.cs) se não existirem.
- Swagger e SwaggerUI estão habilitados (app.UseSwagger(); app.UseSwaggerUI()).
- MapControllers() é chamado, portanto rotas com atributos [Route] e padrões "api/[controller]" estão ativos.

--------------------------------------------------------------------------------
Exemplos rápidos (curl)
- Registrar:
curl -X POST "http://bluesentinal.somee.com/Usuario/registrar" -H "Content-Type: application/json" -d '{"email":"usuario@example.com","password":"Senha123!","confirmPassword":"Senha123!"}'

- Login (exemplo genérico, rota pode variar):
curl -X POST "http://bluesentinal.somee.com/Usuario/login" -H "Content-Type: application/json" -d '{"email":"usuario@example.com","password":"Senha123!"}'

- List users (controller):
curl "http://bluesentinal.somee.com/api/Usuarios"

- Vincular drone (usuário autenticado):
curl -X POST "http://bluesentinal.somee.com/api/Drones/vincular?mac=AA:BB:CC:DD:EE:FF" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d '{"localizacao":"Gate A","tempoEmMili":0,"status":false}'

--------------------------------------------------------------------------------
Observações finais e recomendações
- Para obter os schemas e respostas exatas (incluindo formatos retornados pelo MapIdentityApi em runtime), abra o Swagger no ambiente: http://bluesentinal.somee.com/swagger
- Verifique se endpoints sensíveis (ex.: listar todos usuários) devem ser públicos. Atualmente GET /api/Usuarios não exige autorização no código; considere proteger com [Authorize(Roles = "Admin")] se necessário.
- Padronize erros: algumas rotas retornam strings; recomenda-se usar um objeto de erro consistente: { "error": { "code": ..., "message":"...", "details": [...] } }.
- Habilite HTTPS em produção e restrinja CORS em ambiente real.

--------------------------------------------------------------------------------
