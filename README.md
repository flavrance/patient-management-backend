# FinX - Sistema de Gestão de Pacientes

## Descrição
O FinX é um sistema de gestão de pacientes desenvolvido em .NET 9 que permite o gerenciamento completo de informações de pacientes em uma clínica ou consultório médico.

## Tecnologias Utilizadas
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker
- AutoMapper
- FluentValidation
- JWT (JSON Web Tokens)
- Polly (para políticas de retry)
- xUnit (para testes)

## Estrutura do Projeto
O projeto segue a arquitetura limpa (Clean Architecture) e está organizado nas seguintes camadas:

- **FinX.Domain**: Contém as entidades de domínio e interfaces de repositório
- **FinX.Application**: Contém os DTOs, serviços, validadores e mapeamentos
- **FinX.Infrastructure**: Contém a implementação do banco de dados e repositórios
- **FinX.API**: Contém os controllers e configuração da API
- **FinX.UnitTests**: Contém os testes unitários

## Configuração do Ambiente de Desenvolvimento

### Pré-requisitos
- .NET 9 SDK
- Docker Desktop
- Visual Studio 2022 ou VS Code

### Executando o Projeto
1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/finx.git
   cd finx
   ```

2. Certifique-se de que o Docker Desktop esteja em execução
   
3. Execute o Docker Compose:
   ```bash
   docker-compose down -v # Remove containers e volumes anteriores, se necessário
   docker-compose up -d --build
   ```

4. Verifique se os contêineres estão em execução:
   ```bash
   docker ps
   ```

5. A API estará disponível em:
   - HTTP: http://localhost:5000

### Banco de Dados e Migrações
O projeto utiliza o Entity Framework Core para gerenciar o banco de dados PostgreSQL e aplica automaticamente as migrações durante o início da aplicação:

1. As migrações são executadas automaticamente ao iniciar a aplicação, se necessário
2. Se o banco de dados já foi criado e as migrações já foram aplicadas, o sistema detecta isso e não executa novamente
3. O aplicativo implementa um mecanismo de retry usando Polly para garantir conexões confiáveis com o banco de dados
4. Para criar novas migrações após alterações no modelo de dados:
   ```bash
   cd src/FinX.API
   dotnet ef migrations add NomeDaMigracao --project ../FinX.Infrastructure --startup-project .
   ```

5. Para aplicar manualmente as migrações (geralmente não necessário):
   ```bash
   dotnet ef database update --project ../FinX.Infrastructure --startup-project .
   ```

### Solução de Problemas com Docker
Se você encontrar problemas ao acessar a aplicação através da porta 5000:

1. Verifique se os contêineres estão em execução:
   ```bash
   docker ps
   ```

2. Verifique os logs do contêiner da API:
   ```bash
   docker logs finx-api-1
   ```

3. Se ocorrerem erros de conexão com o banco de dados:
   ```bash
   # Verifique se o contêiner do PostgreSQL está em execução
   docker ps | grep postgres
   
   # Verifique os logs do PostgreSQL
   docker logs finx-db-1
   
   # Reinicie os contêineres em ordem
   docker-compose down
   docker-compose up -d db      # Inicie primeiro o banco de dados
   docker-compose up -d api     # Depois inicie a API
   ```

4. Se necessário, reinicie os contêineres:
   ```bash
   docker-compose down
   docker-compose up -d
   ```

5. Verifique se não há outros serviços usando a porta 5000 em sua máquina.

### Acessando o Swagger UI
Para explorar e testar os endpoints da API através da interface do Swagger:

1. Abra seu navegador e acesse:
   ```
   http://localhost:5000/swagger
   ```
   
   Se não funcionar, tente também:
   ```
   http://localhost:5000/swagger/index.html
   ```

2. Na interface do Swagger você poderá:
   - Visualizar todos os endpoints disponíveis da API
   - Testar os endpoints diretamente pelo navegador
   - Verificar os modelos de requisição e resposta
   - Executar operações de CRUD nos recursos disponíveis

3. Para autorização:
   - Use o endpoint `/api/auth/login` para obter um token JWT
   - Use as seguintes credenciais:
     - Email: `admin@finx.com.br`
     - Senha: `123456`
   - Clique no botão "Authorize" no topo da página do Swagger
   - Insira o token recebido no formato: `Bearer {seu-token-jwt}`
   - Após autenticado, você poderá acessar endpoints protegidos

### Segurança da API
Todos os endpoints da API são protegidos por autenticação JWT, com exceção do endpoint de login:

- O endpoint de login (`/api/auth/login`) é o único que pode ser acessado sem autenticação
- Todos os outros endpoints exigem um token JWT válido
- Os tokens expiram após 60 minutos
- Para acessar qualquer endpoint protegido, inclua o header `Authorization: Bearer {seu-token}` em todas as requisições

O Swagger facilita o entendimento e teste da API sem a necessidade de ferramentas externas como Postman ou Insomnia.

### Endpoints da API

#### Autenticação
- `POST /api/auth/login`: Autenticar usuário e obter token JWT

#### Pacientes
- `GET /api/patients`: Lista todos os pacientes (com paginação)
- `GET /api/patients/{id}`: Obtém um paciente específico
- `POST /api/patients`: Cria um novo paciente
- `PUT /api/patients/{id}`: Atualiza um paciente existente
- `DELETE /api/patients/{id}`: Remove um paciente

#### Consultas
- `GET /api/appointments`: Lista todas as consultas (com paginação)
- `GET /api/appointments/{id}`: Obtém uma consulta específica
- `POST /api/appointments`: Cria uma nova consulta
- `PUT /api/appointments/{id}`: Atualiza uma consulta existente
- `DELETE /api/appointments/{id}`: Remove uma consulta

#### Histórico Médico
- `GET /api/medicalhistory/patient/{patientId}`: Obtém todo o histórico médico de um paciente
- `GET /api/medicalhistory/{id}`: Obtém um registro específico do histórico médico
- `POST /api/medicalhistory`: Adiciona um novo registro ao histórico médico
- `PUT /api/medicalhistory/{id}`: Atualiza um registro do histórico médico
- `DELETE /api/medicalhistory/{id}`: Remove um registro do histórico médico

#### Exames Externos
- `GET /api/externalexams/patient/{patientId}`: Obtém todos os exames externos de um paciente
  > **Nota**: Este endpoint utiliza um serviço mock para demonstração

## Testes
Para executar os testes unitários:
```bash
dotnet test
```

## Contribuição
1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Faça commit das suas alterações (`git commit -m 'Adiciona nova feature'`)
4. Faça push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Licença
Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes. 