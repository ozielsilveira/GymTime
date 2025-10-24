# 🏋️ GymTime - Sistema de Gerenciamento de Academia

API RESTful para gerenciamento de academias, membros, aulas e agendamentos.

## 🚀 Quick Start

```bash
# 1. Clonar o repositório
git clone https://github.com/ozielsilveira/GymTime.git
cd GymTime

# 2. Iniciar com Docker Compose
docker-compose up -d --build

# 3. Aguardar ~30-40 segundos para migrations automáticas

# 4. Acessar
# - API: http://localhost:5000
# - Swagger: http://localhost:5000
# - Prometheus: http://localhost:9090
# - Grafana: http://localhost:3000 (admin/admin)
```

**Pronto!** As migrations do banco de dados são aplicadas **automaticamente** durante o startup. 🎉

---

## ✨ Novidades

### 🔄 Auto-Migration (Novo!)
- ✅ **Não precisa mais rodar scripts de inicialização**
- ✅ Migrations aplicadas automaticamente no startup da API
- ✅ Health checks garantem ordem correta de inicialização
- ✅ Logs detalhados de todo o processo

📖 Detalhes: [observability/AUTO-MIGRATION.md](observability/AUTO-MIGRATION.md)

---

## 📋 Pré-requisitos

- **Docker** >= 20.10
- **Docker Compose** >= 2.0
- **Portas livres**: 5000, 5432, 9090, 3000

### Desenvolvimento Local (Opcional)
- **.NET SDK 9.0**
- **PostgreSQL 16** (se não usar Docker)

---

## 🏗️ Arquitetura

```
GymTime/
├── GymTime.Api/              # Camada de apresentação (Controllers, Middleware)
├── GymTime.Application/      # Camada de aplicação (Services, DTOs)
├── GymTime.Domain/           # Camada de domínio (Entities, Interfaces)
├── GymTime.Infrastructure/   # Camada de infraestrutura (DB, Repositories)
├── GymTime.*.Tests/          # Testes unitários de cada camada
├── observability/# Prometheus, Grafana, documentação
└── docker-compose.yml        # Orquestração de containers
```

### Stack Tecnológica
- **Framework**: .NET 9
- **Database**: PostgreSQL 16
- **ORM**: Entity Framework Core
- **Authentication**: JWT
- **Logging**: Serilog
- **Metrics**: OpenTelemetry + Prometheus
- **Visualization**: Grafana
- **Containerization**: Docker

---

## 📊 Observability Stack

O projeto inclui stack completa de observabilidade:

