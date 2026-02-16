```mermaid
graph TD
    subgraph Client_Layer["客户端层"]
        Client[浏览器/移动端]
    end

    subgraph LB["负载均衡器"]
        Nginx[Nginx / HAProxy]
    end

    subgraph Web_Cluster["ASP.NET Core (Orleans Client) 集群"]
        Web1[Web 实例 1]
        Web2[Web 实例 2]
        WebN[Web 实例 N]
    end

    subgraph Cache["Redis 集群"]
        Redis[Redis Master + Replicas]
    end

    subgraph Search["OpenSearch 集群"]
        OS[OpenSearch 主节点 + 数据节点]
    end

    subgraph Silo_Cluster["Orleans Silo 集群"]
        Silo1[Silo 节点 1]
        Silo2[Silo 节点 2]
        SiloN[Silo 节点 N]
    end

    subgraph DB["MySQL 集群"]
        MySQL_Master[MySQL 主库]
        MySQL_Slave[MySQL 从库]
    end

    subgraph Queue["Kafka 集群"]
        Kafka[Kafka Broker]
    end

    OS_Updater[OpenSearch 更新服务]

    Client -->|"HTTPS"| Nginx
    Nginx -->|"分发请求"| Web1
    Nginx -->|"分发请求"| Web2
    Nginx -->|"分发请求"| WebN

    Web1 -->|"直连读缓存 (RESP)"| Redis
    Web2 -->|"直连读缓存"| Redis
    WebN -->|"直连读缓存"| Redis

    Web1 -->|"直连搜索 (HTTP)"| OS
    Web2 -->|"直连搜索"| OS
    WebN -->|"直连搜索"| OS

    Web1 -->|"Orleans TCP (调用Grain)"| Silo1
    Web1 -->|"Orleans TCP"| Silo2
    Web1 -->|"Orleans TCP"| SiloN
    Web2 -->|"Orleans TCP"| Silo1
    Web2 -->|"Orleans TCP"| Silo2
    Web2 -->|"Orleans TCP"| SiloN
    WebN -->|"Orleans TCP"| Silo1
    WebN -->|"Orleans TCP"| Silo2
    WebN -->|"Orleans TCP"| SiloN

    Silo1 -->|"ADO.NET (MySQL协议)"| MySQL_Master
    Silo2 -->|"ADO.NET"| MySQL_Master
    SiloN -->|"ADO.NET"| MySQL_Master
    Silo1 -->|"可选读从库"| MySQL_Slave
    Silo2 -->|"可选读从库"| MySQL_Slave
    SiloN -->|"可选读从库"| MySQL_Slave

    Silo1 -->|"可选：写后删除缓存"| Redis
    Silo2 -->|"可选：写后删除缓存"| Redis
    SiloN -->|"可选：写后删除缓存"| Redis

    Silo1 -->|"事务性发件箱发布事件 (Kafka协议)"| Kafka
    Silo2 -->|"事务性发件箱发布事件"| Kafka
    SiloN -->|"事务性发件箱发布事件"| Kafka

    Kafka -->|"消费事件 (Kafka协议)"| OS_Updater
    OS_Updater -->|"HTTP (OpenSearch API)"| OS

    style Client fill:#f9f,stroke:#333,stroke-width:2px
    style Nginx fill:#ccf,stroke:#333,stroke-width:2px
    style Web1 fill:#cfc,stroke:#333,stroke-width:2px
    style Web2 fill:#cfc,stroke:#333,stroke-width:2px
    style WebN fill:#cfc,stroke:#333,stroke-width:2px
    style Redis fill:#ffc,stroke:#333,stroke-width:2px
    style OS fill:#ffc,stroke:#333,stroke-width:2px
    style Silo1 fill:#cff,stroke:#333,stroke-width:2px
    style Silo2 fill:#cff,stroke:#333,stroke-width:2px
    style SiloN fill:#cff,stroke:#333,stroke-width:2px
    style MySQL_Master fill:#fcc,stroke:#333,stroke-width:2px
    style MySQL_Slave fill:#fcc,stroke:#333,stroke-width:2px
    style Kafka fill:#ddd,stroke:#333,stroke-width:2px
    style OS_Updater fill:#e6e6fa,stroke:#333,stroke-width:2px
