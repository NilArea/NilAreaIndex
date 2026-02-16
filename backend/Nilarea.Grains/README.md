## Orleans具体实现

## 职责范围

项目的Orleans相关代码

### 核心功能

- 数据库实体管理(RedisFactory,DbContext),数据库实体(Dbe)
- Grain实现(ImplGrains)
- EFCore处理(Migrations)
- 单例服务(Services)[数据库操作,邮件发送...],服务初始化(ServiceInitializer)
