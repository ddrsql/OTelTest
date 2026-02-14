# 项目背景: AbpFramework.OTel

## 项目概览

这是一个基于 **ASP.NET Boilerplate (ABP) Framework v7.3.0** 构建的 **.NET Framework 4.6.2** 应用程序。它采用分层架构（Application, Core, EntityFramework, WebApi），并包含两个 Web 前端：
1.  **WebMpa:** 多页应用程序 (MVC)。
2.  **WebSpaAngular:** 使用 **AngularJS 1.x** 的单页应用程序 (SPA)。

该项目配置为使用 **MySQL** 作为数据库，并集成 **OpenTelemetry** 以实现可观测性。

## 关键技术

*   **框架:** .NET Framework 4.6.2
*   **基础框架:** ASP.NET Boilerplate (Legacy) 7.3.0
*   **数据库:** MySQL (通过 Entity Framework 6)
*   **前端:** AngularJS 1.8.2, Bootstrap, jQuery
*   **可观测性:** OpenTelemetry (Traces & Logs)

## 架构

*   **AbpFramework.OTel.Core:** 领域层（实体、领域服务、授权）。
*   **AbpFramework.OTel.EntityFramework:** 数据访问层（EF6 DBContext、迁移）。
*   **AbpFramework.OTel.Application:** 应用服务（DTO、应用服务）。
*   **AbpFramework.OTel.WebApi:** Web API 层。
*   **AbpFramework.OTel.WebMpa:** MVC Web 应用程序。
*   **AbpFramework.OTel.WebSpaAngular:** AngularJS SPA 宿主。

## 配置与设置

### 1. 先决条件
*   Visual Studio 2022（推荐），需安装 .NET Framework 4.6.2 目标包。
*   本地运行的 MySQL Server（默认端口 3306）。

### 2. 数据库连接
连接字符串配置在 `Web.config` 中（在 `WebMpa` 和 `WebSpaAngular` 中均可找到）。
**默认连接字符串:**
```xml
<add name="Default" connectionString="server=localhost;port=3306;database=AbpFrameworkOTel;uid=root;pwd=1q2w3E*;" providerName="MySql.Data.MySqlClient" />
```
*   **操作:** 确保您的本地 MySQL 凭据与此匹配，或更新 `Web.config` 文件。

### 3. OpenTelemetry 设置
OpenTelemetry 在 `Global.asax.cs` 和 `Web.config` 中配置。
*   **端点:** `http://localhost:4318` (OTLP HTTP)。
*   **服务名称:** `AbpFrameworkOTel`（或通过 `OTel_ServiceName` 配置）。
*   **启用状态:** 由 `Web.config` 中的 `<add key="OTel_Enabled" value="true" />` 控制。

### 4. 运行应用程序
*   在 Visual Studio 中打开 `AbpFramework.OTel.sln`。
*   将 `AbpFramework.OTel.WebMpa` 或 `AbpFramework.OTel.WebSpaAngular` 设置为启动项目。
*   运行解决方案 (F5)。

## 关键文件

*   **`AbpFramework.OTel.sln`**: 主解决方案文件。
*   **`AbpFramework.OTel.WebMpa/Web.config`**: 主配置（数据库连接、OTel 设置）。
*   **`AbpFramework.OTel.WebMpa/Global.asax.cs`**: 应用程序启动逻辑，包括 OpenTelemetry 初始化。
*   **`AbpFramework.OTel.Core/OTelCoreModule.cs`**: 核心模块配置。
*   **`AbpFramework.OTel.WebSpaAngular/App/Main/app.js`**: AngularJS 应用程序的主入口点。

## 开发说明

*   **NuGet 包:** 该项目似乎混合使用了 `packages.config`（旧式）。请确保在构建前还原 NuGet 包。
*   **前端资源:** AngularJS 脚本位于 `AbpFramework.OTel.WebSpaAngular/App`。