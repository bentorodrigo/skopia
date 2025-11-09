# 🧩 Gerenciador de Projetos - API RESTful em .NET 8

Uma API RESTful desenvolvida em **C# e .NET 8**, utilizando **Entity Framework Core com banco de dados em memória**, projetada para gerenciar **usuários, projetos e tarefas**.  
O sistema segue princípios **Domain-Driven Design (DDD)** e **boas práticas REST**, com suporte completo a **Swagger/OpenAPI** para documentação interativa.

---

## 🚀 Tecnologias utilizadas

- **.NET 8**
- **Entity Framework Core (InMemory)**
- **ASP.NET Core Web API**
- **Swagger / Swashbuckle**
- **LINQ e Programação Assíncrona**
- **Injeção de Dependência (DI)**
- **C# 12 / Records / DTOs**
- **DDD + Vertical Slice Architecture**

---

## 🧱 Estrutura das entidades principais

### 🧑 User / Usuario
Representa a pessoa que utiliza o sistema e é proprietária de um ou mais projetos.

| Campo | Tipo | Descrição |
|--------|------|-----------|
| `Id` | `int` | Identificador único (auto incremento) |
| `Name` | `string` | Nome do usuário |
| `Email` | `string` | Endereço de e-mail |
| `Function` | `UserFunction` | Enum (`Normal`, `Gerente`) |

---

### 📁 Project / Projeto
Um projeto pode conter até **20 tarefas**. Cada usuário pode possuir vários projetos.

| Campo | Tipo | Descrição |
|--------|------|-----------|
| `Id` | `int` | Identificador único (auto incremento) |
| `Name` | `string` | Nome do projeto |
| `Description` | `string` | Descrição opcional |
| `UserId` | `int` | ID do usuário criador |
| `TaskItems` | `List<TaskItem>` | Lista de tarefas vinculadas |

**Regras de negócio:**
- Um projeto **não pode ser removido** se ainda houver tarefas pendentes.
- É sugerido ao usuário concluir ou remover as tarefas antes da exclusão.

---

### ✅ TaskItem / Tarefa
Unidade de trabalho pertencente a um projeto.

| Campo | Tipo | Descrição |
|--------|------|-----------|
| `Id` | `int` | Identificador único |
| `Title` | `string` | Título da tarefa |
| `Description` | `string` | Detalhes da tarefa |
| `DueDate` | `DateTime` | Data de vencimento |
| `Status` | `TaskStatus` | Enum (`Pendente`, `EmAndamento`, `Concluida`) |
| `Priority` | `TaskPriority` | Enum (`Baixa`, `Media`, `Alta`) |
| `ProjectId` | `int` | ID da tarefa atribuida |
| `Comments` | `List<Comment>` | Lista de comentários da tarefa |
| `Historic` | `List<TaskHistory>` | Lista de historico de alterações da tarefa |

**Regras de negócio:**
- A **prioridade** é definida na criação e **não pode ser alterada**.
- Cada **atualização** gera um registro no **histórico de alterações**.
- Cada **projeto** pode conter **no máximo 20 tarefas**.
- Usuários podem **adicionar comentários**, que também são registrados no histórico.

---

## 🧮 Relatórios e restrições

- Apenas usuários com **função “Gerente”** podem gerar relatórios de desempenho.
- Os relatórios incluem:
  - Média de tarefas concluídas por usuário nos últimos 30 dias.
- Implementação disponível via **endpoints dedicados (futuros)**.

---

## ⚙️ Configuração e execução do projeto

### 🧩 Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Editor recomendado: [Visual Studio Code](https://code.visualstudio.com/) ou [Rider](https://www.jetbrains.com/rider/)

### ▶️ Executando a API 

#### 🐳 Execução com Docker

O projeto pode ser facilmente executado dentro de um container Docker, sem necessidade de instalar o .NET SDK localmente.

#### 📦 **Pré-requisitos**

