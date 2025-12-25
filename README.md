# 从“黑盒”到“全景”：在.NET中拥抱OpenTelemetry与SigNoz的可观测工具+

> 作为开发者，你是否曾为这些问题困扰过？

* 问题排查像“开盲盒”：用户报告接口慢，你只能埋头翻看分散的日志文件，试图从碎片信息中拼凑出一次请求的完整路径。
* 监控数据“烟囱林立”：Metrics、Tracing、Logs 分散在不同的系统里，查看一个完整的问题上下文需要在多个标签页间反复横跳。
* 技术栈绑定之痛：一旦选用了某个APM厂商的SDK，迁移成本高到让人望而却步。

如果你对以上任何一点深感共鸣，那么是时候了解 OpenTelemetry（简称OTel）​ 和 SigNoz​ 这个组合了。它们一个定义了开放标准，一个提供了开箱即用的实现。


## OpenTelemetry：可观测性
想象一下，你的应用是一个由微服务、数据库、消息队列组成的复杂城市。过去，每个组件（或APM厂商）都说着自己的“方言”（私有协议），导致沟通成本极高。
OpenTelemetry (OTel) 就是为解决这个问题而生的。​ 它由CNCF孵化，目标是为可观测性数据（追踪、指标、日志）提供一套与供应商无关的、统一的采集和传输标准。你可以把它理解为可观测性领域的“普通话”或“USB-C接口”。

它的核心优势在于：
* 供应商中立： 用一套SDK和API完成数据采集，通过简单的配置即可将数据发送到任何支持OTLP协议的后端（如kibana、Jaeger、zipkin、Tempo、SigNoz）。
* 一次集成，多处可用： 避免了对特定APM厂商的锁定，未来切换后端成本极低。

## SigNoz：开箱即用的全景观测平台
### 安装signoz
https://signoz.io/docs/install/docker/
```shell
# 使用 Docker Compose 安装 SigNoz
git clone -b main https://github.com/SigNoz/signoz.git && cd signoz/deploy/
cd docker
docker compose up -d --remove-orphans
```

## AbpFramework(.NetFramework4.6.2及以上版本)
https://github.com/ddrsql/OTelTest/tree/main/AbpFramework

## AbpCore(.NetCore3.1及以上版本)
https://github.com/ddrsql/OTelTest/tree/main/AbpCore

## Volo.Abp
https://github.com/ddrsql/OTelTest/tree/main/VoloAbp



参考

https://lecarvalho.medium.com/opentelemetry-logs-using-log4net-f573a800c627