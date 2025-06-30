[![Build Status](https://github.com/abusalehnayeem/your-repo-name/actions/workflows/build-deploy.yml/badge.svg )](https://github.com/abusalehnayeem/your-repo-name/actions )
[![License: MIT](https://img.shields.io/github/license/abusalehnayeem/your-repo-name )](https://github.com/abusalehnayeem/your-repo-name/blob/main/LICENSE )
[![GitHub Stars](https://img.shields.io/github/stars/abusalehnayeem/your-repo-name )](https://github.com/abusalehnayeem/your-repo-name/stargazers )
[![Docker Pulls](https://img.shields.io/docker/pulls/your-dockerhub-username/your-image-name )](https://hub.docker.com/r/your-dockerhub-username/your-image-name )
[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/abusalehnayeem/your-repo-name?label=code%20quality)]( https://www.codefactor.io/repository/github/abusalehnayeem/your-repo-name )
[![Dependabot Enabled](https://img.shields.io/badge/dependabot-enabled-brightgreen )](https://github.com/abusalehnayeem/your-repo-name/security/dependabot )
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?logo=postgresql&logoColor=white)]( https://www.postgresql.org/ )
[![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)]( https://dotnet.microsoft.com/ )
[![.NET Core](https://img.shields.io/badge/.NET_Core-5C2D91?logo=dotnet&logoColor=white)]( https://dotnet.microsoft.com/ )
[![Ollama](https://img.shields.io/badge/Ollama-4A7856?logo=ollama&logoColor=white)]( https://ollama.ai/ )

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