Antes de iniciar, certifique-se de ter instalado:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (ou Docker Engine)
- [Git](https://git-scm.com/downloads)

#### ⚙️ **Construindo a imagem**

Com o terminal aberto na **pasta raiz da solução (`Skopia/`)**, execute o comando:

```bash
docker build -t skopia-api .
```

#### ▶️ **Executando a imagem**

Com o terminal ainda aberto e após a execução do comando anterior, realize a execução do seguinte comando:

```bash
docker run -d -p 8080:8080 -p 8081:8081 --name skopia-container skopia-api
```

#### 🌐 Acessando a aplicação

Depois que o container estiver em execução, a API estará disponível em:

👉 http://localhost:8080/swagger

Essa rota exibe a documentação interativa (Swagger UI) da aplicação.

#### 🛑 Encerrando a execução

Para parar e remover o container:

```
docker stop skopia-container
docker rm skopia-container
```

Se quiser remover também a imagem:

```
docker rmi skopia-api
```

## 🧭 Perguntas para o Product Owner (Refinamento e Melhorias Futuras) - Fase 2

Abaixo estão perguntas direcionadas na qual eu pensei e idealizei para guiar essas conversas e garantir clareza nos objetivos, regras de negócio e impactos esperados.

---

### 🎯 Contexto e Objetivos
- Qual o principal objetivo de negócio dessa funcionalidade? (ex: melhorar produtividade, facilitar acompanhamento de projetos, gerar relatórios gerenciais)
- Essa melhoria visa resolver algum problema específico percebido pelos usuários (ex: dificuldade em visualizar progresso ou registrar tarefas)?
- Como essa funcionalidade contribui para o controle e gestão de projetos e equipes?

---

### 👥 Usuário e Jornada
- Quais perfis de usuários utilizarão essa funcionalidade? (ex: gerente, colaborador, administrador)
- Há diferenças na forma como gerentes e usuários comuns interagem com tarefas e relatórios?
- Quais são as maiores dores ou dificuldades que os usuários enfrentam atualmente na criação, atualização ou exclusão de tarefas?

---

### ⚙️ Regras de Negócio e Fluxo
- Existem limites de tarefas por projeto, ou regras de prioridade específicas que devem ser respeitadas?
- Como deve funcionar o fluxo de atualização de status das tarefas (ex: de “Em andamento” para “Concluída”)?
- O histórico de alterações e comentários deve registrar todas as mudanças, inclusive as feitas por administradores?
- O que deve acontecer se o usuário tentar criar um projeto ou tarefa com informações incompletas?

---

### 💾 Dados, Relatórios e Integrações
- Quais dados devem ser exibidos nos relatórios de desempenho (ex: número de tarefas concluídas, tempo médio de conclusão, percentual por usuário)?
- O relatório de desempenho deve abranger apenas os últimos 30 dias ou permitir filtros por período?
- Há planos de integração com ferramentas externas (ex: Power BI, Trello, Jira, planilhas, etc.)?
- Precisamos armazenar o histórico de relatórios gerados?

---

### 🔐 Segurança e Permissões
- Quais ações cada tipo de usuário pode realizar? (ex: apenas o gerente pode excluir tarefas ou gerar relatórios)
- O sistema deve restringir o acesso a relatórios de outros usuários?
- Há necessidade de registrar quem criou, editou ou excluiu cada tarefa ou projeto (auditoria)?
- Como deve ser tratado o acesso de usuários inativos ou removidos do sistema?

---

### 📈 Métricas e Indicadores
- Quais indicadores são mais relevantes para medir o desempenho das equipes e dos usuários?
- O PO deseja acompanhar métricas de uso do sistema (quantidade de tarefas criadas, concluídas, tempo médio de execução)?
- Há interesse em implementar gráficos ou dashboards de acompanhamento visual?

---

### 🚀 Roadmap e Prioridades
- Quais são as próximas áreas do sistema que devem receber melhorias? (ex: comentários em tarefas, visualização de relatórios, performance)
- Essa melhoria é uma prioridade imediata ou faz parte de uma entrega futura?
- Há dependências com outros módulos ou integrações que precisam ser consideradas?
- Existe alguma restrição técnica, de prazo ou compliance que impacta essa entrega?

## 🧩 Possíveis Melhorias e Evoluções do Projeto

Durante a análise e implementação do projeto, foram identificados diversos pontos que podem ser aprimorados visando **melhor manutenibilidade, escalabilidade, performance e organização arquitetural**.  
Abaixo estão sugestões práticas de melhoria que podem orientar futuras evoluções do sistema.

---

### 🏗️ Arquitetura e Organização de Código - Fase 3

- **Adotar o padrão Vertical Slice completo:**  
  O projeto já segue parcialmente o conceito, mas pode ser aprimorado com:
  - Separação mais clara entre **Features** (ex: `Users`, `Projects`, `Reports`, `Tasks`), cada uma com seus próprios *handlers*, *controllers* e *mapeamentos*.
  - Redução de dependências entre camadas e menor acoplamento entre domínios.
  - Padronização de respostas e DTOs por *feature*.

- **Aplicação de princípios de DDD (Domain-Driven Design):**
  - Introduzir *Value Objects* e *Aggregates Roots* para entidades centrais (`User`, `Project`, `Task`).
  - Incluir serviços de domínio e eventos para capturar regras de negócio complexas (ex: notificação de tarefa concluída).
  - Facilitar testes e evolução sem quebrar regras do domínio.

- **Implementar CQRS (Command Query Responsibility Segregation):**
  - Separar operações de leitura (queries) e escrita (commands) para facilitar escalabilidade e otimização.
  - Facilitar cache e replicação de leitura em casos de crescimento do sistema.

- **Utilização de Mediator (ex: MediatR):**
  - Tornar o fluxo de comandos e consultas mais limpo e desacoplado.
  - Melhorar rastreabilidade e testes de unidade de cada operação.

- **Adoção de Automapper ou Mapster:**
  - Reduzir código repetitivo de mapeamento entre entidades e DTOs.
  - Facilitar manutenção e inclusão de novos campos.

- **Preparação para execução em ambiente Cloud:**
  - Estruturar o projeto para rodar facilmente em **containers Docker**.
  - Criar arquivos `Dockerfile` e `docker-compose.yml` padronizados para todos os serviços.
  - Definir variáveis de ambiente para *connection strings*, chaves e configurações sensíveis.

- **Integração com CI/CD (Continuous Integration / Continuous Deployment):**
  - Configurar pipeline no GitHub Actions, Azure DevOps ou GitLab CI para:
    - Executar testes automatizados.
    - Fazer build e push automático de containers.
    - Realizar deploy automatizado em ambiente de homologação ou produção.

- **Observabilidade e Monitoramento:**
  - Incluir logs estruturados (ex: com Serilog ou Elastic Stack).
  - Adicionar métricas e *health checks*.
  - Preparar o sistema para alertas e rastreamento de falhas em cloud.

- **Análise estática de código e padronização:**
  - Adicionar ferramentas como **SonarQube**, **Roslyn Analyzers** ou **StyleCop**.
  - Aplicar regras de nomenclatura, complexidade e acoplamento.

- **Melhoria nas queries e relatórios:**
  - Otimizar consultas LINQ com *projection* direta e *AsNoTracking()* para cenários de leitura.
  - Adicionar filtros de data e paginação em relatórios de usuários.
  - Permitir exportação de relatórios (ex: CSV, PDF).

- **Cache e performance:**
  - Introduzir cache em relatórios e consultas de leitura frequente (ex: via Redis).
  - Analisar índices no banco de dados e ajustar *queries* mais pesadas.

---