# 项目 上下文

## 目的
本项目的目标是构建一个基于 **ABP Framework** 的分层单体架构应用程序，集成 **OpenTelemetry (OTel)** 以实现全面的可观测性（日志、指标、链路追踪）。
项目名为 `VoloAbp.OTel`，旨在演示或实现一个具有生产级监控能力的现代 .NET Web 应用。

## 技术栈
- **后端框架**: .NET 9.0 (C#)
- **应用框架**: ABP Framework 9.3.6
- **数据库**: MySQL (Entity Framework Core)
- **前端技术**: ASP.NET Core MVC / Razor Pages (使用 LeptonX Lite 主题)
- **可观测性**: OpenTelemetry (.NET SDK)
    - Exporters: OTLP, Console
    - Instrumentation: ASP.NET Core, EF Core, HttpClient, StackExchange.Redis, Hangfire, Runtime, Process
- **身份认证**: OpenIddict (集成在 ABP 模块中)
- **其他依赖**:
    - Serilog (日志)
    - AutoMapper (对象映射)
    - Node.js (v18/v20, 用于前端构建)

## 项目约定

### 代码风格
-遵循标准 C# 编码规范 (Microsoft conventions)。
- 使用 `.editorconfig` 进行统一格式化（缩进 4 空格，C#）。
- `LangVersion` 设置为 `latest`。

### 架构模式
- **领域驱动设计 (DDD)**: 分层架构
    - `Domain`: 核心业务逻辑，实体，值对象。
    - `Domain.Shared`: 领域层共享的常量、枚举、本地化资源。
    - `Application`: 应用服务，DTOs，用例编排。
    - `Application.Contracts`: 应用层接口和 DTO 定义。
    - `EntityFrameworkCore`: 数据访问层 (EF Core 配置，仓储实现)。
    - `HttpApi`: API 控制器定义。
    - `Web`: UI 层 (MVC/Razor Pages)，也作为宿主应用。
    - `DbMigrator`: 数据库迁移控制台应用。

### 测试策略
- **单元/集成测试**: 使用 xUnit。
- 测试项目位于 `test/` 目录下，对应各层级 (e.g., `Domain.Tests`, `Application.Tests`)。
- 使用 ABP 提供的测试基类 (`AbpIntegratedTest<T>`) 进行集成测试。

### Git工作流
- 遵循标准 Git 工作流。
- 提交信息应清晰描述变更内容。

## 领域上下文
- 项目目前包含基本的 ABP 模块（身份管理、多租户、审计日志等）。
- `Authors` 和 `Books` 目录（在 `Application` 和 `Domain` 层）暗示存在一个图书管理或类似的示例业务域。

## 重要约束
- 需要 .NET 9.0 SDK。
- 需要 Node.js环境运行前端构建任务。
- 生产环境需要配置 OpenIddict 的签名证书。

## 外部依赖
- **MySQL**: 主数据库。
- **Redis** (可选/潜在): OpenTelemetry 包含 Redis 埋点，暗示可能使用 Redis 作为缓存或分布式锁。