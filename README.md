# MemoirMind AI Assistant 🧠

[![Build Status](https://github.com/abusalehnayeem/memoirmind-ai-assistant/actions/workflows/build-deploy.yml/badge.svg)](https://github.com/abusalehnayeem/memoirmind-ai-assistant/actions)  
[![License](https://img.shields.io/github/license/abusalehnayeem/memoirmind-ai-assistant )](https://github.com/abusalehnayeem/memoirmind-ai-assistant/blob/main/LICENSE)  
[![.NET Core](https://img.shields.io/badge/.NET-Core-blue?logo=dotnet)]( https://dotnet.microsoft.com/)  
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-3161C2?logo=postgresql)]( https://www.postgresql.org/)  
[![pgvector](https://img.shields.io/badge/pgvector-546e7a?logo=postgresql)]( https://github.com/pgvector/pgvector)  
[![Ollama](https://img.shields.io/badge/Ollama-FF6B6B?logo=ollama)]( https://ollama.ai/)  
[![Semantic Kernel](https://img.shields.io/badge/Semantic_Kernel-0078D7?logo=microsoft)]( https://learn.microsoft.com/en-us/semantic-kernel/)  
[![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker)]( https://www.docker.com/)  
[![GDPR Compliant](https://img.shields.io/badge/GDPR-Compliant-4285F4?logo=data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAiIGhlaWdodD0iMTAiIHZpZXdCb3g9IjAgMCAxMCAxMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cGF0aCBkPSJNMCAwaDEwVjEwSDBWMEgweiIgZmlsbD0iIzQyODVGNCIvPjwvc3ZnPg==)](docs/GDPR-Compliance.md)  
[![Cloud Ready]( https://img.shields.io/badge/Cloud_Ready-Azure%2FAWS%2FGCP-333 )](docs/Skills-Demonstrated.md#cloud--devops)  

# MemoirMind AI Assistant 🧠

A personalized AI assistant built with **C#/.NET Core**, **PostgreSQL (pgvector)**, and **Ollama (Semantic Kernel)**. Designed for **long-term memory retention**, automation, and GDPR compliance — ideal for showcasing full-stack and AI integration skills.

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

# Run backend
dotnet run --project src/TelegramBot.Api
