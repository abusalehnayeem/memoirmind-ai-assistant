# MemoirMind AI Assistant 🧠

A personalized AI assistant built with **C#/.NET Core**, **PostgreSQL (pgvector)**, and **Ollama (Semantic Kernel)**. Designed for **long-term memory retention**, automation, and GDPR compliance — ideal for showcasing full-stack and AI integration skills in European tech roles.

---

## 🎯 About  
This project demonstrates:
- **Full-stack expertise**: C#/.NET Core backend + Angular frontend (optional).
- **AI/ML integration**: Semantic Kernel + Ollama for LLM orchestration.
- **Data modeling**: PostgreSQL with `pgvector` for semantic search.
- **Enterprise-grade practices**: Security (JWT), GDPR compliance, cost-effective infrastructure.

---

## 🛠️ Tech Stack
| Component          | Technology                     |
|--------------------|--------------------------------|
| **Backend**        | ASP.NET Core 8                 |
| **Frontend**       | Angular 17 (optional)          |
| **Database**       | PostgreSQL + pgvector          |
| **LLM**            | Ollama + DeepSeek-R1-Distill-Qwen-7B |
| **Real-Time**      | SignalR (optional)             |
| **Cloud**          | Azure/AWS/GCP ready (optional) |

---

## 🌟 Key Features
| Feature              | Description                                                                 |
|----------------------|-----------------------------------------------------------------------------|
| **Scoped DB Control**| PostgreSQL schema (`memories`) with restricted role (`memories_role`).     |
| **Semantic Memory**  | `pgvector` for fuzzy concept retrieval (e.g., "last month's coffee spending").|
| **Scheduled Tasks**  | Cron jobs for automation (e.g., weekly reports).                            |
| **Telegram Bot**     | Natural language interface with real-time responses.                        |
| **Self-Evolving Prompts** | User preferences stored in `public.system_prompts`.                     |

---

## 💼 Why This Matters for Jobs in Europe
1. **Cloud Skills**: Azure PostgreSQL/Docker integration aligns with enterprise trends.
2. **GDPR Compliance**: Structured data storage demonstrates data privacy awareness.
3. **Cost Efficiency**: Uses open-source tools (Ollama) instead of paid LLM APIs.
4. **DevOps**: GitHub Actions for CI/CD pipelines.

---

## 🚀 Getting Started
### Local Setup (Docker)
```bash
# Start PostgreSQL + pgvector
docker-compose -f docker/postgres.Dockerfile up -d

# Start Ollama
docker-compose -f docker/ollama.Dockerfile up -d

# Configure environment variables
cp .env.example .env

# Run backend
dotnet run --project src/TelegramBot.Api
