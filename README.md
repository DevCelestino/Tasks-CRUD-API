# Tasks CRUD API - Gabriel Celestino

## Descrição

Este projeto é uma REST API para gerenciamento de tarefas vinculadas a pessoas. Ele permite a criação, leitura, atualização e exclusão (CRUD) de tarefas, utilizando uma arquitetura que inclui serviços, controladores, DTOs, entidades e repositórios. A adição de tarefas é feita através de uma mensageria RabbitMQ, utilizando um consumer para processar as mensagens.

## Estrutura do Projeto

A estrutura do projeto é organizada da seguinte forma:

- `docker-compose.yml` - Arquivo de configuração do Docker Compose.

# Web API

- `Api/Controllers/` - Gerencia as requisições e respostas da API.
- `Application/DTOs/` - Objetos de transferência de dados utilizados na aplicação.
- `Application/Services/` - Contém a lógica de negócios da aplicação.
- `Domain/Entities/` - Define os modelos de dados utilizados.
- `Infrastructure/Repositories/` - Gerencia a persistência de dados.
- `Infrastructure/Migrations/` - Contém os arquivos de migração para o banco de dados.
- `Tests/` - Contém os testes automatizados da aplicação.

# Consumer

- `Application/DTOs/` - Objetos de transferência de dados utilizados na aplicação.
- `Application/Services/` - Contém a lógica de negócios da aplicação.
- `Domain/Entities/` - Define os modelos de dados utilizados.
- `Infrastructure/Repositories/` - Gerencia a persistência de dados.

## Tecnologias Utilizadas

- **ASP.NET Core e .NET 8.0** para a construção da API e do Consumer.
- **RabbitMQ** para mensageria.
- **MSSQL** como banco de dados.
- **Docker** para containerização.

## Pré-requisitos

- Docker e Docker Compose instalados na máquina.

## Execução

Para iniciar a aplicação, siga os passos abaixo:

1. Navegue até a pasta raiz do projeto:

   ```bash
   cd /caminho/para/o/projeto

2. Execute o comando:
   ```bash
   docker-compose up -d

Isso irá iniciar os seguintes containers:

- API (REST) `http://localhost:2000`
- Consumer (para processar tarefas via RabbitMQ) `http://localhost:2500`
- RabbitMQ (mensageria) `http://localhost:3000`
- MSSQL (banco de dados) `localhost,1343`

## Endpoints

A API oferece os seguintes endpoints (todos podem ser consultados através do Swagger em `http://localhost:2000/swagger`):

- `GET /Tasks?id={id}` - Retorna todas as tarefas se nenhum id especificado ou apenas as tarefas dos ids informados.
- `GET /Tasks/ByPersonIds?id={id}` - Retorna todas as pessoas com suas tarefas vinculadas se nenhum id especificado ou apenas as pessoas dos ids informados.
- `POST /Tasks` - Cria uma nova tarefa.
- `PUT /Tasks` - Atualiza uma tarefa existente.
- `DELETE /Tasks/{id}` - Remove uma tarefa.

## Contato

Se você tiver alguma dúvida, sinta-se à vontade para entrar em contato!