- **Prometheus**: Coleta de métricas (http://localhost:9090)
- **Grafana**: Visualização de métricas (http://localhost:3000)
  - 📊 **Dashboard pré-configurado**: "GymTime API - Métricas"
  - 🔐 Login: `admin` / `admin`
  - 📖 [Guia Completo do Grafana](observability/GRAFANA-GUIDE.md)
- **Serilog**: Logging estruturado
- **OpenTelemetry**: Instrumentação da API
- **Health Checks**: `/health` endpoint

📖 **Documentação Completa**: [observability/README.md](observability/README.md)  
🚀 **Setup Rápido**: [observability/QUICKSTART.md](observability/QUICKSTART.md)  
🔧 **Troubleshooting**: [observability/TROUBLESHOOTING.md](observability/TROUBLESHOOTING.md)  
📊 **Grafana Dashboard**: [observability/GRAFANA-GUIDE.md](observability/GRAFANA-GUIDE.md)

### 🎯 Testar Métricas

Gere tráfego para ver as métricas no dashboard:

```powershell
# Windows
.\observability\generate-traffic.ps1

# Linux/Mac
chmod +x observability/generate-traffic.sh
./observability/generate-traffic.sh
```

Depois acesse: http://localhost:3000/d/gymtime-api-metrics

---

## 🔍 Endpoints Principais

### Health & Metrics
- `GET /health` - Health check
- `GET /metrics` - Métricas Prometheus

### Gym Members
- `GET /api/GymMember` - Listar membros
- `GET /api/GymMember/{id}` - Buscar membro
- `POST /api/GymMember` - Criar membro
- `PUT /api/GymMember/{id}` - Atualizar membro
- `DELETE /api/GymMember/{id}` - Deletar membro

### Classes
- `GET /api/Class` - Listar aulas
- `POST /api/Class` - Criar aula
- ...

### Bookings
- `GET /api/Booking` - Listar agendamentos
- `POST /api/Booking` - Criar agendamento
- ...

📖 **Swagger UI**: http://localhost:5000 (Development)

---

## 🧪 Testes

```bash
# Rodar todos os testes
dotnet test

# Rodar testes de um projeto específico
dotnet test GymTime.Api.Tests

# Com cobertura
dotnet test /p:CollectCoverage=true
```

---

## 🛠️ Desenvolvimento

### Executar localmente (sem Docker)

1. **Iniciar PostgreSQL**:
   ```bash
   docker run -d \
     -p 5432:5432 \
     -e POSTGRES_USER=postgres \
     -e POSTGRES_PASSWORD=postgres \
     -e POSTGRES_DB=gymtime \
     postgres:16
   ```

2. **Aplicar migrations**:
   ```bash
   cd GymTime.Api
   dotnet ef database update --project ../GymTime.Infrastructure
   ```

3. **Rodar API**:
   ```bash
   cd GymTime.Api
   dotnet run
   ```

### Criar nova migration

```bash
cd GymTime.Api
dotnet ef migrations add NomeDaMigration --project ../GymTime.Infrastructure
```

### Rollback migration

```bash
cd GymTime.Api
dotnet ef database update NomeMigrationAnterior --project ../GymTime.Infrastructure
```

---

## 🐳 Docker

### Comandos Úteis

```bash
# Ver logs
docker logs -f gymtime-api
docker logs -f gymtime-db

# Ver migrations sendo aplicadas
docker logs gymtime-api | grep -i migration

# Reiniciar API (re-aplica migrations)
docker-compose restart gymtime-api

# Rebuild completo
docker-compose down -v
docker-compose up -d --build

# Verificar saúde
docker ps
curl http://localhost:5000/health
```

### Health Checks

Os containers incluem health checks automáticos:
- **PostgreSQL**: Verifica com `pg_isready`
- **API**: Verifica endpoint `/health`

A API só inicia após o PostgreSQL estar saudável.

---

## 📝 Variáveis de Ambiente

### API Container (`docker-compose.yml`)

```yaml
ASPNETCORE_ENVIRONMENT: "Development"
ASPNETCORE_URLS: "http://+:80"
ConnectionStrings__GymTimeDbConnection: "Host=gymtime-db;..."
Jwt__Issuer: "GymTime"
Jwt__Audience: "GymTimeClients"
Jwt__SecretKey: "..."  # Mudar em produção!
Observability__ServiceName: "GymTime.Api"
Observability__ServiceVersion: "1.0.0"
```

---

## 🔒 Segurança

- **JWT Authentication**: Todos os endpoints requerem autenticação (exceto `/health`, `/metrics`)
- **CORS**: Configurado para desenvolvimento (`AllowAnyOrigin`)
  - ⚠️ **Produção**: Alterar para domínios específicos
- **Rate Limiting**: Implementado para prevenir abuso

⚠️ **IMPORTANTE**: Trocar `Jwt__SecretKey` em produção!

---

## 📦 CI/CD

O projeto está preparado para CI/CD com:
- ✅ Auto-migrations (não requer intervenção manual)
- ✅ Health checks
- ✅ Testes automatizados
- ✅ Docker multi-stage builds
- ✅ Logging estruturado

---

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/MinhaFeature`
3. Commit: `git commit -m 'Adiciona MinhaFeature'`
4. Push: `git push origin feature/MinhaFeature`
5. Abra um Pull Request

---

## 📄 Licença

Este projeto é de uso educacional.

---

## 🐛 Problemas Comuns

### API demora para iniciar
- Normal! Migrations estão sendo aplicadas (30-40s)
- Veja o progresso: `docker logs -f gymtime-api`

### Prometheus não coleta métricas
- Aguarde 10-15 segundos após startup
- Verifique: http://localhost:9090/targets

📖 **Mais soluções**: [observability/TROUBLESHOOTING.md](observability/TROUBLESHOOTING.md)

---

## 👤 Autor

**Oziel Silveira**
- GitHub: [@ozielsilveira](https://github.com/ozielsilveira)

---

## 📚 Documentação Adicional

- 📖 [Observability README](observability/README.md) - Stack de observabilidade
- 🚀 [Quick Start](observability/QUICKSTART.md) - Setup rápido
- 🔧 [Troubleshooting](observability/TROUBLESHOOTING.md) - Solução de problemas
- ✨ [Auto-Migration](observability/AUTO-MIGRATION.md) - Migrations automáticas
- 📜 [Scripts](observability/SCRIPTS.md) - Referência de scripts
- 📊 [Grafana Dashboard](observability/GRAFANA-GUIDE.md) - Guia completo do dashboard
